#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class WeaponInfoInitializer
{
    static WeaponInfoInitializer()
    {
        InitializeWeaponTypes();
    }

    [MenuItem("Tools/Initialize Weapon Types")]
    public static void InitializeWeaponTypes()
    {
        string[] guids = AssetDatabase.FindAssets("t:WeaponInfo");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            WeaponInfo info = AssetDatabase.LoadAssetAtPath<WeaponInfo>(path);
            if (info != null)
            {
                bool dirty = false;
                if (info.name.Contains("Bow") && info.weaponType != WeaponInfo.WeaponType.Ranged)
                {
                    info.weaponType = WeaponInfo.WeaponType.Ranged;
                    dirty = true;
                }
                else if (info.name.Contains("Staff") && info.weaponType != WeaponInfo.WeaponType.Magic)
                {
                    info.weaponType = WeaponInfo.WeaponType.Magic;
                    dirty = true;
                }
                else if ((info.name.Contains("Sword") || info.name.Contains("Hammer")) && info.weaponType != WeaponInfo.WeaponType.Melee)
                {
                    info.weaponType = WeaponInfo.WeaponType.Melee;
                    dirty = true;
                }

                if (dirty)
                {
                    EditorUtility.SetDirty(info);
                    Debug.Log($"[WeaponInfoInitializer] Automatically set {info.name} type to {info.weaponType}");
                }
            }
        }
        AssetDatabase.SaveAssets();
    }
}
#endif
