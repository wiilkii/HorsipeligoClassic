// totally AI-genned tbh, adding my own changes to this base structure

using UnityEngine;
using HarmonyLib;
using HorseRidingClassic.Source.GameState;
using HorseRidingClassic.Source.SaveSystem;
using Horsipelago.Patches;
using Horsipelago.Actions;

namespace Horsipelago.Utils
{
    public class DebugMenu : MonoBehaviour
    {
        private bool menuOpen = true; // can hide menu later if needed
        private Rect windowRect = new Rect(Screen.width - 320, 200, 300, 250);

        ActionsHandler actionsHandler = ActionsHandler.Instance;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) // pick whatever key you like
            {
                menuOpen = !menuOpen;
            }
        }

        void OnGUI()
        {
            if (!menuOpen) return;
            windowRect = GUI.Window(0, windowRect, DrawWindow, "Horsipelago Debug");
        }

        void DrawWindow(int id)
        {
            GUILayout.Label("Breeding");
            if (GUILayout.Button("Trigger Horse Breed Now"))
            {
                TriggerBreed();
            }

            GUILayout.Space(10);
            GUILayout.Label("Speed");
            if (GUILayout.Button("Set Horse Speed to 250"))
            {
                SetHorseSpeed(250f);
            }

            GUILayout.Space(10);
            GUILayout.Label("Gates");
            if (GUILayout.Button("Log All Gates"))
            {
                LogAllGates();
            }

            GUILayout.Space(10);
            GUILayout.Label("Unlock Gates");
            if (GUILayout.Button("Unlock Next Gate"))
            {
                actionsHandler.UnlockNextGate();
            }

            GUI.DragWindow();
        }

        public void TriggerBreed()
        {
            
            actionsHandler.TriggerBreed();
        }

        void SetHorseSpeed(float speed)
        {
            actionsHandler.SetHorseSpeed(speed);
        }
        
        public static void LogAllGates()
        {
            var gates = Object.FindObjectsByType<AppleGate>(FindObjectsSortMode.None);
            foreach (var gate in gates)
            {
                var requiredApples = Traverse.Create(gate).Field("requiredApples").GetValue<int>();
                Plugin.BepinLogger.LogInfo($"Gate '{gate.gameObject.name}' requires {requiredApples} apples");
            }
        }


    }
}