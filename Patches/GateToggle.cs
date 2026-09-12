using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Apples;
using Horsipelago;
using UnityEngine;
using System.Collections.Generic;


namespace Horsipelago.Patches;

[HarmonyPatch(typeof(AppleGate), "OnCollectedApplesChanged")]
public static class GateTogglePatch
{
    public static HashSet<string> UnlockedGates = new HashSet<string>();


    static bool Prefix(AppleGate __instance)
    {
        string gateName = __instance.gameObject.name;
        if (UnlockedGates.Contains(gateName))
        {
            Plugin.BepinLogger.LogInfo($"Gate '{gateName}' is already unlocked.");
            return false; // Skip the original method if the gate is already unlocked
        }

        Plugin.BepinLogger.LogInfo($"GateToggle called! Gate: {__instance.gameObject.name}");

        var closedGateContent = Traverse.Create(__instance).Field("closedGateContent").GetValue<GameObject>();
        var openGateContent = Traverse.Create(__instance).Field("openGateContent").GetValue<GameObject>();
        bool unlocked = false;

        closedGateContent.SetActive(!unlocked);
        openGateContent.SetActive(unlocked);

        return false;
    }
}