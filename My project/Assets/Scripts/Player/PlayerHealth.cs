using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int MaxHealth;
    int currentHealth;

    public HealthBar healthBar;

    public UnityEvent OnDeath;

    private void OnEnable()
    {
        OnDeath.AddListener(Death); // sửa chính tả
    }

    private void Start()
    {
        currentHealth = MaxHealth;
        healthBar.UpdateBar(currentHealth, MaxHealth);
    }

    public void TakeDamage(int Damage) 
    {
        currentHealth -= Damage;

        if (currentHealth <= 0) // sửa điều kiện
        {
            currentHealth = 0;
            OnDeath.Invoke();
        }

        healthBar.UpdateBar(currentHealth, MaxHealth);
    }

    public void Death()
    {
        Destroy(gameObject); // sửa chính tả
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }
}