using HarmonyLib;
using Horsipelago.Archipelago;
using HorseRidingClassic.Source.Apples;
using Horsipelago;
using Horsipelago.Models;
using HorseRidingClassic.Source.MainMenu;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;



namespace Horsipelago.Patches;

[HarmonyPatch(typeof(MainMenu), "ResetSave")]
public class ResetPatch
{
    static void Postfix()
    {
        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData", "LocalLow", "Juice Cube", "Horse Riding Classic");

        string filePath = Path.Combine(dir, "Archipelago.ride");

        File.WriteAllText(filePath, """
        {
            "archipelagoBreedsOwed": 0,
            "archipelagoBreedsUsed": 0
        }
        """);
    }
}