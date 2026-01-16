using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Shooter : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletMoveSpeed;
    [SerializeField] private int busrtCount;
    [SerializeField] private int projectilesPerBurst;
    [SerializeField][Range(0, 359)] private float angleSpread;
    [SerializeField] private float startingDistance = 0.1f ;
    [SerializeField] private float timeBetweenBrusts;
    [SerializeField] private float restTime = 1f;

    private bool isShooting = false;

    public void Attack()
    {
        if (!isShooting)
        {
            StartCoroutine(ShootRoutine());
        }
    }

    private IEnumerator ShootRoutine()
    {
        isShooting = true;

        float startAngle, currenAngle, angleStep;

        TargetConeOfInfluence(out startAngle, out currenAngle, out angleStep);

        

        for (int i = 0 ; i < busrtCount; i++)
        {
            for (int j = 0 ; j<projectilesPerBurst;j++)
            {
                Vector2 pos =  FindBulletSpawnPos(currenAngle);

                GameObject newBullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
                newBullet.transform.right = newBullet.transform.position - transform.position;

                if(newBullet.TryGetComponent(out Projectile projectile))
                {
                    projectile.UpdateMoveSpeed(bulletMoveSpeed);
                }
                currenAngle += angleStep; 
            }
            currenAngle = startAngle;

            yield return new WaitForSeconds(timeBetweenBrusts);
            TargetConeOfInfluence(out startAngle, out currenAngle, out angleStep);
        }

        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }
    private void TargetConeOfInfluence(out float startAngle, out float currenAngle, out float angleStep)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        float endAngle = targetAngle;
        currenAngle = targetAngle;
        float halfAngleSpread = 0f;
        angleStep = 0;
        if(angleSpread != 0)
        {
            angleStep = angleSpread / (projectilesPerBurst -1 );
            halfAngleSpread = angleSpread / 2f;
            startAngle = targetAngle - halfAngleSpread;
            endAngle = targetAngle + halfAngleSpread;
            currenAngle = startAngle;
        }
    }
    private Vector2 FindBulletSpawnPos( float currenAngle)
    {
        float x = transform.position.x + startingDistance * Mathf.Cos(currenAngle * Mathf.Deg2Rad);
        float y = transform.position.y + startingDistance * Mathf.Sin(currenAngle * Mathf.Deg2Rad);
    
        Vector2 pos = new Vector2(x,y);

        return pos;
    }
}
