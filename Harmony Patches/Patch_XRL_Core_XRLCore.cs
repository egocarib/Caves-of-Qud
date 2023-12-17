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

        /* Vé.aisse update

            Apparently player GameObject is set later than it was when the mod was made.
            Since both events, EmbarkEvent and OnLoadAlwaysEvent need a player GameObject,
            I'm manually calling those in a bootGame postfix, at which point player will
            be initialized properly. 

        */

        // [HarmonyTranspiler]
        // [HarmonyPatch("bootGame")]
        // static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        // {
        //     var Sequence = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
        //     {
        //         new PatchTargetInstruction(OpCodes.Ldstr, "Starting game!")
        //     });

        //     bool patched = false;
        //     foreach (var instruction in instructions)
        //     {
        //         if (!patched && Sequence.IsMatchComplete(instruction))
        //         {
        //             yield return new CodeInstruction(OpCodes.Call, Events_EmbarkEvent);
        //             yield return new CodeInstruction(OpCodes.Call, Events_OnLoadAlwaysEvent);
        //             patched = true;
        //         }
        //         yield return instruction;
        //     }
        //     if (patched)
        //     {
        //         PatchHelpers.LogPatchResult("XRLCore.bootGame",
        //             "Patched successfully." /* Enables an event framework that other QudUX features rely on. */ );
        //     }
        //     else
        //     {
        //         PatchHelpers.LogPatchResult("XRLCore.bootGame",
        //             "Failed. This patch may not be compatible with the current game version. "
        //             + "Custom tiles chosen during character creation won't be properly applied at game start, "
        //             + "and certain other event-based QudUX features might not work as expected.");
        //     }
        // }

        [HarmonyPostfix]
        [HarmonyPatch("bootGame")]
        public static void PostFix()
        {
            Concepts.Events.EmbarkEvent();
            Concepts.Events.AlwaysLoadEvent("boot game");
        }
    }

    [HarmonyPatch(typeof(XRL.XRLGame))]
    class Patch_XRL_Core_XRLCore
    {
        /* Vé.aisse updates

            LoadGame Method changed location
            XRL.Core.XRLCore::LoadGame --> XRL.XRLGame::LoadGame
        */

        [HarmonyPostfix]
        [HarmonyPatch("LoadGame")]
        static void Postfix()
        {
            try
            {
                QudUX.Concepts.Events.SaveLoadEvent();
                QudUX.Concepts.Events.AlwaysLoadEvent("load everything");
            }
            catch(System.Exception e) 
            {
                Utilities.Logger.Log(e.Message);
            }
        }
    }
}
