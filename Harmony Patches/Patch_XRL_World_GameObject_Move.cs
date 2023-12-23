using System;
using System.Linq;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using static QudUX.HarmonyPatches.PatchHelpers;
using static QudUX.Concepts.Constants.MethodsAndFields;
using System.Reflection;
using XRL.World;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch]
    class Patch_XRL_World_GameObject_Move
    {
        static MethodInfo TargetMethod()
        {
            List<MethodInfo> gameObjectMethod = AccessTools.GetDeclaredMethods(typeof(XRL.World.GameObject));
            foreach(MethodInfo method in gameObjectMethod)
            {
                if(method.Name == "Move")
                {
                    ParameterInfo[] arguments = method.GetParameters();

                    if(arguments.Length > 2 && arguments[0].ParameterType == typeof(string) && arguments[1].ParameterType == typeof(GameObject).MakeByRefType())
                    {
                        return method;
                    }
                }
            }
            
            return null;
        }


        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler_Move1(IEnumerable<CodeInstruction> instructions)
        {
            var Sequence = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldstr, "You cannot go that way."),
                new PatchTargetInstruction(OpCodes.Call, MessageQueue_AddPlayerMessage, 2)
            });

            bool patched = false;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (!patched && Sequence.IsMatchComplete(instruction))
                {
                    yield return new CodeInstruction(OpCodes.Ldstr, "{{w|Can't go that way!}},{{w|Nothing there!}}");
                    yield return new CodeInstruction(OpCodes.Call, ParticleTextMaker_EmitFromPlayer);
                    patched = true;
                }
            }
            ReportPatchStatus("First part", patched);
        }

        [HarmonyTranspiler]
        static IEnumerable<CodeInstruction> Transpiler_Move2(IEnumerable<CodeInstruction> instructions)
        {
            // Look for the sequence handling movements into deep liquid
            var Sequence1 = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldloc_S), // push liquid GameObject to stack
                new PatchTargetInstruction(OpCodes.Callvirt, 0),
                new PatchTargetInstruction(OpCodes.Stelem_Ref, 0),
                new PatchTargetInstruction(OpCodes.Dup, 0),
                new PatchTargetInstruction(OpCodes.Ldc_I4_5, 0),
                new PatchTargetInstruction(OpCodes.Ldstr, " and start swimming.", 0),
                new PatchTargetInstruction(OpCodes.Stelem_Ref, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Ldloc_S, 0),
                new PatchTargetInstruction(OpCodes.Ldc_I4_1, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Ldarg_1, 0),
                new PatchTargetInstruction(OpCodes.Stfld, XRLCore_MoveConfirmDirection, 0)
            });

            // Look for the sequence handling movement into shallow liquid
            var Sequence2 = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldstr, " again to confirm."),
                new PatchTargetInstruction(OpCodes.Stelem_Ref, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Ldc_I4_S, 0),
                new PatchTargetInstruction(OpCodes.Ldc_I4_1, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Ldarg_1, 0),
                new PatchTargetInstruction(OpCodes.Stfld, XRLCore_MoveConfirmDirection, 0)
            });

            int seq = 1;
            bool patched = false;
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (seq == 1)
                {
                    if (Sequence1.IsMatchComplete(instruction))
                    {
                        yield return Sequence1.MatchedInstructions[0].Clone();
                        yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                        yield return new CodeInstruction(OpCodes.Call, ParticleTextMaker_EmitFromPlayerIfLiquid);
                        seq++;
                    }
                }
                else if (!patched && Sequence2.IsMatchComplete(instruction))
                {
                    yield return Sequence1.MatchedInstructions[0].Clone();
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Call, ParticleTextMaker_EmitFromPlayerIfLiquid);
                    patched = true;
                }
            }

            ReportPatchStatus("second part", patched);
        }

        private static readonly Dictionary<string, bool> PatchStatuses = new Dictionary<string, bool>();
        private static void ReportPatchStatus(string patchname, bool success)
        {
            PatchStatuses.Add(patchname, success);
            if (PatchStatuses.Count >= 2)
            {
                int failCount = PatchStatuses.Where(s => !s.Value).Count();
                if (failCount > 0)
                {
                    string msg = "both";
                    if (failCount < 2)
                    {
                        msg = PatchStatuses.Where(s => !s.Value).ToArray()[0].Key;
                    }
                    PatchHelpers.LogPatchResult("GameObject.Move",
                        $"Failed ({msg}). This patch may not be compatible with the current game version. "
                        + "Some particle text effects may not be shown when movement is prevented.");
                }
                else
                {
                    PatchHelpers.LogPatchResult("GameObject.Move",
                        "Patched successfully." /* Adds option to show particle text messages when movement is prevented for various reasons. */ );
                }
                PatchStatuses.Clear();
            }
        }
    }
}
