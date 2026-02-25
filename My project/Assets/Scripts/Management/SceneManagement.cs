using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneTransitionName { get; private set; }

    public Vector3 DefaultSpawnPosition { get; private set; }
    public Vector3 CheckpointSpawnPosition { get; private set; }

    private bool useCheckpoint = false;

    
    public void SetTransitionName(string sceneTransitionName)
    {
        SceneTransitionName = sceneTransitionName;
    }

    public void SetDefaultSpawn(Vector3 position)
    {
        DefaultSpawnPosition = position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        CheckpointSpawnPosition = position;
        useCheckpoint = true;
    }

    public Vector3 GetSpawnPosition()
    {
        if (useCheckpoint)
            return CheckpointSpawnPosition;

        return DefaultSpawnPosition;
    }

    public void ResetTransition()
    {
        SceneTransitionName = "";
    }

    public void ResetCheckpoint()
    {
        useCheckpoint = false;
    }
}