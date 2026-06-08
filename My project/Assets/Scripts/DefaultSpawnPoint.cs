using UnityEngine;

public class DefaultSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        if (SceneManagement.Instance != null)
        {
            SceneManagement.Instance.SetDefaultSpawn(transform.position);

            if (string.IsNullOrEmpty(SceneManagement.Instance.SceneTransitionName))
            {
                if (PlayerController.Instance != null)
                {
                    PlayerController.Instance.transform.position = transform.position;
                }
            }
        }
    }
}