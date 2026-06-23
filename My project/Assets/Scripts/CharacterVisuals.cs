using UnityEngine;

namespace Game.Characters
{
    /// <summary>
    /// Handles the visual representation of a character based on CharacterData.
    /// 
    /// Inspector Setup Instructions:
    /// 1. Attach this script to your main Character GameObject.
    /// 2. Ensure the Character GameObject has a SpriteRenderer component (it will be auto-fetched if left null).
    /// 3. Create an empty child GameObject on your Character, name it "Weapon", and add a SpriteRenderer to it.
    /// 4. Drag that "Weapon" GameObject into the 'WeaponSpriteRenderer' field in this script.
    /// 5. Assign your desired CharacterData (e.g., GladiatorData) to the 'CurrentData' field.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class CharacterVisuals : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The ScriptableObject containing stats and sprites for this character class.")]
        public CharacterData CurrentData;

        [Header("Renderers")]
        [Tooltip("SpriteRenderer for the character's body. Auto-assigned if empty.")]
        public SpriteRenderer CharacterSpriteRenderer;
        
        [Tooltip("SpriteRenderer for the equipped weapon (should be a child GameObject).")]
        public SpriteRenderer WeaponSpriteRenderer;

        private void Awake()
        {
            // Cache the Character's SpriteRenderer component to avoid GetComponent calls later
            if (CharacterSpriteRenderer == null)
            {
                CharacterSpriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            // Apply the visuals at startup
            ApplyVisuals();
        }

        /// <summary>
        /// Reads from CurrentData and assigns the correct sprites to the SpriteRenderers.
        /// </summary>
        public void ApplyVisuals()
        {
            if (CurrentData == null)
            {
                Debug.LogWarning($"[{gameObject.name}] CharacterVisuals: No CharacterData assigned!", this);
                return;
            }

            if (CharacterSpriteRenderer != null)
            {
                CharacterSpriteRenderer.sprite = CurrentData.IdleSpriteSheet;
            }

            if (WeaponSpriteRenderer != null)
            {
                WeaponSpriteRenderer.sprite = CurrentData.WeaponSprite;
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] CharacterVisuals: WeaponSpriteRenderer is missing. Please assign a child SpriteRenderer for the weapon.", this);
            }
        }

        /// <summary>
        /// Call this to dynamically swap classes at runtime.
        /// </summary>
        /// <param name="newData">The new character data to apply</param>
        public void SwapClass(CharacterData newData)
        {
            if (newData != null)
            {
                CurrentData = newData;
                ApplyVisuals();
            }
        }
    }
}
