using UnityEngine;

public class AreaEntrance : MonoBehaviour
{
    [SerializeField] private string transitionName;

    private void Start()
    {
        
        if (!string.IsNullOrEmpty(SceneManagement.Instance.SceneTransitionName))
        {
            if (transitionName == SceneManagement.Instance.SceneTransitionName)
            {
                PlayerController.Instance.transform.position = transform.position;
                SceneManagement.Instance.ResetTransition();
            }
        }
        else
        {
            
            PlayerController.Instance.transform.position =
                SceneManagement.Instance.GetSpawnPosition();
        }

        CameraController.Instance.SetPlayerCameraFollow();
        UIFade.Instance.FadeToClear();
    }
}