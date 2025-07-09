using UnityEngine;

public class DoorInteract : MonoBehaviour
{
    [SerializeField] GameObject textInteract;
    [SerializeField] string sceneTarget;
    [SerializeField] string spawnPointName;

    bool inside;
    PlayerController player;

    void Awake() => textInteract?.SetActive(false);

    void Update()
    {
        if (!inside || player == null) return;

        if (textInteract) textInteract.SetActive(true);

        if (player.InteractPressed)
        {
            inside = false;
            SpawnPoint.LastSpawn = spawnPointName;
            ScreenFader.FadeToScene(sceneTarget);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        { inside = true; player = col.GetComponent<PlayerController>(); }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        { inside = false; if (textInteract) textInteract.SetActive(false); player = null; }
    }

    void OnDisable()
    {
        inside = false;
        if (textInteract) textInteract.SetActive(false);
        player = null;
    }
}
