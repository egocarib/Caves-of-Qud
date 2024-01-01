using System.Collections;
using System.Collections.Generic;
using XRL;
using XRL.Core;
using XRL.UI;
using XRL.World;
using XRL.World.AI.Pathfinding;
using XRL.World.Capabilities;
using static XRL.Messages.MessageQueue;

namespace QudUX.AutoActActions
{
    public class PickupSelection : OngoingAction
    {

        private StateEnum _PickupState;
        private List<GameObject> _Targets;
        private GameObject _Player;
        private Cell _TargetCell;

        public PickupSelection(List<GameObject> targets, GameObject player) : base()
        {
            _Targets = targets;
            _Player = player;
        }
        public override bool IsMovement()
        {
            return _PickupState == StateEnum.Moving;
        }

        public override bool IsGathering()
        {
            return _PickupState == StateEnum.Gathering;
        }

        public override bool CanComplete()
        {
            return _Targets.Count == 0;
        }

        public override bool Continue()
        {
            FindPath path = GetPath();
            if (path == null)
            {
                Popup.Show("You can't figure out how to reach any of your targets.");
                return false;
            }

            _TargetCell = path.Steps[path.Steps.Count - 1];

            if (_TargetCell.IsAdjacentTo(_Player.CurrentCell) || _TargetCell == _Player.CurrentCell)
            {
                string pickupDebug = "Trying to pickup...";
                _PickupState = StateEnum.Gathering;
                List<GameObject> pickedItems = new List<GameObject>();
                foreach (GameObject target in _TargetCell.GetObjects(obj => _Targets.Contains(obj)))
                {
                    if (_Player.TakeObject(target, false, false, null, "QuickPickup"))
                    {
                        pickedItems.Add(target);
                    }
                }

                if(pickedItems.Count > 0)
                {
                    pickupDebug += " And failed ?";
                }
                else pickupDebug += " And succeeded.";

                foreach (GameObject item in pickedItems)
                {
                    _Targets.Remove(item);
                }

                if (_Targets.Count == 0)
                {
                    AddPlayerMessage("&WYou picked all the items you were interested in.");
                    The.Core.PlayerAvoid.Clear();
                    return false;
                }
            }
            
            _PickupState = StateEnum.Moving;
            The.Core.PlayerAvoid.Enqueue(new XRLCore.SortPoint(_Player.CurrentCell.X, _Player.CurrentCell.Y));

            bool moved = AutoAct.TryToMove(
                _Player,
                _Player.CurrentCell,
                path.Steps[1],
                path.Directions[0]
            );

            if (!moved)
            {
                Popup.Show("You can't figure out how to reach any of your targets.");
                return false;
            }

            AutoAct.Action = this;
            return true;
        }

        private FindPath GetPath()
        {
            List<FindPath> pathes = new List<FindPath>();

            foreach (GameObject go in _Targets)
                pathes.Add(new FindPath(_Player.CurrentCell, go.CurrentCell, false, true, _Player, 95, true, false, false, false, false, The.Core.PlayerAvoid));

            pathes.Sort((a, b) => a.Steps.Count - b.Steps.Count);
            foreach (FindPath p in pathes)
            {
                if (p.Usable) return p;
            }

            return null;
        }

        public override string GetDescription()
        {
            return "Quick Pickup";
        }

        private enum StateEnum
        {
            Moving,
            Gathering
        }
    }
}