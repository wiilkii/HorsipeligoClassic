using HarmonyLib;
using Horsipelago.Archipelago;
using Horsipelago;
using HorseRidingClassic.Source.Player;

namespace Horsipelago.Patches;

[HarmonyPatch(typeof(OnPlayerDeathEvent), "TriggerDeathEvent")]
public static class DeathPatch
{
    static void Prefix()
    {
        Plugin.BepinLogger.LogInfo("no :( died");
    }
}