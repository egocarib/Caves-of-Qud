using HarmonyLib;
using QudUX.Concepts;
using XRL.Messages;

namespace QudUX.HarmonyPatches
{
    [HarmonyPatch(typeof(XRL.World.GameObject))]
    class Patch_XRL_World_GameObject_CanAutoget
    {
        [HarmonyPrefix]
        [HarmonyPatch("CanAutoget")]
        static bool Postfix(XRL.World.GameObject __instance, ref bool __result)
        {
            __result = !XRL.World.Parts.QudUX_AutogetHelper.IsAutogetDisabledByQudUX(__instance);
            return __result;
        }
    }
}
