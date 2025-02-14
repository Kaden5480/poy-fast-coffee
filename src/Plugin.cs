using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

#if BEPINEX
using BepInEx;
using BepInEx.Configuration;

namespace FastCoffee {
    [BepInPlugin("com.github.Kaden5480.poy-fast-coffee", "FastCoffee", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        public void Awake() {
            // Injections
            Harmony.CreateAndPatchAll(typeof(PatchCoffeeInject));
            Harmony.CreateAndPatchAll(typeof(PatchHideArmsInject));

            // Updating injections
            Harmony.CreateAndPatchAll(typeof(PatchCoffeeSip));

            CommonAwake();
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

#endif

        public const float speedMult = 8f;

        public static Plugin plugin;

        private void CommonAwake() {
            plugin = this;
        }
    }

    [HarmonyPatch(typeof(CoffeeDrink), "SipCoffee")]
    static class PatchCoffeeSip {
        private const float defaultCoffeeSpeed = 1f;
        public const float defaultWait = 1.25f;
        public const float defaultHideArmsWait = 2.3f;

        static float wait;
        static float hideArmsWait;

        public static FieldInfo waitInfo = AccessTools.Field(
            typeof(PatchCoffeeSip), nameof(wait)
        );
        public static FieldInfo hideArmsWaitInfo = AccessTools.Field(
            typeof(PatchCoffeeSip), nameof(hideArmsWait)
        );
        private static bool CanSpeedUp(Climbing climbing) {

            TimeAttack timeAttack = GameObject.FindObjectOfType<TimeAttack>();

            if (timeAttack == null) {
                return false;
            }

            if (GameManager.control.permaDeathEnabled
                || GameManager.control.freesoloEnabled
            ) {
                return false;
            }

            if (climbing.fpController.IsGrounded() == false
                || timeAttack.isInColliderActivationRange == false
            ) {
                return false;
            }

            return true;
        }

        static void Prefix(CoffeeDrink __instance, Climbing ___climbing) {
            __instance.coffeeDrinkAnimationSpeed = defaultCoffeeSpeed;
            wait = defaultWait;
            hideArmsWait = defaultHideArmsWait;

            if (CanSpeedUp(___climbing)) {
                __instance.coffeeDrinkAnimationSpeed = 2.2f;
                wait = 0f;
                hideArmsWait = 1.05f;
            }
        }
    }

    [HarmonyPatch(typeof(CoffeeDrink), "HideArms")]
    [HarmonyPatch(MethodType.Enumerator)]
    static class PatchHideArmsInject {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> insts
        ) {
            bool done = false;

            foreach (CodeInstruction inst in insts) {
                if (done == false
                    && inst.LoadsConstant(PatchCoffeeSip.defaultHideArmsWait) == true
                ) {
                    inst.opcode = OpCodes.Ldsfld;
                    inst.operand = PatchCoffeeSip.hideArmsWaitInfo;

                    done = true;
                }

                yield return inst;
            }
        }
    }

    [HarmonyPatch(typeof(CoffeeDrink), "StartCoffeeSip")]
    [HarmonyPatch(MethodType.Enumerator)]
    static class PatchCoffeeInject {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> insts
        ) {
            bool done = false;

            foreach (CodeInstruction inst in insts) {
                if (done == false
                    && inst.LoadsConstant(PatchCoffeeSip.defaultWait) == true
                ) {
                    inst.opcode = OpCodes.Ldsfld;
                    inst.operand = PatchCoffeeSip.waitInfo;

                    done = true;
                }

                yield return inst;
            }
        }
    }
}
