using System.Collections;
using UnityEngine;

public class WarningUI : MonoBehaviour
{
    public static WarningUI Instance;

    [SerializeField] private CanvasGroup warningCanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private Coroutine routine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        warningCanvasGroup.alpha = 0f;
    }

    public void ShowWarning()
    {
        if (routine != null)
            StopCoroutine(routine);

        routine = StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        float timer = 0f;
        warningCanvasGroup.alpha = 1f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            warningCanvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }

        warningCanvasGroup.alpha = 0f;
    }
}