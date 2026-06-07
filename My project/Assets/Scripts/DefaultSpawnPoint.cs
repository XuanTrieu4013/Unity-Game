using UnityEngine;

public class DefaultSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (SceneManagement.Instance != null)
        {
            SceneManagement.Instance.SetDefaultSpawn(transform.position);
        }
    }
}