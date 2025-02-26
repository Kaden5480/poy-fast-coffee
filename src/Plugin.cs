using System;

using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX
using BepInEx;
using BepInEx.Configuration;

namespace FastCoffee {
    [BepInPlugin("com.github.Kaden5480.poy-fast-coffee", "FastCoffee", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        public void Awake() {
            Harmony.CreateAndPatchAll(typeof(Patches.CoffeeEntry));
            Harmony.CreateAndPatchAll(typeof(Patches.CanInstantCoffee));
            Harmony.CreateAndPatchAll(typeof(Patches.CoffeeDuration));
            Harmony.CreateAndPatchAll(typeof(Patches.HideArmsDuration));

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            CommonAwake();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            CommonSceneLoad();
        }

        private void OnSceneUnloaded(Scene scene) {
            CommonSceneUnload();
        }

#elif MELONLOADER
using MelonLoader;
using MelonLoader.Utils;

[assembly: MelonInfo(typeof(FastCoffee.Plugin), "FastCoffee", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace FastCoffee {
    public class Plugin: MelonMod {
        public override void OnInitializeMelon() {
            CommonAwake();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
            CommonSceneLoad();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) {
            CommonSceneUnload();
        }

#endif

        public static Plugin plugin;

        public static void LogDebug(string message) {
#if DEBUG
            if (plugin == null) {
                Console.WriteLine($"FastCoffee: {message}");
                return;
            }
    #if BEPINEX
            plugin.Logger.LogDebug(message);
    #elif MELONLOADER
            plugin.MelonLogger.Msg(message);
    #endif
#endif
        }

        private void CommonAwake() {
            plugin = this;
        }

        private void CommonSceneLoad() {
            Patches.CoffeeEntry.OnSceneLoaded();
        }

        private void CommonSceneUnload() {
            Patches.CoffeeEntry.OnSceneUnloaded();
        }
    }
}
