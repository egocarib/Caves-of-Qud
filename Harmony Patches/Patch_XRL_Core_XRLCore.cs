using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static QudUX.HarmonyPatches.PatchHelpers;
using static QudUX.Concepts.Constants.MethodsAndFields;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.CharacterBuilds.Qud.QudGameBootModule))]
    class Patch_XRL_CharacterBuilds_Qud_QudGameBootModule
    {
        [HarmonyTranspiler]
        [HarmonyPatch("bootGame")]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var Sequence = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldstr, "Starting game!")
            });

            bool patched = false;
            foreach (var instruction in instructions)
            {
                if (!patched && Sequence.IsMatchComplete(instruction))
                {
                    yield return new CodeInstruction(OpCodes.Call, Events_EmbarkEvent);
                    yield return new CodeInstruction(OpCodes.Call, Events_OnLoadAlwaysEvent);
                    patched = true;
                }
                yield return instruction;
            }
            if (patched)
            {
                PatchHelpers.LogPatchResult("XRLCore.NewGame",
                    "Patched successfully." /* Enables an event framework that other QudUX features rely on. */ );
            }
            else
            {
                PatchHelpers.LogPatchResult("XRLCore.NewGame",
                    "Failed. This patch may not be compatible with the current game version. "
                    + "Custom tiles chosen during character creation won't be properly applied at game start, "
                    + "and certain other event-based QudUX features might not work as expected.");
            }
        }
    }

    [HarmonyPatch(typeof(XRL.Core.XRLCore))]
    class Patch_XRL_Core_XRLCore
    {
        [HarmonyPrefix]
        [HarmonyPatch("RunGame")]
        static void Prefix()
        {
            try
            {
                QudUX.Concepts.Events.SaveLoadEvent();
                QudUX.Concepts.Events.OnGameRuns();
            }
            catch { }
        }
    }
}
