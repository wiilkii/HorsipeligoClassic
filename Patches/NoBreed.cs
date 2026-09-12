using HarmonyLib;
using Horsipelago.Archipelago;
using Horsipelago;
using HorseRidingClassic.Source.HorseBreeding;
using HorseRidingClassic.Source.GameState;
using HorseRidingClassic.Source.SaveSystem;



namespace Horsipelago.Patches;

[HarmonyPatch(typeof(GameManager), "LoadBreeding")]
public static class NoBreedPatch
{
    public static bool IsBreedingAllowed = false;

    static bool Prefix()
    {
        if (!IsBreedingAllowed)
        {
            Plugin.BepinLogger.LogInfo("blocked breed");
            return false; // disables the horse breeding menu from being opened, effectively disabling horse breeding
        }
    return true;
    }
}

[HarmonyPatch(typeof(GameManager), "LoadGame")]
public static class NoBreedOnLoadGamePatch
{
    static void Prefix(GameManager __instance)
    {
        var newHorseOwedField = Traverse.Create(__instance).Field("newHorseOwed").GetValue<BoolSaveEntry>();
        if (newHorseOwedField.Value)
        {
            Plugin.BepinLogger.LogInfo("block load/death breed");
            newHorseOwedField.Value = false;
        }
    }
}