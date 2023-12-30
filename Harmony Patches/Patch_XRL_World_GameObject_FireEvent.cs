using HarmonyLib;
using XRL.World;
using QudUX.Concepts;

[HarmonyPatch(typeof(GameObject))]
public class Patch_XRL_World_GameObject_FireEvent
{
    [HarmonyPatch("FireEvent", typeof(Event)), HarmonyPrefix]
    static void ShouldAutoget(GameObject __instance, Event E)
    {
        if(E.ID == "CommandTakeObject")
        {
            GameObject g = E.GetParameter("Object") as GameObject;
            if(g == null) return;

            if(g.GetStringProperty(Constants.QuickPickupProperty) == "yes")
            {
                g.RemoveStringProperty(Constants.QuickPickupProperty);
            }
        }
    }
}