using HarmonyLib;
using Horsipelago.Archipelago;
using Horsipelago;
using HorseRidingClassic.Source.HorseBreeding;
using HorseRidingClassic.Source.GameState;
using HorseRidingClassic.Source.SaveSystem;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;



namespace Horsipelago.Patches;

[HarmonyPatch(typeof(GameManager), "LoadBreeding")]
public static class NoBreedPatch
{
    public static bool IsBreedingAllowed = false;



    static bool Prefix()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData", "LocalLow", "Juice Cube", "Horse Riding Classic");

        string filePath = Path.Combine(dir, "Archipelago.ride");

        JObject savedata = JObject.Parse(File.ReadAllText(filePath));

        

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

        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData", "LocalLow", "Juice Cube", "Horse Riding Classic");

        string filePath = Path.Combine(dir, "Archipelago.ride");

        JObject savedata = JObject.Parse(File.ReadAllText(filePath));

        var newHorseOwedField = Traverse.Create(__instance).Field("newHorseOwed").GetValue<BoolSaveEntry>();

        int owed = (int)savedata["archipelagoBreedsOwed"];

        if (owed > 0)
        {
            newHorseOwedField.Value = true;

            savedata["archipelagoBreedsOwed"] = owed - 1;
            savedata["archipelagoBreedsUsed"] = (int)savedata["archipelagoBreedsUsed"] + 1;
            File.WriteAllText(filePath, savedata.ToString(Formatting.Indented));

            return;
        }

        
        if (newHorseOwedField.Value)
        {
            Plugin.BepinLogger.LogInfo("block load/death breed");
            newHorseOwedField.Value = false;
        }
    }
}