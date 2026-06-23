using UnityEngine;

namespace Game.Characters
{
    /// <summary>
    /// ScriptableObject containing all base stats and visuals for a specific Character Class.
    /// How to use:
    /// 1. Right-click in the Project window -> Create -> Game -> Character Data.
    /// 2. Name the file (e.g., "GladiatorData").
    /// 3. Fill in the inspector fields with your corresponding sprites (Greatsword, etc.) and stats.
    /// </summary>
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Game/Character Data", order = 0)]
    public class CharacterData : ScriptableObject
    {
        [Header("Stats")]
        public string ClassName;
        public float MoveSpeed;
        public int BaseHealth;
        public int BaseDamage;

        [Header("Visuals")]
        [Tooltip("The main sprite or sprite sheet used for the Idle state.")]
        public Sprite IdleSpriteSheet;
        
        [Tooltip("The sprite or sprite sheet used for the Attack state.")]
        public Sprite AttackSpriteSheet;
        
        [Tooltip("The sprite used for the character's equipped weapon (e.g., Staff, Longbow, Greatsword).")]
        public Sprite WeaponSprite;
    }
}
