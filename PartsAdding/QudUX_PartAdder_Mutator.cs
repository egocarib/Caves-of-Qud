using XRL.World;
using XRL.World.Parts;
using QudUX.Utilities;

namespace XRL
{
    [PlayerMutator]
    public class QudUX_PartAdder_Mutator : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            player.AddPart<QudUX_AutogetHelper>();
            player.AddPart<QudUX_CommandListener>();
            player.AddPart<QudUX_ConversationHelper>();
            player.AddPart<QudUX_LegendaryInteractionListener>();

            QudUX.Utilities.Logger.Log("Added parts on player through mutator");
        }
    }
}
