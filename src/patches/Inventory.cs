using HarmonyLib;

namespace FastCoffee.Patches {
    /**
     * <summary>
     * Prevents the use of the normal coffee methods
     * when fast coffee is active or available.
     * </summary>
     */
    [HarmonyPatch(typeof(Inventory), "DrinkCoffee")]
    static class BypassNormal {
        static bool Prefix() {
            if (Plugin.instance.coffee.CanUse() == true) {
                Plugin.LogDebug("[BypassNormal] Bypassing Inventory.DrinkCoffee, fast coffee is available to use");
                return false;
            }

            if (Plugin.instance.coffee.isRunning == true) {
                Plugin.LogDebug("[BypassNormal] Bypassing Inventory.DrinkCoffee, fast coffee is running");
                return false;
            }

            Plugin.LogDebug("[BypassNormal] Not bypassing Inventory.DrinkCoffee");
            return true;
        }
    }
}
