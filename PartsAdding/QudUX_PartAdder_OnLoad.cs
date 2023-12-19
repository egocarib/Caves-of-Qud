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

                QudUX.Utilities.Logger.Log("Ensured Parts on player through Game Loaded callback");
            }
        }
    }
}