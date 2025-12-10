using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;

    private int currenHealth;

    private void Start()
    {
        currenHealth = startingHealth;
    }

    public void TakeDamage(int damage)
    {
        currenHealth -= damage;
        Debug.Log(currenHealth);
        DetectDeath();
    }

    private void DetectDeath()
    {
        if(currenHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
