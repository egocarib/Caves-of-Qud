using XRL.World;
using XRL.World.Parts;
using QudUX.Utilities;


namespace XRL
{
    [HasCallAfterGameLoaded]
    public class QudUX_PartAdder_OnLoad
    {
        [CallAfterGameLoaded]
        public static void EnsureQudUXParts()
        {
            GameObject player = The.Player;
            
            if(player != null)
            {
                player.RequirePart<QudUX_AutogetHelper>();
                player.RequirePart<QudUX_CommandListener>();
                player.RequirePart<QudUX_ConversationHelper>();
                player.RequirePart<QudUX_LegendaryInteractionListener>();
                player.RequirePart<QudUX_QuickPickupPart>();

                QudUX.Utilities.Logger.Log("Player parts initialized (after game load)");
            }
        }
    }
}