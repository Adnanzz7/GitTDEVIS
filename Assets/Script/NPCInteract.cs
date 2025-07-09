using UnityEngine;
using Fungus;
using System.Collections;

public class NPCInteract : MonoBehaviour
{
    [SerializeField] GameObject textInteract;
    [SerializeField] Flowchart flowchart;
    [SerializeField] string blockName = "StartRestock";

    bool inside;
    PlayerController player;
    bool running;

    void Awake() => textInteract?.SetActive(false);

    void Update()
    {
        if (!inside || player == null || running) return;

        textInteract.SetActive(true);

        if (player.InteractPressed)
        {
            running = true;
            player.SetFrozen(true);
            textInteract.SetActive(false);
            StartCoroutine(RunFlow());
        }
    }

    IEnumerator RunFlow()
    {
        flowchart.ExecuteBlock(blockName);
        yield return new WaitUntil(() => flowchart.GetExecutingBlocks().Count == 0);
        player.SetFrozen(false);
        running = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            inside = true;
            player = col.GetComponent<PlayerController>();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            inside = false;
            textInteract?.SetActive(false);
            player = null;
        }
    }
}
