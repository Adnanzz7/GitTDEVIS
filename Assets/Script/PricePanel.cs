using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PricePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI wortelRow;
    [SerializeField] TextMeshProUGUI tomatRow;
    [SerializeField] TextMeshProUGUI kentangRow;
    [SerializeField] TextMeshProUGUI cabaiRow;
    [SerializeField] Button btnClose;
    [SerializeField] CanvasGroup cg;

    void Awake()
    {
        btnClose.onClick.AddListener(() => Toggle(false));
        Toggle(false);
    }

    void Toggle(bool on)
    {
        gameObject.SetActive(on);
        cg.alpha = on ? 1 : 0;
        cg.blocksRaycasts = on;
    }

    public void ShowPrices()
    {
        gameObject.SetActive(true);
        var m = GameManager.Instance.MarketManager;
        FillRow(wortelRow, Item.Wortel, m);
        FillRow(tomatRow, Item.Tomat, m);
        FillRow(kentangRow, Item.Kentang, m);
        FillRow(cabaiRow, Item.Cabai, m);

        Toggle(true);
    }

    void FillRow(TextMeshProUGUI row, string id, MarketManager m)
    {
        int price = m.HargaSatuan(id);
        int d = m.Delta(id);
        string arrow = d > 0 ? "<color=#ff5555>↑</color>"
                    : d < 0 ? "<color=#55ff55>↓</color>"
                            : "=";
        row.text = $"{id}: {price}  {arrow}";
    }
}
