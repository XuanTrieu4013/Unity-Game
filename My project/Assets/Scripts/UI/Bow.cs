using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;

    public GameObject ArrowPrefab => arrowPrefab;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    public void Attack()
    {
        myAnimator.SetTrigger(FIRE_HASH);
        GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
        DamageSource ds = newArrow.GetComponent<DamageSource>();
        if (ds != null)
        {
            ds.SetWeaponInfo(weaponInfo, arrowPrefab);
        }
    }

    public void UseSpecialSkill()
    {
        myAnimator.SetTrigger(FIRE_HASH);
        Quaternion baseRotation = ActiveWeapon.Instance.transform.rotation;
        float[] angles = { 0, 15, -15 };
        foreach (float angle in angles)
        {
            Quaternion rot = baseRotation * Quaternion.Euler(0, 0, angle);
            GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, rot);
            newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
            DamageSource ds = newArrow.GetComponent<DamageSource>();
            if (ds != null)
            {
                ds.SetWeaponInfo(weaponInfo, arrowPrefab);
            }
        }
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}


