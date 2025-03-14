using System.Collections;

using UnityEngine;

namespace FastCoffee {
    /**
     * <summary>
     * A reimplementation of the coffee logic
     * for better control over coffee behaviour.
     * </summary>
     */
    public class Coffee : Loggable {
        // Shorthand for accessing the cache
        private Cache cache {
            get => Plugin.instance.cache;
        }

        // Support accessing statically
        private static Coffee instance = null;

        // Whether fast coffee is running
        public bool isRunning {
            get => coffeeCoroutine != null
                || visualCoroutine != null;
        }

        // Coroutines
        private Coroutine coffeeCoroutine = null;
        private Coroutine visualCoroutine = null;

        // Duration of the coffee effect to apply
        private const float coffeeDuration = 7.96f;

        // Duration of the visual fading in/out
        private const float visualFadeDuration = 1f;

        // Minimum values
        private const float doubleVisionMin = 0f;
        private const float gradientMin = 0f;
        private const float ripplesMin = 0f;
        private const float tiltShiftMin = 0f;
        private const float vignetteMin = 0.362f;

        // Maximum values
        private const float doubleVisionMax = 0.368f;
        private const float gradientMax = 0.129f;
        private const float ripplesMax = 0.33f;
        private const float tiltShiftMax = 0f;
        private const float vignetteMax = 0.482f;

        /**
         * <summary>
         * Constructs an instance of Coffee.
         * </summary>
         */
        public Coffee() {
            instance = this;
        }

        /**
         * <summary>
         * Whether fast coffee can be used.
         * </summary>
         * <param name="force">Whether to use more lenient checks</param>
         * <returns>True if it can be used, false otherwise</returns>
         */
        public bool CanUse(bool force = false) {
            // Disable in yfyd/fs mode
            if (GameManager.control.permaDeathEnabled == true
                || GameManager.control.freesoloEnabled == true
            ) {
                LogDebug("Can't use in yfyd/fs mode");
                return false;
            }

            // Can't use if coffee isn't unlocked
            if (GameManager.control.coffee == false) {
                LogDebug("Can't use, coffee hasn't been unlocked yet");
                return false;
            }

            // Can't use in a menu
            if (InGameMenu.isCurrentlyNavigationMenu == true) {
                LogDebug("Can't use in a navigation menu");
                return false;
            }

            // Validate cache
            if (cache.IsComplete() == false) {
                LogDebug("Can't use, cache is incomplete");
                return false;
            }

            // You need to have coffee available
            if (cache.coffee.coffeeSipsLeft < 1) {
                LogDebug("Can't use, no coffee left");
                return false;
            }

            // Always available in routing flag mode
            if (cache.routingFlag.currentlyUsingFlag == true) {
                LogDebug("Can always use in routing flag mode");
                return true;
            }

            // If forcefully applying, skip other checks
            if (force == true) {
                LogDebug("Forcefully applying coffee, bypassing grounded checks");
                return true;
            }

            // Otherwise, you have to be standing at the base of a peak
            if (cache.playerMove.IsGrounded() == false
                || cache.timeAttack.isInColliderActivationRange == false
            ) {
                LogDebug("Can't use, not grounded in time attack zone");
                return false;
            }

            // In any other case, allowed
            LogDebug("Using fast coffee is allowed");
            return true;
        }

        /**
         * <summary>
         * Applies the coffee effect.
         * </summary>
         */
        private IEnumerator ApplyCoffee() {
            LogDebug("ApplyCoffee: Started");
            cache.coffee.isCoffeeHigh = true;

            LogDebug($"ApplyCoffee: Waiting for {coffeeDuration}s");
            yield return new WaitForSeconds(coffeeDuration);
            LogDebug($"ApplyCoffee: Finished waiting");

            cache.coffee.isCoffeeHigh = false;

            coffeeCoroutine = null;
            LogDebug("ApplyCoffee: Finished");
        }

        /**
         * <summary>
         * Updates the visuals.
         * </summary>
         * <param name="max">Whether to move towards the maximum values</param>
         * <param name="delta">The delta</param>
         */
        private void MoveTowards(bool max, float delta) {
            float doubleVisionValue = (max == true) ? doubleVisionMax : doubleVisionMin;
            float gradientValue = (max == true) ? gradientMax : gradientMin;
            float ripplesValue = (max == true) ? ripplesMax : ripplesMin;
            float tiltShiftValue = (max == true) ? tiltShiftMax : tiltShiftMin;
            float vignetteValue = (max == true) ? vignetteMax : vignetteMin;

            cache.doubleVision.intensity.value = Mathf.MoveTowards(
                cache.doubleVision.intensity.value, doubleVisionValue, delta
            );
            cache.gradient.intensity.value = Mathf.MoveTowards(
                cache.gradient.intensity.value, gradientValue, delta
            );
            cache.ripples.strength.value = Mathf.MoveTowards(
                cache.ripples.strength.value, ripplesValue, delta
            );
            cache.tiltShift.areaSize.value = Mathf.MoveTowards(
                cache.tiltShift.areaSize.value, tiltShiftValue, delta
            );
            cache.vignette.intensity.value = Mathf.MoveTowards(
                cache.vignette.intensity.value, vignetteValue, delta
            );
        }

        /**
         * <summary>
         * Applies the coffee visual.
         * </summary>
         */
        private IEnumerator ApplyVisual() {
            LogDebug("ApplyVisual: Started");

            // Fade the visual in
            LogDebug("ApplyVisual: Fading in visual");

            cache.coffee.volume.enabled = true;
            float time = 0f;
            while (time < visualFadeDuration) {
                float delta = Time.deltaTime;
                MoveTowards(true, delta);
                time += delta;
                yield return null;
            }

            LogDebug("ApplyVisual: Finished fading in");

            // Wait until coffee duration ends
            float waitTime = coffeeDuration - visualFadeDuration;
            LogDebug($"ApplyVisual: Waiting for {waitTime}s");
            yield return new WaitForSeconds(waitTime);
            LogDebug($"ApplyVisual: Finished waiting");

            // Fade the visual out
            LogDebug("ApplyVisual: Fading out visual");

            time = 0f;
            while (time < visualFadeDuration) {
                float delta = Time.deltaTime;
                MoveTowards(false, delta);
                time += delta;
                yield return null;
            }
            cache.coffee.volume.enabled = false;

            visualCoroutine = null;
            LogDebug("ApplyVisual: Finished");
        }

        /**
         * <summary>
         * Resets coffee related fields and also stops
         * coroutines running in CoffeeDrink.
         * </summary>
         */
        private void ResetCoffeeState() {
            if (cache.IsComplete() == false) {
                LogDebug("Can't reset state, the cache is incomplete");
                return;
            }

            // Stop everything from running
            cache.coffee.StopAllCoroutines();

            // Remove the coffee effect
            cache.coffee.isCoffeeHigh = false;

            // Overwrite fields
            cache.coffee.armMeshL.enabled = false;
            cache.coffee.armMeshR.enabled = false;
            cache.coffee.drinkingCoffeL = false;
            cache.coffee.drinkingCoffeR = false;
            cache.inventory.pickedItemUsingLeft = false;
            cache.inventory.pickedItemUsingRight = false;
            cache.coffee.holdingCoffeeButtonDown = false;

            // Stop the animation
            cache.coffee.coffeeDrinkAnimation.Stop();
            cache.coffeeBottle.SetActive(false);

            GameObject animationObj = cache.coffee.coffeeDrinkAnimation.gameObject;
            animationObj.transform.localPosition = cache.coffeeDrinkPosition;
            animationObj.transform.localRotation = cache.coffeeDrinkRotation;

            // Clear the UI
            cache.coffeeUsesLeftCG.alpha = 0f;
            CoffeeDrink.runningCoffeeTooltip = false;

            // Clear the visual immediately
            cache.doubleVision.intensity.value = doubleVisionMin;
            cache.gradient.intensity.value = gradientMin;
            cache.ripples.strength.value = ripplesMin;
            cache.tiltShift.areaSize.value = tiltShiftMin;
            cache.vignette.intensity.value = vignetteMin;
            cache.coffee.volume.enabled = false;

            LogDebug("Reset coffee state");
        }

        /**
         * <summary>
         * Attempts to trigger fast coffee.
         * </summary>
         * <param name="force">Whether to more forcefully apply coffee</param>
         * <returns>True if coffee was reapplied, false otherwise</returns>
         */
        private bool Start(bool force = false) {
            if (CanUse(force) == false) {
                return false;
            }

            // Make sound effects
            cache.coffee.slurpSound.SingleSound();
            cache.coffeeEffectAudio.Play();

            // Reset the state of coffee
            ResetCoffeeState();

            // Update coffee counts and display UI where appropriate
            if (GameManager.control.allArtefactsUnlocked == false) {
                int oldSips = cache.coffee.coffeeSipsLeft;

                cache.coffee.coffeeSipsLeft--;
                cache.coffee.StartCoroutine("CoffeeLeftTooltip");

                LogDebug(
                    "Infinite coffee not unlocked, reduced sips left"
                    + $" {oldSips} -> {cache.coffee.coffeeSipsLeft}"
                );
            }

            // Update global stats
            int oldUses = GameManager.control.global_stats_coffeeuses;
            GameManager.control.global_stats_coffeeuses++;

            LogDebug(
                $"Updated global stats {oldUses} ->"
                + $" {GameManager.control.global_stats_coffeeuses}"
            );

            // Start the coroutines
            coffeeCoroutine = Plugin.instance
                .StartCoroutine(ApplyCoffee());
            LogDebug("Started coffee coroutine");

            visualCoroutine = Plugin.instance
                .StartCoroutine(ApplyVisual());
            LogDebug("Started visual coroutine");

            return true;
        }

        /**
         * <summary>
         * Forcefully stops fast coffee from running.
         * </summary>
         */
        public void Stop() {
            // Don't need to check if fast coffee can be used
            // this method is meant to be forceful

            // Stop the audio
            if (cache.coffeeEffectAudio != null) {
                cache.coffeeEffectAudio.Stop();
            }

            // Stop any coroutines
            if (coffeeCoroutine != null) {
                Plugin.instance.StopCoroutine(coffeeCoroutine);
                coffeeCoroutine = null;
                LogDebug("Stopped coffee coroutine");
            }

            if (visualCoroutine != null) {
                Plugin.instance.StopCoroutine(visualCoroutine);
                visualCoroutine = null;
                LogDebug("Stopped visual coroutine");
            }

            // Reset states after stopping coroutines
            ResetCoffeeState();
            LogDebug("Stopped fast coffee");
        }

        /**
         * <summary>
         * Toggles fast coffee on/off.
         * </summary>
         */
        public void Toggle() {
            if (isRunning == true) {
                Stop();
            }
            else {
                Start();
            }
        }

        /**
         * <summary>
         * Reapplies coffee.
         * This is used for integration with Fast Reset.
         * </summary>
         */
        public static void Reapply() {
            if (instance == null) {
                Plugin.LogDebug("[FastCoffee.Coffee]: No instance found, not reapplying coffee");
                return;
            }

            instance.Stop();

            // Need to apply more forcefully to bypass grounded checks
            if (instance.Start(true) == true) {
                instance.LogDebug("Reapplied coffee");
            }
            else {
                instance.LogDebug("Failed to reapply coffee");
            }
        }
    }
}
