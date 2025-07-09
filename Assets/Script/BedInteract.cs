using System.Collections;
using UnityEngine;

public class BedInteract : MonoBehaviour
{
    [SerializeField] GameObject textPrompt;
    [SerializeField] Transform sleepSpot;
    [SerializeField] float moveSpeed = 1.5f;
    [SerializeField] float fadeTime = 0.4f;

    bool inside;
    PlayerController player;
    bool running;

    void Awake() => textPrompt.SetActive(false);

    void Update()
    {
        if (!inside || running || player == null) return;

        textPrompt.SetActive(true);

        if (player.InteractPressed)
        {
            running = true;
            textPrompt.SetActive(false);
            player.SetFrozen(true);
            StartCoroutine(SleepRoutine());
        }
    }

    IEnumerator SleepRoutine()
    {
        Vector3 originalPos = player.transform.position;

        ScreenFader.FadeToBlack(fadeTime);
        yield return new WaitForSecondsRealtime(fadeTime);

        if (sleepSpot != null)
        {
            while (Vector2.Distance(player.transform.position, sleepSpot.position) > 0.02f)
            {
                player.transform.position = Vector3.MoveTowards(
                    player.transform.position,
                    sleepSpot.position,
                    moveSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(1.0f);

        ScreenFader.FadeFromBlack(fadeTime);
        yield return new WaitForSecondsRealtime(fadeTime);

        player.transform.position = originalPos;

        GameManager.Instance.SaveManager.MarkSlept();
        UIToast.Show("Progress saved", 2f);

        player.SetFrozen(false);
        running = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    { if (col.CompareTag("Player")) { inside = true; player = col.GetComponent<PlayerController>(); } }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            inside = false;
            textPrompt.SetActive(false);

            if (!running)
                player = null;
        }
    }
}
