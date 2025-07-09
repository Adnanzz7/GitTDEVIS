using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup panel;
    [SerializeField] Button btnQuit;
    [SerializeField] Button btnResume;

    PlayerController player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        btnQuit.onClick.AddListener(OnQuit);
        btnResume.onClick.AddListener(() => Toggle(false));

        Toggle(false);
    }

    void Update()
    {
        if (player == null) return;

        if (player.EscapePressed)
            Toggle(!panel.gameObject.activeSelf);
    }

    void Toggle(bool on)
    {
        panel.gameObject.SetActive(on);
        panel.blocksRaycasts = on;
        panel.alpha = on ? 1 : 0;
        Time.timeScale = on ? 0 : 1;
    }

    void OnQuit()
    {
        if (GameManager.Instance.SaveManager.IsUnsaved())
        {
            PopupUnsaved.Show(
                "Kamu belum tidur / belum save!\nKeluar tetap?",
                onQuit: () => { Time.timeScale = 1; Application.Quit(); });
        }
        else
        {
            Time.timeScale = 1;
            Application.Quit();
        }
    }
}
