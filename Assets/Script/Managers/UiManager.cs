using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    const int MAX_STOCK = 25;

    [SerializeField] RectTransform barContainer;

    [SerializeField] Image wortelBar;
    [SerializeField] Image tomatBar;
    [SerializeField] Image kentangBar;
    [SerializeField] Image cabaiBar;
    [SerializeField] Image backgroundBar;

    [SerializeField] Image coinImage;
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text stockPercentText;

    void Start()
    {
        GameManager.Instance.HookUI(this);
        var player = GameManager.Instance.PlayerManager;

        player.OnStockChanged += UpdateBar;
        player.OnMoneyChanged += UpdateCoin;

        UpdateBar("", 0);
        UpdateCoin(player.Money);
    }
    void OnDestroy()
    {
        var player = GameManager.Instance?.PlayerManager;
        if (player != null)
        {
            player.OnStockChanged -= UpdateBar;
            player.OnMoneyChanged -= UpdateCoin;
        }
    }

    void UpdateCoin(int money)
    {
        if (coinText == null) return;
        coinText.text = money.ToString();
    }

    public void UpdateAllBars() => UpdateBar("", 0);
    public void UpdateBar(string id, int qty)
    {
        if (this == null || !this.gameObject.activeInHierarchy) return;

        var player = GameManager.Instance.PlayerManager;
        if (player == null || barContainer == null) return;

        if (wortelBar == null || tomatBar == null || kentangBar == null || cabaiBar == null || backgroundBar == null)
            return;

        int wortel = player.GetQty("Wortel");
        int tomat = player.GetQty("Tomat");
        int kentang = player.GetQty("Kentang");
        int cabai = player.GetQty("Cabai");

        int totalStock = wortel + tomat + kentang + cabai;
        int maxTotalStock = MAX_STOCK * 4;
        float percent = Mathf.Clamp01(totalStock / (float)maxTotalStock) * 100f;

        float containerHeight = barContainer.rect.height;
        float unitHeight = containerHeight / maxTotalStock;

        float hWortel = wortel * unitHeight;
        float hTomat = tomat * unitHeight;
        float hKentang = kentang * unitHeight;
        float hCabai = cabai * unitHeight;

        SetBarFull(backgroundBar.rectTransform);
        SetBar(cabaiBar.rectTransform, 0, hCabai);
        SetBar(kentangBar.rectTransform, hCabai, hKentang);
        SetBar(tomatBar.rectTransform, hCabai + hKentang, hTomat);
        SetBar(wortelBar.rectTransform, hCabai + hKentang + hTomat, hWortel);

        if (stockPercentText != null)
            stockPercentText.text = $"{Mathf.RoundToInt(percent)}%";
    }

    void SetBar(RectTransform bar, float bottomY, float height)
    {
        if (bar == null) return;

        bar.anchorMin = new Vector2(0, 0);
        bar.anchorMax = new Vector2(1, 0);
        bar.pivot = new Vector2(0.5f, 0);
        bar.anchoredPosition = new Vector2(0, bottomY);
        bar.sizeDelta = new Vector2(0, height);
    }

    void SetBarFull(RectTransform bar)
    {
        if (bar == null) return;

        bar.anchorMin = new Vector2(0, 0);
        bar.anchorMax = new Vector2(1, 1);
        bar.pivot = new Vector2(0.5f, 0);
        bar.anchoredPosition = Vector2.zero;
        bar.sizeDelta = Vector2.zero;
    }
}
