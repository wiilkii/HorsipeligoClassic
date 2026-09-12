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

namespace Horsipelago.Actions;

public class ActionsHandler
{
    private static ActionsHandler _instance;
    public static ActionsHandler Instance => _instance ??= new ActionsHandler();
    
    public void TriggerBreed()
    {
        var gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Plugin.BepinLogger.LogWarning("GameManager.Instance not set yet");
            return;
        }
        NoBreedPatch.IsBreedingAllowed = true;
        var newHorseOwedField = Traverse.Create(gameManager).Field("newHorseOwed").GetValue<BoolSaveEntry>();
        newHorseOwedField.Value = true;
        gameManager.LoadBreeding(); // public method, no need for Traverse here
        NoBreedPatch.IsBreedingAllowed = false; // reset the flag after triggering the breed
    }

    public void SetHorseSpeed(float speed)
    {
        var movement = Object.FindFirstObjectByType<Movement>();
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
        var gates = Object.FindObjectsByType<AppleGate>(FindObjectsSortMode.None);
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
}
