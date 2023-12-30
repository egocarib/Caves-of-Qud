
using System;
using System.Collections.Generic;
using ConsoleLib.Console;
using XRL.World;
using XRL.World.Parts;

namespace XRL.UI
{
    [UIView("QudUX:QuickPickupSettings", ForceFullscreen: false, NavCategory: "Menu", UICanvas: null)]
    public class QudUX_QuickPickupSettingsScreen : IScreen, IWantsTextConsoleInit
    {
        private static readonly Dictionary<string, string> _ItemDescription = new Dictionary<string, string>()
        {
            { "Tier 0",             "&yEvery {{w|Bronze}} item"},
            { "Tier 1",             "&yIron mace, Stun rod, Desert Kris, Iron Vinereaper, Stun & Leather whips, Iron long swords... The least powerful of Iron Items"},
            { "Tier 2",             "&ySome more powerful Iron items like the Two handed axe"},
            { "Tier 3",             "&y{{b|Carbide}} equipment and weapons"},
            { "Tier 4",             "&y{{B|Folded Carbide}} equipment and weapons"},
            { "Tier 5",             "&y{{K|Fullerite}} equipment and weapons"},
            { "Tier 6",             "&y{{crysteel|Crysteel}} equipment and weapons"},
            { "Tier 7",             "&y{{K|Flawless}} {{crysteel|Crysteel}} equipment and weapons"},
            { "Tier 8",             "&y{{zetachrome|Zetachrome}} equipment and weapons"},
            { "Head",               "&yAll {{W|Head armor}} of the game"},
            { "Face",               "&yAll {{W|Face armor}} of the game"},
            { "Body",               "&yAll {{W|Body armor}} of the game"},
            { "Hand",               "&yAll {{W|Hand armor}} of the game"},
            { "Arm",                "&yAll {{W|Arm armor}} of the game"},
            { "Feet",               "&yAll {{W|Feet armor}} of the game"},
            { "Back",               "&yAll {{W|Back armor}} of the game"},
            { "Floating Nearby",    "&yAll {{W|Floating Nearby items}} of the game"},
            { "Axes",               "&yAll {{W|Axes}} items"},
            { "Daggers",            "&yAll {{W|Daggers}} of the game"},
            { "Cudgels",            "&yAll {{W|Cudgels}} of the game"},
            { "Pistols",            "&yAll {{W|Pistols}} of the game"},
            { "Rifles",             "&yAll {{W|Rifles}} of the game"},
            { "Heavy Weapons",      "&yAll {{W|Heavy Weapons}} of the game"},
            { "Bows",               "&yAll {{W|Bows}} of the game"},
            { "Shields",            "&yAll {{W|Shields}} of the game"},
            { "Armor",              "&yAll {{W|Armor}} of the game"},
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

            int tabFocus = 0;
            int selection = 1;

            if (!GO.TryGetPart<QudUX_QuickPickupPart>(out var part))
            {
                return ScreenReturn.Exit;
            }

            while (!stepOut)
            {
                _Buffer.Clear();
                _Buffer.SingleBox(0, 0, 79, 2, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));
                _Buffer.SingleBox(0, 2, 79, 14, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));
                _Buffer.SingleBox(0, 14, 79, 24, ColorUtility.MakeColor(TextColor.Grey, TextColor.Black));

                string a = "&yYou can decide here {{W|which items}} you want to be listed in";
                string b = "&ythe {{W|Quick Pickup-up QuickMenu}}. Items can either be filtered";
                string c = "&yby their {{W|Tier}} or by their {{W|Type}}. Both filters will be";
                string d = "&y{{W|combined additively}} when scanning nearby items.";
                int height = ((9 - 4) / 2) + 15;
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
                _Buffer.Goto(0, 14);
                _Buffer.Write(195);

                _Buffer.Goto(79, 2);
                _Buffer.Write(180);
                _Buffer.Goto(79, 14);
                _Buffer.Write(180);


                string title = "{{y|[ {{W|Quick Pick-up Settings}} ]}}";
                int startPos = GetCenteredOffsetForString(title);
                _Buffer.Goto(startPos, 0);
                _Buffer.Write(title);

                int third = (80 / 3) - 1;

                string tierTabTitle;
                string itemTypesTabTitle;
                string armorTabTitle;

                if (tabFocus == 0)
                {
                    itemTypesTabTitle = "{{W|> Weapons <}}";
                    tierTabTitle = "{{y|Tiers}}";
                    armorTabTitle = "{{y|Armor}}";
                }
                else
                if (tabFocus == 1)
                {
                    itemTypesTabTitle = "{{y|Weapons}}";
                    armorTabTitle = "{{W|> Armor <}}";
                    tierTabTitle = "{{y|Tiers}}";
                }
                else
                {

                    itemTypesTabTitle = "{{y|Weapons}}";
                    armorTabTitle = "{{y|Armor}}";
                    tierTabTitle = "{{W|> Tiers <}}";
                }

                startPos = (third - ColorUtility.StripFormatting(itemTypesTabTitle).Length) / 2;
                _Buffer.Goto(startPos, 1);
                _Buffer.Write(itemTypesTabTitle);

                startPos = (third - ColorUtility.StripFormatting(armorTabTitle).Length) / 2;
                _Buffer.Goto(third + startPos, 1);
                _Buffer.Write(armorTabTitle);

                startPos = (third - ColorUtility.StripFormatting(tierTabTitle).Length) / 2;
                _Buffer.Goto(2 * third + startPos, 1);
                _Buffer.Write(tierTabTitle);

                int index = 0;

                Dictionary<string, ValueTuple<bool, string>> targetObjects = default;
                switch (tabFocus)
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

                if(selection >= targetObjects.Count)
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
                int maxWidth = 80 - (_ItemDescriptionSectionStart.x - 2);

                string[] words = _ItemDescription[currentKey].Split(" ");
                List<string> lines = new List<string>();
                lines.Add("");
                for (int i = 0; i < words.Length; i++)
                {
                    if (lines[lines.Count - 1].Length + ColorUtility.StripFormatting(words[i]).Length + 1 >= maxWidth)
                    {
                        lines.Add(words[i] + " ");
                        continue;
                    }

                    lines[lines.Count - 1] += words[i] + " ";
                }

                int initHeight = (12 - lines.Count) / 2;
                for (int i = 0; i < lines.Count; i++)
                {
                    _Buffer.Goto(_ItemDescriptionSectionStart.x, _ItemDescriptionSectionStart.y + initHeight + i);
                    _Buffer.Write(lines[i]);
                }

                _Buffer.Goto(0, 0);

                _Console.DrawBuffer(_Buffer);

                Keys keys = Keyboard.getvk(Options.MapDirectionsToKeypad);

                if (keys == Keys.MouseEvent && Keyboard.CurrentMouseEvent.Event == "Command:Toggle")
                {
                    tabFocus = ++tabFocus % 3;
                }
                if (keys == Keys.NumPad6)
                {
                    if (tabFocus < 2)
                        tabFocus = ++tabFocus;
                }
                else
                if (keys == Keys.NumPad4)
                {
                    if (tabFocus > 0)
                        tabFocus = --tabFocus;
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
            return (sectionWidth - ColorUtility.StripFormatting(s).Length) / 2;
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