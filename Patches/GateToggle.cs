using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Apples;
using Horsipelago;
using UnityEngine;


namespace Horsipelago.Patches;

[HarmonyPatch(typeof(AppleGate), "OnCollectedApplesChanged")]
public static class GateTogglePatch
{
    static bool Prefix(AppleGate __instance)
    {
        Plugin.BepinLogger.LogInfo("GateToggle called!");

        var closedGateContent = Traverse.Create(__instance).Field("closedGateContent").GetValue<GameObject>();
        var openGateContent = Traverse.Create(__instance).Field("openGateContent").GetValue<GameObject>();

        bool unlocked = true;

        // sets gate toggles permanently, will be tied to a key item within archipelago later
        closedGateContent.SetActive(!unlocked);
        openGateContent.SetActive(unlocked);

        return false;
    }
}