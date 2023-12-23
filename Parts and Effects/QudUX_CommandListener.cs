using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class QudUX_CommandListener : IPart
    {
        public static readonly string CmdBatchAddLegendaryEntry = "QudUX_BatchAddLegendaryEntry";
        public static readonly string CmdOpenSpriteMenu = "QudUX_OpenSpriteMenu";
        public static readonly string CmdOpenAutogetMenu = "QudUX_OpenAutogetMenu";
        public static readonly string CmdOpenGameStatsMenu = "QudUX_OpenGameStatsMenu";

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, CmdOpenSpriteMenu);
            Object.RegisterPartEvent(this, CmdOpenAutogetMenu);
            Object.RegisterPartEvent(this, CmdOpenGameStatsMenu);
            Object.RegisterPartEvent(this, CmdBatchAddLegendaryEntry);
            
            base.Register(Object);
        }

		public override bool AllowStaticRegistration()
		{
			return true;
		}
		
        public override bool FireEvent(Event E)
        {
            if (E.ID == CmdOpenSpriteMenu)
            {
                QudUX.Wishes.SpriteMenu.Wish();
            }
            if (E.ID == CmdOpenAutogetMenu)
            {
                QudUX.Wishes.AutopickupMenu.Wish();
            }
            if (E.ID == CmdOpenGameStatsMenu)
            {
                QudUX.Wishes.GameStatsMenu.Wish();
            }
            if (E.ID == CmdBatchAddLegendaryEntry)
            {
                AddPlayerMessage("Batch Add command received");
                QudUX_LegendaryInteractionListener.BatchMarkLegendary();
            }
            return base.FireEvent(E);
        }
    }
}
