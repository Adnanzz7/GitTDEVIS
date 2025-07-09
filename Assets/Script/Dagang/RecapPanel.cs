using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecapPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI buyerText;
    [SerializeField] TextMeshProUGUI wortelText;
    [SerializeField] TextMeshProUGUI tomatText;
    [SerializeField] TextMeshProUGUI kentangText;
    [SerializeField] TextMeshProUGUI cabaiText;
    [SerializeField] TextMeshProUGUI coinText;
    [SerializeField] Button backButton;

    void Awake()
    {
        gameObject.SetActive(false);
        backButton.onClick.AddListener(BackToWalk);
    }

    public void ShowRecap()
    {
        buyerText.text = SalesStats.Buyers.ToString();
        wortelText.text = SalesStats.SoldWortel.ToString();
        tomatText.text = SalesStats.SoldTomat.ToString();
        kentangText.text = SalesStats.SoldKentang.ToString();
        cabaiText.text = SalesStats.SoldCabai.ToString();
        coinText.text = SalesStats.CoinEarned.ToString();

        gameObject.SetActive(true);
    }

    void BackToWalk()
    {
        SpawnPoint.LastSpawn = "frontDagang";
        SceneManager.LoadScene("WalkScene");
    }
}
