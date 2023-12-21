using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using static QudUX.HarmonyPatches.PatchHelpers;
using static QudUX.Concepts.Constants.MethodsAndFields;
using UnityEngine;
using UnityEngine.SearchService;
using Trivial.CodeSecurity;
using System.Windows.Forms.VisualStyles;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.UI.Look))]
    public class Patch_XRL_UI_Look
    {

        [HarmonyTranspiler]
        [HarmonyPatch("ShowLooker")]
        [HarmonyDebug]
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //First sequence changes value of text before any rendering
            //Third sequence adds hotkey to mark legendary
            var Sequence1 = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {   
                new PatchTargetInstruction(OpCodes.Stloc_S), //Save stack value into text variable
                new PatchTargetInstruction(OpCodes.Ldloc_S, 0), //Load Target GameObject on stack
                new PatchTargetInstruction(OpCodes.Ldloc_S, 1), //Load UI Hotkey string on stack
                new PatchTargetInstruction(OpCodes.Ldstr, "CmdWalk"),
                new PatchTargetInstruction(OpCodes.Ldstr, " | {{hotkey|n}} show navweight", 8),
                new PatchTargetInstruction(OpCodes.Call, 0),
                new PatchTargetInstruction(OpCodes.Stloc_S, 0),
                new PatchTargetInstruction(OpCodes.Ldloc_S, 0) //load num
            });
            var Sequence2 = new PatchTargetInstructionSet(new List<PatchTargetInstruction>
            {
                new PatchTargetInstruction(OpCodes.Ldloc_S),// load key to stack
                new PatchTargetInstruction(OpCodes.Ldc_I4_S, (object)101, 0), // load keycode on stack
                new PatchTargetInstruction(OpCodes.Beq_S, 0), // compare and branch according to last values
                new PatchTargetInstruction(OpCodes.Ldloc_S, 0),// load key to stack
                new PatchTargetInstruction(OpCodes.Ldc_I4_S, (object)27, 0),// load keycode on stack
                new PatchTargetInstruction(OpCodes.Bne_Un_S, 0), // compare and branch according to last values
                new PatchTargetInstruction(OpCodes.Ldc_I4_1, 0), // load 1 on stack
                new PatchTargetInstruction(OpCodes.Stloc_2, 0), // push value on stack into var index 2
                new PatchTargetInstruction(OpCodes.Ldloc_S, 0)  // load key to stack
            });

            int seq = 1;
            bool patched = false;
            foreach (var instruction in instructions)
            {
                if(seq == 1)
                {
                    if(Sequence1.IsMatchComplete(instruction)) 
                    {   
                        //modify hotkey text according to Look target
                        CodeInstruction loadNum = instruction.Clone();
                        // instruction bears label for a branching happening right before it
                        // cloning creates an identical instruction minus labels
                        // I just add labels to the first instruction I insert below
                        // Then I yield the copy of the current instruction made above last
                        // Effectively moving the labels up, at the first instruction I add
                        yield return Sequence1.MatchedInstructions[2].Clone().MoveLabelsFrom(instruction); //push text string with labels on stack on stack
                        yield return Sequence1.MatchedInstructions[1].Clone(); //push target 
                        yield return new CodeInstruction(OpCodes.Call, LookExtender_ReturnModifiedString); // Modify string and push on stack
                        yield return Sequence1.MatchedInstructions[0].Clone(); //save to string var;
                        yield return loadNum; 
                        seq++;
                        continue;
                    }
                }
                else if (!patched)
                {
                    if (Sequence2.IsMatchComplete(instruction))
                    {
                        //Adding keypress handling to mark legendary
                        CodeInstruction loadKeyWithoutLabel = instruction.Clone(); 
                        //Here we do basically the same as above, since our stop instruction
                        //also bears labels. We simply clone it to discard its labels,
                        //transfer labels to our first instruction, do our things, and
                        //yield the instruction without the labels, to keep the original behaviour
                        yield return Sequence2.MatchedInstructions[0].Clone().MoveLabelsFrom(instruction); // pushing current key input on stack, with labels
                        yield return Sequence1.MatchedInstructions[1].Clone(); // pushing target GameObject to stack
                        yield return new CodeInstruction(OpCodes.Ldloc_2); // pushing bool var "flag" on stack. Responsible for stepping out of look loop
                        yield return new CodeInstruction(OpCodes.Call, LookExtender_CheckKeyPress); //call our method, which puts a bool on stack
                        yield return new CodeInstruction(OpCodes.Stloc_2); // and poping our bool from stack into flag var, to update
                        yield return loadKeyWithoutLabel; // Finally pushing our stop instruction, to maintain original behaviour
                        patched = true;
                        continue;
                    }
                }
                yield return instruction;
            }
            if (patched)
            {
                PatchHelpers.LogPatchResult("Look",
                    "Patched successfully." /* Adds option to mark legendary creature locations in the journal from the Look UI. */ );
            }
            else
            {
                PatchHelpers.LogPatchResult("Look",
                    "Failed. This patch may not be compatible with the current game version. "
                    + "The option to mark legendary creature locations in journal won't be available from the Look UI.");
            }
        }
    }
}
