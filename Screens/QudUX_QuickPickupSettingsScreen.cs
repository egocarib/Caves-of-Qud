
using System;
using System.Linq;
using System.Collections.Generic;
using ConsoleLib.Console;
using XRL.World;
using XRL.World.Parts;
using QudUX.Utilities;

namespace XRL.UI
{
    [UIView("QudUX:QuickPickupSettings", ForceFullscreen: true, NavCategory: "Menu", UICanvas: null)]
    public class QudUX_QuickPickupSettingsScreen : IScreen, IWantsTextConsoleInit
    {
        private static readonly Dictionary<string, string> _ItemDescription = new Dictionary<string, string>()
        {
            { "Tier 0",             "&yEvery {{w|Bronze}} item"},
            { "Tier 1",             "&y{{W|Short Bow}} and least powerful {{W|Iron}} equipment and weapons"},
            { "Tier 2",             "&y{{W|Revolver}}, {{W|Musket}}, {{W|Compound Bow}}... And {{W|Steel}} equipment and weapons"},
            { "Tier 3",             "&y{{b|Carbide}} equipment and weapons"},
            { "Tier 4",             "&y{{B|Folded Carbide}} equipment and weapons"},
            { "Tier 5",             "&y{{K|Fullerite}} equipment and weapons"},
            { "Tier 6",             "&y{{crysteel|Crysteel}} equipment and weapons"},
            { "Tier 7",             "&y{{K|Flawless}} {{crysteel|Crysteel}} equipment and weapons"},
            { "Tier 8",             "&y{{zetachrome|Zetachrome}} equipment and weapons"},
            { "Head",               "&yAll {{W|Head armor}}"},
            { "Face",               "&yAll {{W|Face armor}}"},
            { "Body",               "&yAll {{W|Body armor}}"},
            { "Hand",               "&yAll {{W|Hand armor}}"},
            { "Arm",                "&yAll {{W|Arm armor}}"},
            { "Feet",               "&yAll {{W|Feet armor}}"},
            { "Back",               "&yAll {{W|Back armor}}"},
            { "Floating Nearby",    "&yAll {{W|Floating Nearby items}}"},
            { "Tools",              "&yAll {{W|Tools}} of various purpose, toolkits, Hoversled, portable wall, tatoo gun..."},
            { "Axes",               "&yAll {{W|Axes}} items"},
            { "Daggers",            "&yAll {{W|Daggers}}"},
            { "Long Blades",        "&yAll {{W|Long Blades}}"},
            { "Cudgels",            "&yAll {{W|Cudgels}}"},
            { "Pistols",            "&yAll {{W|Pistols}}"},
            { "Rifles",             "&yAll {{W|Rifles}}"},
            { "Heavy Weapons",      "&yAll {{W|Heavy Weapons}}"},
            { "Bows",               "&yAll {{W|Bows}}"},
            { "Shields",            "&yAll {{W|Shields}}"},
        };

        private static TextConsole _Console;
        private static ScreenBuffer _Buffer;

        private Vector2i _ItemListSectionStart = new Vector2i(3, 4);
        private Vector2i _ItemDescriptionSectionStart = new Vector2i(40, 2);

        public void Init(TextConsole console, ScreenBuffer buffer)
        {
            _Console = console;
            _Buffer = buffer;
        }

        public ScreenReturn Show(GameObject GO)
        {
            GameManager.Instance.PushGameView("QudUX:QuickPickupSettings");
            bool stepOut = false;
            int selection = 1;

            if (!GO.TryGetPart<QudUX_QuickPickupPart>(out var part))
            {
                return ScreenReturn.Exit;
            }

            while (!stepOut)
            {
                _Buffer.Clear();
                _Buffer.SingleBox(0, 0, 79, 2, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));
                _Buffer.SingleBox(0, 2, 79, 17, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));
                _Buffer.SingleBox(0, 17, 79, 24, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));

                string a = "&y{{W|Disabled}} items will be {{W|ignored}} when listing object.";
                string b = "&y{{W|Tier}} and {{W|Type}} filters will both be applied {{W|additively}}.";
                string c = "&cQuick Pickup might perform other Auto-Explore actions";
                string d = "&cbefore or after picking up selected objects.";
                
                int height = ((7 - 4) / 2) + 18;
                _Buffer.Goto(GetCenteredOffsetForString(a), height);
                _Buffer.Write(a);
                _Buffer.Goto(GetCenteredOffsetForString(b), ++height);
                _Buffer.Write(b);
                _Buffer.Goto(GetCenteredOffsetForString(c), ++height);
                _Buffer.Write(c);
                _Buffer.Goto(GetCenteredOffsetForString(d), ++height);
                _Buffer.Write(d);

                _Buffer.Goto(0, 2);
                _Buffer.Write(195);
                _Buffer.Goto(0, 17);
                _Buffer.Write(195);

                _Buffer.Goto(79, 2);
                _Buffer.Write(180);
                _Buffer.Goto(79, 17);
                _Buffer.Write(180);


                string title = "{{y|[ {{W|Quick Pick-up Settings}} ]}}";
                _Buffer.Goto(1, 0);
                _Buffer.Write(title);

                string tierTabTitle;
                string itemTypesTabTitle;
                string armorTabTitle;

                if (part.LastSelectedTab == 0)
                {
                    itemTypesTabTitle = "{{W|> Weapons <}}";
                    tierTabTitle = "{{y|Tiers}}";
                    armorTabTitle = "{{y|Equipment}}";
                }
                else
                if (part.LastSelectedTab == 1)
                {
                    itemTypesTabTitle = "{{y|Weapons}}";
                    armorTabTitle = "{{W|> Equipment <}}";
                    tierTabTitle = "{{y|Tiers}}";
                }
                else
                {
                    itemTypesTabTitle = "{{y|Weapons}}";
                    armorTabTitle = "{{y|Equipment}}";
                    tierTabTitle = "{{W|> Tiers <}}";
                }

                

                int third = 78 / 3;

                int startPos = 1 + (third - ColorUtility.LengthExceptFormatting(itemTypesTabTitle)) / 2;
                _Buffer.Goto(startPos, 1);
                _Buffer.Write(itemTypesTabTitle);

                startPos = third + (third - ColorUtility.LengthExceptFormatting(armorTabTitle)) / 2;
                _Buffer.Goto(startPos, 1);
                _Buffer.Write(armorTabTitle);

                startPos = -1 + 2*third + (third - ColorUtility.LengthExceptFormatting(tierTabTitle)) / 2;
                _Buffer.Goto(startPos, 1);
                _Buffer.Write(tierTabTitle);

                int index = 0;

                Dictionary<string, ValueTuple<bool, string>> targetObjects = default;
                switch (part.LastSelectedTab)
                {
                    case 0:
                        targetObjects = part.TypesSettings;
                        break;
                    case 1:
                        targetObjects = part.ArmorSettings;
                        break;
                    case 2:
                        targetObjects = part.TierSettings;
                        break;
                }

                if (selection >= targetObjects.Count)
                    selection = targetObjects.Count - 1;

                string currentKey = "";
                foreach (var key in targetObjects.Keys)
                {
                    bool isSelected = selection == index;
                    if (isSelected)
                        currentKey = key;
                    DisplayLine(key, targetObjects[key].Item1, index, isSelected);
                    index++;
                }

                _Buffer.Goto(_ItemDescriptionSectionStart.x, _ItemDescriptionSectionStart.y);
                int maxWidth = 78 - _ItemDescriptionSectionStart.x;

                string[] words = _ItemDescription[currentKey].Split(" ");
                List<string> lines = new List<string>();
                lines.Add("");
                
                for (int i = 0; i < words.Length; i++)
                {
                    if (ColorUtility.LengthExceptFormatting(lines[lines.Count - 1] + words[i]) + 1 >= maxWidth)
                    {
                        lines.Add(words[i]);
                        
                        if(i < words.Length-1)
                            lines[lines.Count - 1] += " ";
                        continue;
                    }
                    
                    lines[lines.Count - 1] += words[i];
                    if(i < words.Length-1)
                        lines[lines.Count - 1] += " ";
                }

                int initHeight = (15 - lines.Count) / 2;
                int xOffset = (80 - _ItemDescriptionSectionStart.x - ColorUtility.LengthExceptFormatting(lines.OrderByDescending(s => s.Length).ToArray()[0])) / 2;
                for (int i = 0; i < lines.Count; i++)
                {
                    _Buffer.Goto(_ItemDescriptionSectionStart.x + xOffset, _ItemDescriptionSectionStart.y + initHeight + i);
                    _Buffer.Write(lines[i]);
                }

                int half = 80 / 2;
                string enableAll = "&y{{W|E}}nable All";
                _Buffer.Goto((half - ColorUtility.LengthExceptFormatting(enableAll)) / 2, 16);
                _Buffer.Write(enableAll);

                string disableAll = "&y{{W|D}}isable All";
                _Buffer.Goto(half + (half - ColorUtility.LengthExceptFormatting(disableAll) )/ 2, 16);
                _Buffer.Write(disableAll);

                _Buffer.EscOr5ToExit();
                _Console.DrawBuffer(_Buffer);

                Keys keys = Keyboard.getvk(Options.MapDirectionsToKeypad);

                if (keys == Keys.MouseEvent && Keyboard.CurrentMouseEvent.Event == "Command:Toggle")
                {
                    part.LastSelectedTab = ++part.LastSelectedTab % 3;
                }
                if (keys == Keys.NumPad6)
                {
                    if (part.LastSelectedTab < 2)
                        part.LastSelectedTab = ++part.LastSelectedTab;
                }
                else
                if (keys == Keys.NumPad4)
                {
                    if (part.LastSelectedTab > 0)
                        part.LastSelectedTab = --part.LastSelectedTab;
                }
                else
                if (keys == Keys.NumPad2 || keys == Keys.Down)
                {
                    if (selection < targetObjects.Count - 1)
                        selection++;
                }
                else
                if (keys == Keys.NumPad8 || keys == Keys.Up)
                {
                    if (selection > 0)
                        selection--;
                }
                else
                if (keys == Keys.Space || keys == Keys.Enter)
                {
                    index = 0;
                    string tKey = "";
                    foreach (var key in targetObjects.Keys)
                    {
                        if (index++ == selection)
                        {
                            tKey = key;
                            break;
                        }
                    }

                    targetObjects[tKey] = (!targetObjects[tKey].Item1, targetObjects[tKey].Item2);
                }
                else
                if (keys == Keys.D)
                {
                    SetAllValuesTo(false, targetObjects);
                }
                else
                if (keys == Keys.E)
                {
                    SetAllValuesTo(true, targetObjects);
                }
                else
                if (keys == Keys.Escape || keys == Keys.NumPad5)
                {
                    stepOut = true;
                }
            }

            GameManager.Instance.PopGameView();
            return ScreenReturn.Exit;
        }

        private void Debug_ScreenCoord()
        {
            for (int i = 0; i <= 24; i++)
            {
                _Buffer.Goto(0, i);
                _Buffer.Write(i.ToString());
            }

            for (int i = 0; i <= 80; i++)
            {
                string parsed = i.ToString();
                if (parsed.Length == 2)
                {
                    _Buffer.Goto(i, 0);
                    _Buffer.Write(parsed[0].ToString());
                    _Buffer.Goto(i, 1);
                    _Buffer.Write(parsed[1].ToString());
                }
                else
                {
                    _Buffer.Goto(i, 0);
                    _Buffer.Write(parsed);
                }
            }
        }

        private int GetCenteredOffsetForString(string s, int sectionWidth = 80)
        {
            return (sectionWidth - ColorUtility.LengthExceptFormatting(s)) / 2;
        }

        private void SetAllValuesTo(bool value, Dictionary<string, ValueTuple<bool, string>> objects)
        {
            var k = objects.Keys.ToArray();
            for (int i = 0; i < k.Length; i++)
            {
                objects[k[i]] = (value, objects[k[i]].Item2);
            }
        }

        private void DisplayLine(string name, bool state, int heightOffset, bool isSelected)
        {
            int x = _ItemListSectionStart.x;
            int y = _ItemListSectionStart.y + heightOffset;

            if (isSelected)
            {
                name = "&Y" + name;
            }
            else
            {
                name = "&y" + name;
            }


            string value = "";
            if (state)
            {
                string prefix = isSelected ? "&G" : "&g";
                value = prefix + "Enabled";
            }
            else
            {
                string prefix = isSelected ? "&R" : "&r";
                value = prefix + "Disabled";
            }

            _Buffer.Goto(x, y);
            _Buffer.Write(name);
            _Buffer.Goto(x + 20, y);
            _Buffer.Write(value);
        }
    }
}