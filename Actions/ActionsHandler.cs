using System.Collections.Generic;
using System.Linq;
using BepInEx;
using Horsipelago.Archipelago;
using UnityEngine;
using HorseRidingClassic.Source.GameState;
using HorseRidingClassic.Source.SaveSystem;
using HarmonyLib;
using Horsipelago.Patches;
using HorseRidingClassic.Source.HorseBreeding;
using HorseRidingClassic.Source.Player;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

namespace Horsipelago.Actions;

public class ActionsHandler
{
    private static ActionsHandler _instance;
    public static ActionsHandler Instance => _instance ??= new ActionsHandler();
    
    private Dictionary<long, bool> keys = new()
    {
        { 2, false },
        { 3, false },
        { 4, false },
        { 5, false }
    };

    public bool TriggerBreed()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Plugin.BepinLogger.LogWarning("GameManager.Instance not set yet");
            return false;
        }

        string dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "AppData", "LocalLow", "Juice Cube", "Horse Riding Classic");

        string filePath = Path.Combine(dir, "Archipelago.ride");

        JObject savedata = JObject.Parse(File.ReadAllText(filePath));

        int owed = (int?)savedata["archipelagoBreedsOwed"] ?? 0;
        int used = (int?)savedata["archipelagoBreedsUsed"] ?? 0;

        savedata["archipelagoBreedsOwed"] = Math.Max(owed - 1, 0);
        savedata["archipelagoBreedsUsed"] = used + 1;
        File.WriteAllText(filePath, savedata.ToString(Formatting.Indented));

        NoBreedPatch.IsBreedingAllowed = true;
        var newHorseOwedField = Traverse.Create(gameManager).Field("newHorseOwed").GetValue<BoolSaveEntry>();
        newHorseOwedField.Value = true;
        if (owed <= 1)
        {
            gameManager.LoadBreeding();
        }
        
        NoBreedPatch.IsBreedingAllowed = false; // reset the flag after triggering the breed

        return true;
    }

    public void SetHorseSpeed(float speed)
    {
        var movement = UnityEngine.Object.FindFirstObjectByType<Movement>();
        if (movement == null)
        {
            Plugin.BepinLogger.LogWarning("Movement instance not found");
            return;
        }
        var currentHorseTraverse = Traverse.Create(movement).Field("currentHorse");
        HorseDataSaveEntry saveEntry = currentHorseTraverse.GetValue<HorseDataSaveEntry>();

        HorseData horseData = saveEntry.Value;       
        horseData.horsePower = speed; //whoa             
        saveEntry.Value = horseData;                  
    }

    public bool UnlockGate(string gateName)
    {
        var gates = UnityEngine.Object.FindObjectsByType<AppleGate>(FindObjectsSortMode.None);
        var gate = gates.FirstOrDefault(g => g.gameObject.name == gateName);

        if (gate == null)
        {
            Plugin.BepinLogger.LogWarning($"Gate '{gateName}' not found");
            return false;
        }

        GateTogglePatch.UnlockedGates.Add(gateName);

        var closedGateContent = Traverse.Create(gate).Field("closedGateContent").GetValue<GameObject>();
        var openGateContent = Traverse.Create(gate).Field("openGateContent").GetValue<GameObject>();

        closedGateContent.SetActive(false);
        openGateContent.SetActive(true);

        Plugin.BepinLogger.LogInfo($"Unlocked gate: {gateName}");
        return true;
    }

    private static readonly string[] GateOrder = 
    {
        "Western Gate",
        "Farm Gate",
        "Modern Gate from Western", // or "Modern Gate from Farm", whichever comes first in your intended progression
        "Modern Gate from Farm",
        "Glue Factory Gate"
    };

    private int _nextGateIndex = 0;

    public bool UnlockNextGate()
    {
        if (_nextGateIndex >= GateOrder.Length)
        {
            Plugin.BepinLogger.LogInfo("All gates already unlocked");
            return false;
        }

        string gateName = GateOrder[_nextGateIndex];
        bool success = UnlockGate(gateName);
        if (success)
        {
            _nextGateIndex++;
        }
        return success;
    }

    public void UnlockSpecificGate(long id)
    {
        switch (id)
        {
            case 2:
                UnlockGate("Farm Gate");
                keys[2] = true;
                if (keys[4])
                {
                    UnlockGate("Modern Gate from Farm");
                }
                break;
            case 3:
                UnlockGate("Western Gate");
                keys[3] = true;
                if (keys[4])
                {
                    UnlockGate("Modern Gate from Western");
                }
                break; 
            case 4:
                if (keys[2])
                {
                    UnlockGate("Modern Gate from Farm");
                }
                if (keys[3])
                {
                    UnlockGate("Modern Gate from Western");
                }
                keys[4] = true;
                break;
            case 5:
                UnlockGate("Glue Factory Gate");
                keys[5] = true;
                break;
            
        }
    }
}
