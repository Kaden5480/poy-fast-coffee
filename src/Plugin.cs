using System;

using BepInEx;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;

namespace FastCoffee {
    [BepInPlugin("com.github.Kaden5480.poy-fast-coffee", "FastCoffee", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        // An instance of Plugin accessible statically
        public static Plugin instance = null;

        // The cache
        public Cache cache { get; } = new Cache();

        // The rewritten coffee logic
        public Coffee coffee { get; } = new Coffee();

        // Applies harmony patches
        private Patcher patcher { get; } = new Patcher();

        /**
         * <summary>
         * Executes when the plugin is being loaded.
         * </summary>
         */
        private void Awake() {
            instance = this;

            // Apply early patches
            patcher.PatchEarly();

            // Track scene state changes
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes when this plugin is being destroyed.
         * </summary>
         */
        private void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        /**
         * <summary>
         * Executes each frame.
         * </summary>
         */
        private void Update() {
            if (cache.player == null) {
                return;
            }

            if (cache.player.GetButtonDown("Coffee") == true) {
                coffee.Toggle();
            }
        }

        /**
         * <summary>
         * Executes whenever a scene loads.
         * </summary>
         */
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            cache.FindObjects();
        }

        /**
         * <summary>
         * Executes whenever a scene unloads.
         * </summary>
         */
        private void OnSceneUnloaded(Scene scene) {
            coffee.Stop();
            cache.Clear();
        }

        /**
         * <summary>
         * Logs a debug message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogDebug(string message) {
#if DEBUG
            if (instance == null) {
                Console.WriteLine($"[Debug] FastCoffee: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
#else
            if (instance != null) {
                instance.Logger.LogDebug(message);
            }
#endif
        }

        /**
         * <summary>
         * Logs an informational message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogInfo(string message) {
            if (instance == null) {
                Console.WriteLine($"[Info] FastCoffee: {message}");
                return;
            }

            instance.Logger.LogInfo(message);
        }

        /**
         * <summary>
         * Logs an error message.
         * </summary>
         * <param name="message">The message to log</param>
         */
        public static void LogError(string message) {
            if (instance == null) {
                Console.WriteLine($"[Error] FastCoffee: {message}");
                return;
            }

            instance.Logger.LogError(message);
        }
    }
}
