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
    [SerializeField] private bool stagger;
    [SerializeField] private bool oscillate;

    private bool isShooting = false;

    private void OnValidate()
    {
        if(oscillate) {stagger = true;}
        if(!oscillate) {stagger = false; }
        if(projectilesPerBurst < 1) {projectilesPerBurst = 1;}
        if(busrtCount < 1 ) {busrtCount = 1; }
        if(timeBetweenBrusts < 0.1f) {timeBetweenBrusts = 0.1f;}
        if(restTime < 0.1f) {restTime = 0.1f ;}
        if(startingDistance < 0.1f ) {startingDistance = 0.1f;}
        if(angleSpread == 0) {projectilesPerBurst = 1;}
        if(bulletMoveSpeed <= 0) {bulletMoveSpeed = 0.1f;}
    }

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

        float startAngle, currenAngle, angleStep, endAngle;
        float timeBetweenProjectiles = 0f;

        TargetConeOfInfluence(out startAngle, out currenAngle, out angleStep, out endAngle);

        if (stagger) {timeBetweenProjectiles = timeBetweenBrusts / projectilesPerBurst ;}

        

        for (int i = 0 ; i < busrtCount; i++)
        {
            if (!oscillate)
            {
                TargetConeOfInfluence(out startAngle, out currenAngle, out angleStep, out endAngle);
            }

            if(oscillate && i % 2 != 1)
            {
                TargetConeOfInfluence(out startAngle, out currenAngle, out angleStep, out endAngle);
            } else if (oscillate)
            {
                currenAngle = endAngle;
                endAngle = startAngle;
                startAngle = currenAngle;
                angleStep *= -1;
            }

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

                if(stagger) { yield return new WaitForSeconds(timeBetweenProjectiles);}
            }
            currenAngle = startAngle;

            if(!stagger) {yield return new WaitForSeconds(timeBetweenBrusts);}
        }

        yield return new WaitForSeconds(restTime);
        isShooting = false;
    }
    private void TargetConeOfInfluence(out float startAngle, out float currenAngle, out float angleStep, out float endAngle)
    {
        Vector2 targetDirection = PlayerController.Instance.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        startAngle = targetAngle;
        endAngle = targetAngle;
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
