using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    [SerializeField] private string targetScene = "WalkScene";
    [SerializeField] private string spawnPointName = "";

    public void BackToScene()
    {
        if (!string.IsNullOrEmpty(spawnPointName))
            SpawnPoint.LastSpawn = spawnPointName;

        SceneManager.LoadScene(targetScene);
    }
}
