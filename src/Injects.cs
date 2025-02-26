namespace FastCoffee {
    public static class Injects {
        public const float defaultCoffeeDuration = 7f;
        public const float defaultHideArmsDuration = 2.3f;

        public static bool canInstantCoffee = RoutingFlag.currentlyAttachedToTeleport;
        public static float coffeeDuration = defaultCoffeeDuration;
        public static float hideArmsDuration = defaultHideArmsDuration;

        public static void Enable() {
            Plugin.LogDebug("Injects enabled");

            canInstantCoffee = true;
            coffeeDuration = 5.96f;
            hideArmsDuration = 0f;
        }

        public static void Disable() {
            Plugin.LogDebug("Injects disabled");

            canInstantCoffee = RoutingFlag.currentlyAttachedToTeleport;
            coffeeDuration = defaultCoffeeDuration;
            hideArmsDuration = defaultHideArmsDuration;
        }
    }
}
