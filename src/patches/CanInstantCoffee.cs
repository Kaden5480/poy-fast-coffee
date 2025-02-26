using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;
using UnityEngine;

namespace FastCoffee.Patches {
    [HarmonyPatch(typeof(CoffeeDrink), "StartCoffeeSip")]
    [HarmonyPatch(MethodType.Enumerator)]
    static class CanInstantCoffee {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> insts
        ) {
            FieldInfo attachedTeleport = AccessTools.Field(
                typeof(RoutingFlag), nameof(RoutingFlag.currentlyAttachedToTeleport)
            );

            FieldInfo attachedTeleportInject = AccessTools.Field(
                typeof(Injects), nameof(Injects.canInstantCoffee)
            );

            bool modified = false;
            int i = 0;
            foreach (CodeInstruction inst in insts) {
                if (modified == false
                    && inst.opcode == OpCodes.Ldsfld
                    && (FieldInfo) inst.operand == attachedTeleport
                ) {
                    Plugin.LogDebug($"CanInstantCoffee: {i} Modifying {Helper.InstToString(inst)}");
                    inst.operand = attachedTeleportInject;
                    Plugin.LogDebug($"CanInstantCoffee: {i} Modified to {Helper.InstToString(inst)}");
                    modified = true;
                }

                yield return inst;
                i++;
            }
        }
    }
}
