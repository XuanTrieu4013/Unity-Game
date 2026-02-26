using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private int enemyCount;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
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
}