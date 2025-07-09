using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerManager PlayerManager { get; private set; }
    public MarketManager MarketManager { get; private set; }
    public TradeManager TradeManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public SaveManager SaveManager { get; private set; }

    [SerializeField] bool ignoreSave = true;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        PlayerManager = new PlayerManager();
        MarketManager = new MarketManager();
        TradeManager = new TradeManager();
        SaveManager = new SaveManager();

        bool loaded = !ignoreSave && SaveManager.LoadPlayer();
        if (!loaded)
        {
            PlayerManager.Money += 10;
            PlayerManager.AddStock(Item.Wortel, 10);
            PlayerManager.AddStock(Item.Tomat, 5);
            PlayerManager.AddStock(Item.Kentang, 15);
            PlayerManager.AddStock(Item.Cabai, 25);
        }
    }

    public void HookUI(UIManager ui) => UIManager = ui;

    void OnApplicationQuit() => SaveManager.SavePlayerImmediate();
}
