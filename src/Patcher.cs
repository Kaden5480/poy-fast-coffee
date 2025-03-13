using System.Reflection;

using HarmonyLib;

namespace FastCoffee {
    /**
     * <summary>
     * A class which applies any harmony patches.
     * </summary>
     */
    public class Patcher : Loggable {
        /**
         * <summary>
         * Applies early patches.
         * These patches are applied on Awake.
         * </summary>
         */
        public void PatchEarly() {
            Harmony.CreateAndPatchAll(typeof(Patches.BypassNormal));

            LogDebug("Applied early patches");
        }

        /**
         * <summary>
         * Gets the FieldInfo for a field
         * from a given type `T`.
         * </summary>
         * <param name="name">The name of the field to get</param>
         * <returns>The FieldInfo, or null if not found</returns>
         */
        public static FieldInfo GetFieldInfo<T>(string name) {
            return AccessTools.Field(typeof(T), name);
        }

        /**
         * <summary>
         * Gets the value of a field of type `FT` from
         * an instance of an object of type `T`.
         * </summary>
         * <param name="instance">The instance to get the value from</param>
         * <param name="name">The name of the field to get</param>
         * <returns>The value of the field</returns>
         */
        public static FT GetField<T, FT>(T instance, string name) {
            return (FT) GetFieldInfo<T>(name).GetValue(instance);
        }
    }
}
