using HarmonyLib;
using HorseRidingClassic.Source.GameState;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace Horsipelago.Patches;

[HarmonyPatch(typeof(SceneManager), "LoadQueuedProfile")]
public static class SetScenePatch
{
    const string TargetScene = "Player";     // scene the game is trying to enter
    const string RedirectScene = "Breed"; // scene you want instead

    static void Prefix(ref SceneProfile ___queuedProfile)
    {
        var gm = GameManager.Instance;
        var tr = Traverse.Create(gm);
        Console.WriteLine($"breedingScene = {tr.Field("breedingScene").GetValue<string>()}");
        Console.WriteLine($"playerScene   = {tr.Field("playerScene").GetValue<string>()}");
        Console.WriteLine($"applesScene   = {tr.Field("applesScene").GetValue<string>()}");
        Console.WriteLine($"areaScenes    = {string.Join(", ", tr.Field("areaScenes").GetValue<string[]>())}");
        string currentActive = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        Console.WriteLine($"Current active scene: {currentActive}");
        if (___queuedProfile == null)
            return;

        string active = ___queuedProfile.activeScene;
        string[] loaded = ___queuedProfile.alwaysLoaded ?? Array.Empty<string>();

        // Temporary: log scene names so you can fill in the constants above
        Console.WriteLine($"Queued profile: active={active}, loaded=[{string.Join(", ", loaded)}]");

        if (active != TargetScene && !loaded.Contains(TargetScene))
            return;

        try
        {
            string filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "AppData", "LocalLow", "Juice Cube", "Horse Riding Classic", "Archipelago.ride");

            if (!File.Exists(filePath))
                return;

            JObject data = JObject.Parse(File.ReadAllText(filePath));
            if ((data.Value<int?>("archipelagoBreedsOwed") ?? 0) == 0)
                return;

            // Swap the target scene for the redirect scene, keep everything else
            string newActive = active == TargetScene ? RedirectScene : active;
            string[] newLoaded = loaded.Select(s => s == TargetScene ? RedirectScene : s).ToArray();

            ___queuedProfile = new SceneProfile(newActive, newLoaded);
        }
        catch (Exception e)
        {
            Console.WriteLine($"SetScenePatch failed: {e}");
        }
    }
}