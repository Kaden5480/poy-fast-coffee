using System;
using System.Reflection.Emit;
using System.Collections.Generic;

using HarmonyLib;

namespace FastCoffee {
    public class Helper {
        /**
         * <summary>
         * Checks whether two ldloc.s instructions are equivalent.
         * </summary>
         * <param name="a">The first instruction</param>
         * <param name="b">The second instruction</param>
         */
        private static bool LdlocEqual(CodeInstruction a, CodeInstruction b) {
            Type typeA = a.operand.GetType();
            Type typeB = b.operand.GetType();

            if (typeA == typeB && typeA == typeof(LocalBuilder)) {
                return a.operand.Equals(b.operand);
            }

            if (typeA == typeof(LocalBuilder)) {
                return ((LocalBuilder) a.operand).LocalIndex == (int) b.operand;
            }

            if (typeB == typeof(LocalBuilder)) {
                return ((LocalBuilder) b.operand).LocalIndex == (int) a.operand;
            }

            return false;
        }

        /**
         * <summary>
         * Compare two instructions for equivalence.
         * </summary>
         * <param name="a">The first instruction to compare</param>
         * <param name="b">The second instruction to compare</param>
         */
        public static bool InstsEqual(CodeInstruction a, CodeInstruction b) {
            // If either are null, always match
            if (a == null || b == null) {
                return true;
            }

            // Check opcodes
            if (a.opcode != b.opcode) {
                return false;
            }

            // Check null operands
            if (a.operand == null || b.operand == null) {
                return true;
            }

            // Specific check for Ldloc_S
            if (a.opcode == OpCodes.Ldloc_S) {
                return LdlocEqual(a, b);
            }

            // Check operand equivalence
            return a.operand.Equals(b.operand);
        }

        /**
         * <summary>
         * Find the locations of sequences of instructions.
         * </summary>
         * <param name="insts">The instructions to search within</param>
         * <param name="pattern">The sequence to search for</param>
         * <return>The indices the instructions start at</return>
         */
        public static IEnumerable<int> FindSeqs(
            List<CodeInstruction> insts,
            CodeInstruction[] pattern
        ) {
            int beginning = 0;
            int patternIndex = 0;

            for (int i = 0; i < insts.Count; i++) {
                // If fully matched, return beginning of the sequence
                if (patternIndex >= pattern.Length) {
                    yield return beginning;

                    // Also reset
                    beginning = i;
                    patternIndex = 0;
                }

                // Check if this instruction matches the pattern
                if (InstsEqual(pattern[patternIndex], insts[i]) == false) {
                    // Reset values
                    beginning = i + 1;
                    patternIndex = 0;
                }
                else {
                    // Increase pattern index
                    patternIndex++;
                }
            }
        }

        /**
         * <summary>
         * Find the first location of a sequence of instructions.
         * </summary>
         * <param name="insts">The instructions to search within</param>
         * <param name="pattern">The sequence to search for</param>
         * <return>The index the instructions start at, -1 if not found</return>
         */
        public static int FindSeq(
            List<CodeInstruction> instructions,
            CodeInstruction[] pattern
        ) {
            foreach (int index in FindSeqs(instructions, pattern)) {
                return index;
            }

            return -1;
        }

        /**
         * <summary>
         * Convert an instruction to a string.
         * </summary>
         * <param name="inst">The instruction to convert</param>
         * <return>The instruction as a string</return>
         */
        public static string InstToString(CodeInstruction inst) {
            if (inst == null || inst.opcode == null) {
                return "Inst was null";
            }

            if (inst.operand != null) {
                return $"{inst.opcode}, {inst.operand}";
            }

            return $"{inst.opcode}";
        }
    }
}
