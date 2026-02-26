using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : Singleton<EnemyManager>
{
    private int enemyCount;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        enemyCount = FindObjectsOfType<EnemyAI>().Length;
    }

    public void EnemyDied()
    {
        enemyCount--;
    }

    public bool AllEnemiesDead()
    {
        return enemyCount <= 0;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}