using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyPathfinding))]
[RequireComponent(typeof(SpriteRenderer))]
public class DenonSlimeAI : MonoBehaviour
{
    [Header("Roaming")]
    [SerializeField] private float roamInterval = 2f;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootInterval = 2f; // bắn mỗi 2 giây

    private EnemyPathfinding enemyPathfinding;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
        StartCoroutine(ShootRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while (true)
        {
            Vector2 roamDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            enemyPathfinding.MoveTo(roamDir);

            // Flip theo hướng di chuyển
            if (roamDir.x < 0f) spriteRenderer.flipX = true;
            else if (roamDir.x > 0f) spriteRenderer.flipX = false;

            yield return new WaitForSeconds(roamInterval);
        }
    }

    private IEnumerator ShootRoutine()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval); // nghỉ đúng bằng shootInterval
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || bulletSpawnPoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);

        Vector2 shootDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.SetDirection(shootDir);
        }
    }
}