using UnityEngine;
using UnityEngine.SceneManagement;

public class HUDBootstrap : MonoBehaviour
{
    [SerializeField] GameObject hudPrefab;
    readonly string[] keepScenes = { "HomeScene", "WalkScene" };

    void Awake() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDestroy() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        bool needHud = System.Array.Exists(keepScenes, name => name == s.name);

        var existingHud = FindObjectOfType<UIManager>(true);

        if (needHud && existingHud == null)
        {
            DontDestroyOnLoad(Instantiate(hudPrefab));
        }
        else if (!needHud && existingHud != null)
        {
            Destroy(existingHud.transform.root.gameObject);
        }
    }
}