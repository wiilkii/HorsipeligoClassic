using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Player;
using Horsipelago;
using UnityEngine;
using HorseRidingClassic.Source.Apples;
using HorseRidingClassic.Source.SaveSystem;
using HorseRidingClassic.Source.HorseBreeding;


namespace Horsipelago.Patches;

// find a better patch method, 

[HarmonyPatch(typeof(Movement), "Update")]
public static class HorseDataPatch
{

    // make this a full controller?, like be able to trigger this upon other events and maybe just build a universal horse data patcher idk copilot autocomplete SHUT UP PELASE IM TRYING TO TYPE

    static void Postfix(Movement __instance)
    {
    var currentHorseTraverse = Traverse.Create(__instance).Field("currentHorse");
    HorseDataSaveEntry saveEntry = currentHorseTraverse.GetValue<HorseDataSaveEntry>();

    HorseData horseData = saveEntry.Value;       
    horseData.horsePower = 100f; // omg call this a glue trap :sob:                  
    saveEntry.Value = horseData;                  
    // so we get a copy of the save data, do shit do it, and reset it back into the save data. cool!

    // Plugin.BepinLogger.LogInfo(saveEntry.Value.horsePower);
    }
}

// not going to use for now, but may be good
// [HarmonyPatch(typeof(HorseDataUtility), "BreedFunction")]
// public static class HorseDataPatch
// {
//     static void Postfix(ref HorseData __result)
//     {
//         __result.horsePower = 10f;
//         Plugin.BepinLogger.LogInfo(__result.horsePower);
//     }
// }