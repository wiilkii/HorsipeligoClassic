using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Apples;
using Horsipelago;




namespace Horsipelago.Patches;

[HarmonyPatch(typeof(Apple), "SetCollected")]
public static class AppleEatPatch
{
    static void Prefix()
    {
        Plugin.BepinLogger.LogInfo("SetCollected called!");
    }

    static void Postfix(Apple __instance)
    {
        Plugin.BepinLogger.LogInfo("SetCollected called!");
        Plugin.BepinLogger.LogInfo($"Apple ID: {__instance.ID}");
        // Plugin.Log.LogInfo(__instance.ID);
    }
}