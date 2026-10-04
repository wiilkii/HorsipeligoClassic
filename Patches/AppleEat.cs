using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Apples;
using Horsipelago;
using Horsipelago.Models;



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
        int id;
        Plugin.BepinLogger.LogInfo("SetCollected called!");
        Plugin.BepinLogger.LogInfo($"Apple ID: {__instance.ID}");
        AppleTracker.Data[__instance.ID] = AppleTracker.Data[__instance.ID] with { Found = true };
        // Plugin.Log.LogInfo(__instance.ID);
        if (__instance.ID == 0)
        {
            id = 100;
        }
        else
        {
            id = __instance.ID;
        }

        Plugin.ArchipelagoClient.sendLocationCheck(id);
    }
}