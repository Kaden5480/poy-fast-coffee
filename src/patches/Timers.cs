using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

using HarmonyLib;

namespace FastCoffee.Patches {
    [HarmonyPatch(typeof(CoffeeDrink), "StartCoffeeSip")]
    [HarmonyPatch(MethodType.Enumerator)]
    static class CoffeeDuration {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> enumInsts
        ) {
            List<CodeInstruction> insts = enumInsts.ToList();
            FieldInfo durationInfo = AccessTools.Field(
                typeof(Injects), nameof(Injects.coffeeDuration)
            );

            int index = Helper.FindSeq(insts,
                new[] {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld, null),
                    new CodeInstruction(OpCodes.Ldc_R4, Injects.defaultCoffeeDuration),
                    new CodeInstruction(OpCodes.Blt, null),
                }
            );

            if (index == -1) {
                Plugin.LogDebug("Failed patching coffee duration");
                yield return null;
            }

            Plugin.LogDebug($"CoffeeDuration: Modifying {Helper.InstToString(insts[index + 2])}");
            insts[index + 2].opcode = OpCodes.Ldsfld;
            insts[index + 2].operand = durationInfo;
            Plugin.LogDebug($"CoffeeDuration: Modified to {Helper.InstToString(insts[index + 2])}");

            foreach (CodeInstruction inst in insts) {
                yield return inst;
            }
        }
    }

    [HarmonyPatch(typeof(CoffeeDrink), "HideArms")]
    [HarmonyPatch(MethodType.Enumerator)]
    static class HideArmsDuration {
        static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> enumInsts
        ) {
            List<CodeInstruction> insts = enumInsts.ToList();
            FieldInfo durationInfo = AccessTools.Field(
                typeof(Injects), nameof(Injects.hideArmsDuration)
            );

            int index = Helper.FindSeq(insts,
                new[] {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldc_R4, Injects.defaultHideArmsDuration),
                    new CodeInstruction(OpCodes.Newobj, null),
                    new CodeInstruction(OpCodes.Stfld, null),
                }
            );

            if (index == -1) {
                Plugin.LogDebug("Failed patching hide arms duration");
                yield return null;
            }

            Plugin.LogDebug($"HideArmsDuration: Modifying {Helper.InstToString(insts[index + 1])}");
            insts[index + 1].opcode = OpCodes.Ldsfld;
            insts[index + 1].operand = durationInfo;
            Plugin.LogDebug($"HideArmsDuration: Modified to {Helper.InstToString(insts[index + 1])}");

            foreach (CodeInstruction inst in insts) {
                yield return inst;
            }
        }
    }
}
