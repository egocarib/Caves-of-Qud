using ConsoleLib.Console;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using static XRL.World.Parts.QudUX_LegendaryInteractionListener;
using LegacyKeyCode = ControlManager.LegacyKeyCode;
using KeyCode = UnityEngine.KeyCode;
using UnityEngine.XR;
using Qud.UI;

namespace QudUX.ScreenExtenders
{
    public class LookExtender
    {
        private static Keys? MarkKey = null;

        public static void AddMarkLegendaryOptionToLooker(ScreenBuffer buffer, GameObject target, string uiHotkeyString)
        {
            if ((target.HasProperty("Hero") || target.GetStringProperty("Role") == "Hero") && target.HasPart(typeof(GivesRep)))
            {
                LegacyKeyCode buttonA = ControlManager.mapCommandToPrimaryLegacyKeycode("CmdWalk");

                if (buttonA.code != KeyCode.M)
                {
                    MarkKey = Keys.M;
                    buffer.WriteAt(1, 0, uiHotkeyString + " | {{hotkey|M}} - mark in journal");
                }
                else if (buttonA.code != KeyCode.J)
                {
                    MarkKey = Keys.J;
                    buffer.WriteAt(1, 0, uiHotkeyString + " | {{hotkey|J}} - mark in journal");
                }
            }
        }

        public static string ReturnModifiedString(string uiHotkeyString, GameObject target)
        {
            if ((target.HasProperty("Hero") || target.GetStringProperty("Role") == "Hero") && target.HasPart(typeof(GivesRep)))
            {
                LegacyKeyCode buttonA = ControlManager.mapCommandToPrimaryLegacyKeycode("CmdWalk");

                if (buttonA.code != KeyCode.M)
                {
                    MarkKey = Keys.M;
                    uiHotkeyString += " | {{hotkey|M}} - mark in journal";
                }
                else if (buttonA.code != KeyCode.J)
                {
                    MarkKey = Keys.J;
                    uiHotkeyString += " | {{hotkey|J}} - mark in journal";
                }
            }
            
            return uiHotkeyString;
        }

        public static void SetModerUIText(string uiHotkeyString, GameObject target)
        {
            if ((target.HasProperty("Hero") || target.GetStringProperty("Role") == "Hero") && target.HasPart(typeof(GivesRep)))
            {
                LegacyKeyCode buttonA = ControlManager.mapCommandToPrimaryLegacyKeycode("CmdWalk");

                if (buttonA.code != KeyCode.M)
                {
                    MarkKey = Keys.M;
                    uiHotkeyString += " | {{hotkey|M}} - mark in journal";
                }
                else if (buttonA.code != KeyCode.J)
                {
                    MarkKey = Keys.J;
                    uiHotkeyString += " | {{hotkey|J}} - mark in journal";
                }
            }

            // PickTargetWindow.currentText = uiHotkeyString;
        }

        public static bool CheckKeyPress(Keys key, GameObject target, bool currentKeyFlag)
        {
            if (currentKeyFlag == true) //already processing a different key request
            {
                return true;
            }
            if (MarkKey != null && key == MarkKey && (target.HasProperty("Hero") || target.GetStringProperty("Role") == "Hero") && target.HasPart(typeof(GivesRep)))
            {
                ToggleLegendaryLocationMarker(target);
                return true;
            }
            return false;
        }
    }
}
