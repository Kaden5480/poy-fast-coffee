using System;

using HarmonyLib;
using UnityEngine;

namespace FastCoffee.Patches {
    [HarmonyPatch(typeof(CoffeeDrink), "SipCoffee")]
    static class CoffeeEntry {
        private static AudioSource coffeeEffect = null;
        private static PlayerMove playerMove = null;
        private static RoutingFlag routingFlag = null;
        private static TimeAttack timeAttack = null;

        private static bool InjectsEnabled() {
            if (playerMove == null
                || routingFlag == null
                || timeAttack == null
            ) {
                return false;
            }

            if (GameManager.control.permaDeathEnabled == true
                || GameManager.control.freesoloEnabled == true
            ) {
                return false;
            }

            if (routingFlag.currentlyUsingFlag == true) {
                return true;
            }

            if (playerMove.IsGrounded() == true
                && timeAttack.isInColliderActivationRange == true
            ) {
                return true;
            }

            return false;
        }

        static void Prefix() {
            if (InjectsEnabled() == true) {
                // Use injected values
                Injects.Enable();
                coffeeEffect.Play();
            }
            else {
                // Use defaults
                Injects.Disable();
            }
        }

        static void Postfix(
            CoffeeDrink __instance,
            Inventory ___inventory
        ) {
            if (InjectsEnabled() == true) {
                __instance.armMeshL.enabled = false;
                __instance.armMeshR.enabled = false;
                __instance.drinkingCoffeL = false;
                __instance.drinkingCoffeR = false;

                ___inventory.pickedItemUsingLeft = false;
                ___inventory.pickedItemUsingRight = false;
            }
        }

        public static void OnSceneLoaded() {
            GameObject coffeeEffectObj = GameObject.Find("CoffeeSound_Coffee_Effect");
            coffeeEffect = coffeeEffectObj.GetComponent<AudioSource>();

            playerMove = GameObject.FindObjectOfType<PlayerMove>();
            routingFlag = GameObject.FindObjectOfType<RoutingFlag>();
            timeAttack = GameObject.FindObjectOfType<TimeAttack>();
        }

        public static void OnSceneUnloaded() {
            coffeeEffect = null;
            playerMove = null;
            routingFlag = null;
            timeAttack = null;
        }
    }
}
