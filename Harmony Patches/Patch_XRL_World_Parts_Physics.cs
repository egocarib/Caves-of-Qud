using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static QudUX.HarmonyPatches.PatchHelpers;
using static QudUX.Concepts.Constants.MethodsAndFields;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.Parts.Physics))]
    class Patch_XRL_World_Parts_Physics
    {
        [HarmonyTranspiler]
        [HarmonyPatch("HandleEvent", new Type[] { typeof(XRL.World.ObjectEnteringCellEvent) })]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var Sequence = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldstr, "The way is blocked by ")
            });

            bool patched = false;
            foreach (var instruction in instructions)
            {
                if (!patched && Sequence.IsMatchComplete(instruction))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(instruction);
                    yield return new CodeInstruction(OpCodes.Callvirt, IPart_get_ParentObject);
                    yield return new CodeInstruction(OpCodes.Call, ParticleTextMaker_EmitFromPlayerIfBarrierInDifferentZone);
                    yield return instruction.Clone();
                    patched = true;
                    continue;
                }
                yield return instruction;
            }
            if (patched)
            {
                PatchHelpers.LogPatchResult("Physics.HandleEvent",
                    "Patched successfully." /* Adds option to show particle text messages when movement to connected zone is prevented. */ );
            }
            else
            {
                PatchHelpers.LogPatchResult("Physics.HandleEvent",
                    "Failed. This patch may not be compatible with the current game version. "
                    + "Some particle text effects may not be shown when movement is prevented.");
            }
        }
    }
}
