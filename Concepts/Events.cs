using XRL.Core;
using XRL.World;
using XRL.World.Parts;
using QudUX.ScreenExtenders;

namespace QudUX.Concepts
{
    //Custom events that are called from Patch_XRL_Core_XRLCore.
    //The player object (XRLCore.Core.Game.Player.Body) is available for use in all of these events.
    public static class Events
    {
        private static GameObject Player => XRLCore.Core?.Game?.Player?.Body;

        //Event fired on new character just before embarking. This event occurs later than PlayerMutator
        //which allows doing certain things that would otherwise be impossible, like setting the player's
        //Tile. Normally player tile is overwritten by XRLCore after PlayerMutator runs.
        public static void EmbarkEvent()
        {
            CreateCharacterExtender.ApplyTileInfoDeferred();
        }

        //Runs immediately after a save is loaded.
        public static void SaveLoadEvent()
        {

        }

        
        /*
            I took the liberty to rename the method and change the patch in which it is used.
            Since all it does is adding parts to the player, I just hooked it
            at some point where I knew the player would be here, i.e. prefixing method
            XRL.Core.XRLCore::RunGame()

            XRL.Core.XRLCore::LoadGame doesn't exist anymore and loading appears to 
            work in a way that makes debugging more tedious than just changing the hook.
            I'm a lazy developper.

            ************ OLD COMMENT
            Runs in all load scenarios - called immediately after each of the events above.
        */
        public static void OnGameRuns()
        {
            if (Player != null)
            {
                Player.RequirePart<QudUX_AutogetHelper>();
                Player.RequirePart<QudUX_CommandListener>();
                Player.RequirePart<QudUX_ConversationHelper>();
                Player.RequirePart<QudUX_LegendaryInteractionListener>();
            }
            else
            {
                QudUX.Utilities.Logger.Log("Couldn't require part on player because reference is null. QudUX.Concepts::Events.OnGameRuns()");
            }
        }
    }
}
