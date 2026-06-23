using UnityEngine;
using UnityEditor;
using Game.Characters; // Ensure we use the correct namespace

namespace Game.EditorScripts
{
    public class CharacterSetupEditor : Editor
    {
        [MenuItem("Tools/Setup Character System")]
        public static void SetupCharacterSystem()
        {
            // 1. Create Scriptable Objects for our 3 classes
            Sprite bowSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Bow.png");
            Sprite staffSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Staff.png");
            Sprite swordSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Player/Sword.png");
            
            // The Ranger- Base.png was not sliced, so we load a properly sliced idle sprite instead
            string idleSpritePath = "Assets/Sprites/Player/Side animations/spr_player_right_idle.png";
            Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(idleSpritePath);
            Sprite playerSprite = null;
            foreach (Object obj in allAssets)
            {
                if (obj is Sprite && obj.name == "spr_player_right_idle_0")
                {
                    playerSprite = obj as Sprite;
                    break;
                }
            }

            CreateOrUpdateCharacterData("GladiatorData", "Gladiator", 4f, 150, 20, playerSprite, swordSprite);
            CreateOrUpdateCharacterData("ArcherData", "Archer", 5.5f, 100, 15, playerSprite, bowSprite);
            CreateOrUpdateCharacterData("MageData", "Mage", 4.5f, 80, 25, playerSprite, staffSprite);

            // 2. Create the main Character GameObject
            GameObject characterObj = new GameObject("PlayerCharacter");
            
            // 3. Add the Visuals Script (this auto-adds SpriteRenderer)
            CharacterVisuals visuals = characterObj.AddComponent<CharacterVisuals>();
            visuals.CharacterSpriteRenderer = characterObj.GetComponent<SpriteRenderer>();

            // 4. Create child Weapon GameObject
            GameObject weaponObj = new GameObject("Weapon");
            weaponObj.transform.SetParent(characterObj.transform);
            weaponObj.transform.localPosition = new Vector3(0.5f, 0, 0); // slightly offset

            // 5. Add SpriteRenderer to weapon
            SpriteRenderer weaponRenderer = weaponObj.AddComponent<SpriteRenderer>();
            visuals.WeaponSpriteRenderer = weaponRenderer;

            // 6. Assign Default Data (Gladiator)
            CharacterData gladiatorData = AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/ScriptableObjects/GladiatorData.asset");
            if (gladiatorData != null)
            {
                visuals.CurrentData = gladiatorData;
                visuals.ApplyVisuals(); // Preview the visuals in Editor
            }

            // Register Undo so the user can easily revert if they want
            Undo.RegisterCreatedObjectUndo(characterObj, "Setup Character System");
            Selection.activeGameObject = characterObj;

            Debug.Log("<color=green><b>Character System Setup Complete!</b></color>\n" +
                      "1. ScriptableObjects created in 'Assets/ScriptableObjects'.\n" +
                      "2. 'PlayerCharacter' GameObject created in the scene with all components attached.");
        }

        private static void CreateOrUpdateCharacterData(string fileName, string className, float speed, int health, int damage, Sprite idleSprite, Sprite weaponSprite)
        {
            string folderPath = "Assets/ScriptableObjects";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");
            }

            string assetPath = $"{folderPath}/{fileName}.asset";
            CharacterData data = AssetDatabase.LoadAssetAtPath<CharacterData>(assetPath);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<CharacterData>();
                data.ClassName = className;
                data.MoveSpeed = speed;
                data.BaseHealth = health;
                data.BaseDamage = damage;
                data.IdleSpriteSheet = idleSprite;
                data.WeaponSprite = weaponSprite;
                
                AssetDatabase.CreateAsset(data, assetPath);
            }
            else
            {
                data.ClassName = className;
                data.MoveSpeed = speed;
                data.BaseHealth = health;
                data.BaseDamage = damage;
                data.IdleSpriteSheet = idleSprite;
                data.WeaponSprite = weaponSprite;
                EditorUtility.SetDirty(data);
            }
            
            AssetDatabase.SaveAssets();
        }
    }
}
