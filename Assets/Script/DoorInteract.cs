using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorInteract : MonoBehaviour
{
    [SerializeField] private GameObject textInteract;
    [SerializeField] private string sceneTarget;
    [SerializeField] private string spawnPointName;

    private bool inside;
    private PlayerController player;

    private void Awake() => textInteract?.SetActive(false);

    private void Update()
    {
        if (!inside || player == null) return;

        textInteract.SetActive(true);

        if (player.InteractPressed)
        {
            SpawnPoint.LastSpawn = spawnPointName;
            ScreenFader.FadeToScene(sceneTarget);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            inside = true;
            player = col.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            inside = false;
            if (textInteract) textInteract.SetActive(false);
            player = null;
        }
    }
}
