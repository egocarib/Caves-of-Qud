using HarmonyLib;
using XRL.World;
using QudUX.Concepts;

[HarmonyPatch(typeof(GameObject))]
public class Patch_XRL_World_GameObject_ShouldAutogetusing
{
    [HarmonyPatch("ShouldAutoget"), HarmonyPrefix]
    static bool Prefix(GameObject __instance, ref bool __result)
    {
        __result = __instance.HasIntProperty(Constants.QuickPickupProperty);
        return !__result;
    }
}