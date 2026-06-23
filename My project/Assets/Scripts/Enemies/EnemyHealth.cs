using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum EnemyDebuffState
{
    None,
    Electrified,
    Marked,
    Vulnerable
}

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    public EnemyDebuffState currentDebuff = EnemyDebuffState.None;
    private Coroutine debuffCoroutine;
    private SpriteRenderer spriteRenderer;
    private Color originalSpriteColor;
    private GameObject activeDebuffTextGo;

    private int currenHealth;
    private Knockback knockback;
    private Flash flash;

    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSpriteColor = spriteRenderer.color;
        }
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

    public void ApplyDebuff(EnemyDebuffState debuff, float duration)
    {
        if (debuffCoroutine != null)
        {
            StopCoroutine(debuffCoroutine);
        }
        
        currentDebuff = debuff;
        debuffCoroutine = StartCoroutine(DebuffDurationRoutine(duration));
    }

    public void ClearDebuff()
    {
        if (debuffCoroutine != null)
        {
            StopCoroutine(debuffCoroutine);
            debuffCoroutine = null;
        }
        currentDebuff = EnemyDebuffState.None;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalSpriteColor;
        }
        HideDebuffIndicator();
    }

    private void ShowDebuffIndicator(string text, Color color)
    {
        if (activeDebuffTextGo != null)
        {
            Destroy(activeDebuffTextGo);
        }
        
        activeDebuffTextGo = new GameObject("DebuffIndicator", typeof(RectTransform), typeof(TextMeshPro));
        activeDebuffTextGo.transform.SetParent(transform);
        activeDebuffTextGo.transform.localPosition = new Vector3(0, 1f, 0); // Hiển thị trên đầu quái
        
        TextMeshPro tmp = activeDebuffTextGo.GetComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 3f;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;
    }

    private void HideDebuffIndicator()
    {
        if (activeDebuffTextGo != null)
        {
            Destroy(activeDebuffTextGo);
            activeDebuffTextGo = null;
        }
    }

    private void OnDestroy()
    {
        HideDebuffIndicator();
    }

    public void ShowComboPopup(string text, Color color)
    {
        CreateFloatingText(text, color, 1f, new Vector3(0, 0.5f, 0));
    }

    private GameObject CreateFloatingText(string text, Color color, float duration, Vector3 offset)
    {
        GameObject textGo = new GameObject("FloatingText", typeof(RectTransform), typeof(TextMeshPro), typeof(FloatingText));
        textGo.transform.position = transform.position + offset;
        
        FloatingText ft = textGo.GetComponent<FloatingText>();
        ft.Setup(text, color, duration, new Vector3(0, 1f, 0));
        return textGo;
    }

    private IEnumerator DebuffDurationRoutine(float duration)
    {
        if (spriteRenderer != null)
        {
            switch (currentDebuff)
            {
                case EnemyDebuffState.Electrified:
                    spriteRenderer.color = new Color(0.5f, 0.8f, 1f, 1f); // Xanh điện (Cyan)
                    ShowDebuffIndicator("TÍCH ĐIỆN", new Color(0.3f, 0.7f, 1f));
                    break;
                case EnemyDebuffState.Marked:
                    spriteRenderer.color = new Color(1f, 0.9f, 0.5f, 1f); // Vàng nhạt (Marked)
                    ShowDebuffIndicator("ĐÁNH DẤU", new Color(1f, 0.9f, 0.2f));
                    break;
                case EnemyDebuffState.Vulnerable:
                    spriteRenderer.color = new Color(1f, 0.5f, 0.5f, 1f); // Đỏ nhạt (Vulnerable)
                    ShowDebuffIndicator("SUY YẾU", new Color(1f, 0.3f, 0.3f));
                    break;
                default:
                    spriteRenderer.color = originalSpriteColor;
                    HideDebuffIndicator();
                    break;
            }
        }

        yield return new WaitForSeconds(duration);

        currentDebuff = EnemyDebuffState.None;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalSpriteColor;
        }
        HideDebuffIndicator();
    }
}

public class FloatingText : MonoBehaviour
{
    private TextMeshPro tmp;
    private float duration;
    private Vector3 targetOffset;

    public void Setup(string text, Color color, float duration, Vector3 targetOffset)
    {
        tmp = GetComponent<TextMeshPro>();
        if (tmp == null) tmp = gameObject.AddComponent<TextMeshPro>();
        
        tmp.text = text;
        tmp.color = color;
        tmp.fontSize = 4f;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 11;
        
        this.duration = duration;
        this.targetOffset = targetOffset;
        
        StartCoroutine(Animate());
    }

    private IEnumerator Animate()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + targetOffset;
        Color startColor = tmp.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            tmp.color = new Color(startColor.r, startColor.g, startColor.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        
        Destroy(gameObject);
    }
}
