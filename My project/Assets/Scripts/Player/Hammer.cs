using System.Collections;
using System.Collections.Generic;
using UnityEngine;public class Hammer : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float hammerAttackCD = .5f;
    [SerializeField] private WeaponInfo weaponInfo;
    

    private Transform weaponCollider;
    private Animator myAnimator;

    private GameObject slashAnim;

    private void Awake() {
        myAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }

    private void Update() {
        if (PauseMenu.GameIsPaused) return;
        MouseFollowWithOffset();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void UseSpecialSkill()
    {
        float[] angles = { 0, 90, 180, 270 };
        Vector3 spawnPos = transform.position;
        foreach (float angle in angles)
        {
            GameObject slash = Instantiate(slashAnimPrefab, spawnPos, Quaternion.Euler(0, 0, angle));
            slash.transform.parent = this.transform.parent;
            slash.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3.5f);
        
        if (ScreenShakeManager.Instance != null)
        {
            ScreenShakeManager.Instance.ShakeScreen();
        }

        foreach (Collider2D col in colliders)
        {
            EnemyHealth enemy = col.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                int skillDamage = weaponInfo.weaponDamage * 2;
                int finalDamage = skillDamage;

                EnemyAI ai = col.GetComponent<EnemyAI>();
                if (ai != null)
                {
                    ai.Stun(1.5f);
                }

                if (enemy.currentDebuff == EnemyDebuffState.Electrified)
                {
                    Collider2D[] nearby = Physics2D.OverlapCircleAll(enemy.transform.position, 3f);
                    foreach (Collider2D nCol in nearby)
                    {
                        EnemyHealth otherEnemy = nCol.GetComponent<EnemyHealth>();
                        if (otherEnemy != null && nCol.gameObject != enemy.gameObject)
                        {
                            otherEnemy.TakeDamage(Mathf.RoundToInt(skillDamage * 1.5f));
                            Knockback nKb = nCol.GetComponent<Knockback>();
                            if (nKb != null) nKb.GetKnockedBack(enemy.transform, 12f);
                        }
                    }
                    enemy.ShowComboPopup("OVERLOAD!", new Color(0.3f, 0.7f, 1f));
                    enemy.ClearDebuff();
                }
                else if (enemy.currentDebuff == EnemyDebuffState.Marked)
                {
                    finalDamage *= 2;
                    if (Stamina.Instance != null)
                    {
                        Stamina.Instance.RefreshStamina();
                    }
                    enemy.ShowComboPopup("CRITICAL!", new Color(1f, 0.9f, 0.2f));
                    enemy.ClearDebuff();
                }
                else
                {
                    enemy.ApplyDebuff(EnemyDebuffState.Vulnerable, 3f);
                    enemy.ShowComboPopup("STUNNED!", new Color(1f, 0.5f, 0.2f));
                }

                enemy.TakeDamage(finalDamage);

                Knockback kb = col.GetComponent<Knockback>();
                if (kb != null)
                {
                    kb.GetKnockedBack(transform, 18f);
                }
            }
        }
    }

    public void Attack() {

        myAnimator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;

    }

   

    public void DoneAttackingAnimEvent() {
        weaponCollider.gameObject.SetActive(false);
    }


    public void SwingUpFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft) { 
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MouseFollowWithOffset() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x) {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        } else {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
