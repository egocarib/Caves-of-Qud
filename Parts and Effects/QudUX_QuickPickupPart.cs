

using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleLib.Console;
using QudUX.Concepts;
using XRL.Core;
using XRL.UI;
using XRL.World.Capabilities;

namespace XRL.World.Parts
{
    public class QudUX_QuickPickupPart : IPart
    {
        private const string _DisplaySettings = "QudUX_OpenQuickPickupSettingsMenu";
        private const string _DisplayQuickMenu = "QudUX_OpenQuickPickupQuickMenu";

        public Dictionary<string, ValueTuple<bool, string>> TierSettings = new Dictionary<string, ValueTuple<bool, string>>
        {
            { "Tier 0", (true, "0") },
            { "Tier 1", (true, "1") },
            { "Tier 2", (true, "2") },
            { "Tier 3", (true, "3") },
            { "Tier 4", (true, "4") },
            { "Tier 5", (true, "5") },
            { "Tier 6", (true, "6") },
            { "Tier 7", (true, "7") },
            { "Tier 8", (true, "8") },
        };


        // Look for inheritance of time Tuple.Item2
        public Dictionary<string, ValueTuple<bool, string>> TypesSettings = new Dictionary<string, ValueTuple<bool, string>>
        {
            { "Axes",           (true, "BaseAxe") },
            { "Daggers",        (true, "BaseDagger") },
            { "Cudgels",        (true, "BaseCudgel") },
            { "Pistols",        (true, "BasePistol") },
            { "Rifles",         (true, "BaseRifle") },
            { "Heavy Weapons",  (true, "BaseHeavyWeapon") },
            { "Bows",           (true, "BaseBow") },
            { "Shields",        (true, "BaseShield") },
        };

        // Look for part "Armor" worn on Tuple.item2
        public Dictionary<string, ValueTuple<bool, string>> ArmorSettings = new Dictionary<string, ValueTuple<bool, string>>
        {
            { "Head",               (true, "Head")},
            { "Face",               (true, "Face")},
            { "Body",               (true, "Body")},
            { "Hand",               (true, "Hands")},
            { "Arm",                (true, "Arm")},
            { "Feet",               (true, "Feet")},
            { "Back",               (true, "Back")},
            { "Floating Nearby",    (true, "Floating Nearby")},
        };

        public override void Register(GameObject Object)
        {
            AddPlayerMessage("Registering events !", 'R');

            Object.RegisterPartEvent(this, _DisplayQuickMenu);
            Object.RegisterPartEvent(this, _DisplaySettings);

            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == _DisplayQuickMenu)
            {
                GoToNextCell();
            }

            if (E.ID == _DisplaySettings)
            {
                var screen = new QudUX_QuickPickupSettingsScreen();
                screen.Show(ParentObject);
            }

            return base.FireEvent(E);
        }

        private void GoToNextCell()
        {
            List<GameObject> selection = new List<GameObject>();
            List<GameObject> exclusions = new List<GameObject>();
            Func<GameObject, string, bool> predicate;

            foreach (Cell c in ParentObject.CurrentZone.GetExploredCells())
            {   
                // Filtering on Tier tag value
                // Simple Tier filtering
                predicate = (GameObject candidate, string value) => candidate.HasTag("Tier") && candidate.GetTier() ==  int.Parse(value);
                GetObjectsFiltered(
                    c,
                    TierSettings,
                    selection,
                    exclusions,
                    predicate
                );

                // Filtering on candidate inheritance
                // used to see if weapons inherits from some baseObject
                predicate = (GameObject candidate, string value) => candidate.GetBlueprint().InheritsFrom(value);
                GetObjectsFiltered(
                    c,
                    TypesSettings,
                    selection,
                    exclusions,
                    predicate
                );

                // Filtering armor based on their body target
                predicate = (GameObject candidate, string value) =>
                {
                    if (candidate.TryGetPart(out Armor armor))
                    {
                        return armor.WornOn == value;
                    }
                    return false;
                };

                GetObjectsFiltered(
                    c,
                    ArmorSettings,
                    selection,
                    exclusions,
                    predicate
                );
            }

            if (selection.Count > 0)
            {
                selection.OrderByDescending(obj =>
                {
                    if(!obj.HasTag("Tier")) return 0;
                    return obj.GetTier();
                });

                List<string> options = new List<string>();
                foreach (var obj in selection)
                    options.Add(obj.BaseDisplayName);

                List<IRenderable> icons = new List<IRenderable>();
                foreach (var obj in selection)
                    icons.Add(obj.RenderForUI());

                List<int> results = Popup.PickSeveral(
                    "Which item do you want to get ?",
                    options.ToArray(),
                    AllowEscape: true,
                    Icons: icons.ToArray()
                );

                foreach (int index in results)
                {
                    GameObject current = selection[index];

                    if (current.HasIntProperty("NoAutoget"))
                        current.RemoveIntProperty("NoAutoget");

                    if (current.HasIntProperty("DroppedByPlayer"))
                        current.RemoveIntProperty("DroppedByPlayer");

                    if (current.HasIntProperty("Autoexplored"))
                        current.RemoveIntProperty("Autoexplored");

                    AutoAct.SetAutoexploreSuppression(current, false);
                    AutoAct.SetAutoexploreActionProperty(current, "Autoget", -1);
                    current.SetStringProperty(Constants.QuickPickupProperty, "yes");
                }

                The.Player.CurrentZone.FlushNavigationCaches();
                AutoAct.Setting = "?";
                ActionManager.SkipPlayerTurn = true;
                return;
            }

            Popup.Show("There is nothing of interest to pick up here...");
        }

        private void GetObjectsFiltered(Cell targetCell, Dictionary<string, ValueTuple<bool, string>> settings, List<GameObject> selectedObjects, List<GameObject> excludedObjects, Func<GameObject, string, bool> predicate)
        {
            // Getting all objects on currentCell
            var allObjects = targetCell.GetObjectsThatInheritFrom("Item");
            foreach(var key in settings.Keys)
            {
                // Removing already excluded objects so far
                allObjects = allObjects.Where(obj => !excludedObjects.Contains(obj)).ToList();

                //Step out if we don't have any other object
                if(allObjects.Count == 0) return;

                var tuple = settings[key];

                // Filtering candidates using passed on predicate, fed with tuple value
                var candidates = allObjects.Where(obj => predicate(obj, tuple.Item2));

                // Exclude what we found if disabled in settings
                if(!tuple.Item1)
                {
                    excludedObjects.AddRange(candidates);
                    continue;
                }

                // Add them to selection otherwise
                selectedObjects.AddRange(candidates);
            }
        }
    }
}