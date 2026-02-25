using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool isDead { get; private set; }
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;

    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;

    [SerializeField] private GameObject deathPanel; // tham chiếu tới UI Panel
    [SerializeField] private GameObject defaultWeaponPrefab; // prefab vũ khí mặc định

    const string HEALTH_SLIDER_TEXT = "Health Slider";
    const string TOWN_TEXT = "Scene1";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
    }

    private void Start()
    {
        ResetHealth(); // reset nhân vật khi scene khởi động

        if (deathPanel != null)
            deathPanel.SetActive(false); // ẩn panel lúc đầu
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();
        if (enemy)
        {
            TakeDamage(1, other.transform);
        }
    }

    public void HealPlayer()
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += 1;
            UpdateHealthSlider();
        }
    }

    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) return;

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }

    private void CheckIfPlayerDeath()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            // Xóa vũ khí khi chết
            if (ActiveWeapon.Instance != null)
                Destroy(ActiveWeapon.Instance.gameObject);

            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);

            // Hiện panel thay vì load scene ngay
            if (deathPanel != null)
                deathPanel.SetActive(true);

            // Dừng game
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // bật lại thời gian

        // Reset nhân vật ngay khi nhấn Restart
        ResetHealth();

        // Ẩn panel chết
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // Load lại scene mong muốn
        SceneManager.LoadScene(TOWN_TEXT);
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthSlider();

        // Reset Animator
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        // Reset lại vũ khí nếu đã bị hủy
        if (ActiveWeapon.Instance == null && defaultWeaponPrefab != null)
        {
            Instantiate(defaultWeaponPrefab, transform.position, Quaternion.identity);
        }
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(damageRecoveryTime);
        canTakeDamage = true;
    }

    private void UpdateHealthSlider()
    {
        if (healthSlider == null)
        {
            healthSlider = GameObject.Find(HEALTH_SLIDER_TEXT).GetComponent<Slider>();
        }

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}