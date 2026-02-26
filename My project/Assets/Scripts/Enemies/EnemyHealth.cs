using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    private int currenHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        currenHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currenHealth -= damage;
        knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath()
{
    if (currenHealth <= 0)
    {
        // Báo cho EnemyManager
        if (EnemyManager.Instance != null)
        {
            EnemyManager.Instance.EnemyDied();
        }

        Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
        GetComponent<PickupSpawner>().DropItem();
        Destroy(gameObject);
    }
}
}
