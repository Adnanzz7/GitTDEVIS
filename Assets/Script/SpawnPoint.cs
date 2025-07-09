using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPoint : MonoBehaviour
{
    public string id;

    public static string LastSpawn = "";

    private void Awake()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (LastSpawn == "") return;

        foreach (var sp in FindObjectsOfType<SpawnPoint>())
        {
            if (sp.id == LastSpawn)
            {
                var player = GameObject.FindGameObjectWithTag("Player");
                if (player)
                    player.transform.position = sp.transform.position;
                break;
            }
        }

        LastSpawn = "";
    }
}
