using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class DayManager : MonoBehaviour
{
    public static DayManager I { get; private set; }

    public int CurrentDay { get; private set; } = 1;
    public bool IsNight { get; private set; }

    [SerializeField] CanvasGroup nightOverlay;

    public event Action OnNight;
    public event Action OnMorning;

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += RebindOverlay;
        if (nightOverlay != null) nightOverlay.gameObject.SetActive(false);
    }

    void OnDestroy() =>
        SceneManager.sceneLoaded -= RebindOverlay;

    void RebindOverlay(Scene s, LoadSceneMode m)
    {
        if (nightOverlay == null)
        {
            var go = GameObject.FindWithTag("NightOverlay");
            if (go != null)
                nightOverlay = go.GetComponent<CanvasGroup>();
        }
    }

    public void StartNight()
    {
        if (IsNight) return;
        IsNight = true;

        if (nightOverlay != null)
        {
            nightOverlay.alpha = 0f;
            nightOverlay.gameObject.SetActive(true);
            ScreenFader.BlendAlpha(nightOverlay, 0.6f, 0.8f);
        }
        OnNight?.Invoke();
    }

    public void StartMorning()
    {
        if (!IsNight) return;
        IsNight = false;
        CurrentDay++;

        if (nightOverlay != null)
        {
            ScreenFader.BlendAlpha(
                nightOverlay, 0f, 0.8f,
                () => nightOverlay.gameObject.SetActive(false));
        }

        GameManager.Instance.MarketManager.RollDailyPrices();
        GameManager.Instance.UIManager?.ShowPricePanel();
        OnMorning?.Invoke();
    }
}
