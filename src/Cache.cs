using Rewired;
using SCPE;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace FastCoffee {
    /**
     * <summary>
     * A cache of objects in the current scene
     * which are required for fast coffee to function.
     * </summary>
     */
    public class Cache : Loggable {
        public CoffeeDrink coffee = null;
        public GameObject coffeeBottle = null;
        public Vector3 coffeeDrinkPosition = Vector3.zero;
        public Quaternion coffeeDrinkRotation = Quaternion.identity;
        public Inventory inventory = null;
        public PlayerMove playerMove = null;
        public RoutingFlag routingFlag = null;
        public TimeAttack timeAttack = null;

        // Controls
        public Player player = null;

        // Coffee audio
        public AudioSource coffeeEffectAudio = null;

        // Coffee UI
        public CanvasGroup coffeeUsesLeftCG {
            get => Patcher.GetField<CoffeeDrink, CanvasGroup>(
                coffee, "coffeeusesleft_CG"
            );
        }

        // Coffee visual
        public DoubleVision doubleVision {
            get => Patcher.GetField<CoffeeDrink, DoubleVision>(
                coffee, "doubleVision"
            );
        }
        public SCPE.Gradient gradient {
            get => Patcher.GetField<CoffeeDrink, SCPE.Gradient>(
                coffee, "gradient"
            );
        }
        public Ripples ripples {
            get => Patcher.GetField<CoffeeDrink, Ripples>(
                coffee, "ripples"
            );
        }
        public TiltShift tiltShift {
            get => Patcher.GetField<CoffeeDrink, TiltShift>(
                coffee, "tiltShift"
            );
        }
        public Vignette vignette {
            get => Patcher.GetField<CoffeeDrink, Vignette>(
                coffee, "vignette"
            );
        }

        /**
         * <summary>
         * Finds and stores objects in the current scene.
         * </summary>
         */
        public void FindObjects() {
            GameObject coffeeAudioObj = GameObject.Find("CoffeeSound_Coffee_Effect");
            if (coffeeAudioObj != null) {
                coffeeEffectAudio = coffeeAudioObj.GetComponent<AudioSource>();
            }

            coffee = GameObject.FindObjectOfType<CoffeeDrink>();
            if (coffee != null) {
                player = Patcher.GetField<CoffeeDrink, Player>(
                    coffee, "player"
                );

                GameObject animationObj = coffee.coffeeDrinkAnimation.gameObject;

                Transform coffeeBottleTransform = animationObj
                    .transform.Find("coffeebottle");

                if (coffeeBottleTransform != null) {
                    coffeeBottle = coffeeBottleTransform.gameObject;
                }

                coffeeDrinkPosition = animationObj.transform.localPosition;
                coffeeDrinkRotation = animationObj.transform.localRotation;
            }

            inventory = GameObject.FindObjectOfType<Inventory>();
            playerMove = GameObject.FindObjectOfType<PlayerMove>();
            routingFlag = GameObject.FindObjectOfType<RoutingFlag>();
            timeAttack = GameObject.FindObjectOfType<TimeAttack>();

            LogDebug("Cached objects");
        }

        /**
         * <summary>
         * Determines whether everything for fast coffee
         * to function is available in the cache.
         * </summary>
         * <returns>True if it is, false otherwise</returns>
         */
        public bool IsComplete() {
            return coffee != null
                && coffeeBottle != null
                && coffeeEffectAudio != null
                && inventory != null
                && player != null
                && playerMove != null
                && routingFlag != null
                && timeAttack != null;
        }

        /**
         * <summary>
         * Clears the objects currently stored in the cache.
         * </summary>
         */
        public void Clear() {
            coffee = null;
            coffeeBottle = null;
            coffeeDrinkPosition = Vector3.zero;
            coffeeDrinkRotation = Quaternion.identity;
            inventory = null;
            player = null;
            playerMove = null;
            routingFlag = null;
            timeAttack = null;

            LogDebug("Cleared cache");
        }
    }
}
