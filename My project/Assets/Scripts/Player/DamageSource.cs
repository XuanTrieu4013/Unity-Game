using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int damageAmount;
    private WeaponInfo weaponInfo;

    private void Start()
    {
        MonoBehaviour currenActiveWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        if (currenActiveWeapon != null && currenActiveWeapon is IWeapon weapon)
        {
            damageAmount = weapon.GetWeaponInfo().weaponDamage;
        }
    }

    public void SetWeaponInfo(WeaponInfo info)
    {
        weaponInfo = info;
        damageAmount = info.weaponDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            WeaponInfo info = null;
            if (weaponInfo != null)
            {
                info = weaponInfo;
            }
            else
            {
                MonoBehaviour activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
                if (activeWeapon != null && activeWeapon is IWeapon weapon)
                {
                    info = weapon.GetWeaponInfo();
                }
            }

            if (info != null)
            {
                Debug.Log($"[DamageSource] Collision with {other.gameObject.name}. WeaponInfo: {info.name}, WeaponType: {info.weaponType}");
                ProcessSynergy(enemyHealth, info);
            }
            else
            {
                Debug.Log($"[DamageSource] Collision with {other.gameObject.name}. No WeaponInfo available. damageAmount: {damageAmount}");
                enemyHealth.TakeDamage(damageAmount);
            }
        }
    }

    private void ProcessSynergy(EnemyHealth enemy, WeaponInfo info)
    {
        int finalDamage = info.weaponDamage;
        bool crit = false;

        Debug.Log($"[ProcessSynergy] Processing synergy. Weapon: {info.name}, Type: {info.weaponType}");

        if (info.weaponType == WeaponInfo.WeaponType.Melee)
        {
            if (enemy.currentDebuff == EnemyDebuffState.Electrified)
            {
                // Combo Mage -> Melee (Overload)
                TriggerOverloadCombo(enemy.transform.position, info.weaponDamage);
                enemy.ShowComboPopup("OVERLOAD!", new Color(0.3f, 0.7f, 1f));
                enemy.ClearDebuff();
            }
            else if (enemy.currentDebuff == EnemyDebuffState.Marked)
            {
                // Combo Archer -> Melee (Hunter's Mark)
                finalDamage *= 2;
                crit = true;
                if (Stamina.Instance != null)
                {
                    Stamina.Instance.RefreshStamina();
                }
                enemy.ShowComboPopup("CRITICAL!", new Color(1f, 0.9f, 0.2f));
                enemy.ClearDebuff();
            }
            else
            {
                // Đòn đánh thường cận chiến tích lũy Vulnerable
                enemy.ApplyDebuff(EnemyDebuffState.Vulnerable, 3f);
            }
        }
        else if (info.weaponType == WeaponInfo.WeaponType.Ranged)
        {
            if (enemy.currentDebuff == EnemyDebuffState.Vulnerable)
            {
                // Combo Melee -> Ranged (Splitting Arrows)
                TriggerSplittingArrowsCombo(enemy.transform.position, info);
                enemy.ShowComboPopup("SPLIT!", new Color(0.4f, 1f, 0.4f));
                enemy.ClearDebuff();
            }
            else
            {
                // Đòn bắn cung thường tích lũy Marked
                enemy.ApplyDebuff(EnemyDebuffState.Marked, 5f);
            }
        }
        else if (info.weaponType == WeaponInfo.WeaponType.Magic)
        {
            // Phép thuật tích lũy Electrified
            enemy.ApplyDebuff(EnemyDebuffState.Electrified, 3f);
        }

        // Gây sát thương lên kẻ địch bị đánh trúng trực tiếp
        enemy.TakeDamage(finalDamage);

        if (crit)
        {
            // Có thể tạo hiệu ứng bay chữ hoặc nháy đỏ tại đây
            Debug.Log("CRITICAL HIT!");
        }
    }

    private void TriggerOverloadCombo(Vector3 position, int baseDamage)
    {
        float explosionRadius = 3f;
        int overloadDamage = Mathf.RoundToInt(baseDamage * 1.5f);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, explosionRadius);

        if (ScreenShakeManager.Instance != null)
        {
            ScreenShakeManager.Instance.ShakeScreen();
        }

        foreach (Collider2D col in colliders)
        {
            EnemyHealth otherEnemy = col.GetComponent<EnemyHealth>();
            if (otherEnemy != null && col.transform.position != position)
            {
                otherEnemy.TakeDamage(overloadDamage);
                Knockback kb = col.GetComponent<Knockback>();
                if (kb != null)
                {
                    kb.GetKnockedBack(this.transform, 12f);
                }
            }
        }
    }

    private void TriggerSplittingArrowsCombo(Vector3 position, WeaponInfo weaponInfo)
    {
        MonoBehaviour activeWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        if (activeWeapon != null && activeWeapon is Bow bow)
        {
            Quaternion baseRotation = ActiveWeapon.Instance.transform.rotation;
            
            float angleOffset = 30f;
            Quaternion leftRot = baseRotation * Quaternion.Euler(0, 0, angleOffset);
            Quaternion rightRot = baseRotation * Quaternion.Euler(0, 0, -angleOffset);

            if (bow.ArrowPrefab != null)
            {
                GameObject leftArrow = Instantiate(bow.ArrowPrefab, position, leftRot);
                leftArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
                DamageSource leftDs = leftArrow.GetComponent<DamageSource>();
                if (leftDs != null)
                {
                    leftDs.SetWeaponInfo(weaponInfo);
                }
                
                GameObject rightArrow = Instantiate(bow.ArrowPrefab, position, rightRot);
                rightArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
                DamageSource rightDs = rightArrow.GetComponent<DamageSource>();
                if (rightDs != null)
                {
                    rightDs.SetWeaponInfo(weaponInfo);
                }
            }
        }
    }
}
