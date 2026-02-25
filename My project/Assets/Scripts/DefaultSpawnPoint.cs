using UnityEngine;

public class DefaultSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        SceneManagement.Instance.SetDefaultSpawn(transform.position);
    }
}