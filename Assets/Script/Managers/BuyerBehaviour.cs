using UnityEngine;
using Fungus;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BuyerBehaviour : MonoBehaviour
{
    public event Action OnTradeFinished;

    [SerializeField] Flowchart flow;
    [SerializeField] NPCProfileSO defaultProfile;
    NPCProfileSO prof;

    public Flowchart Flow => flow;
    public static Flowchart CurrentFlow { get; private set; }
    public Personality PersonalityEnum => prof.personality;

    public static BuyerBehaviour CurrentBuyer { get; private set; }
    static readonly string[] productIds = { Item.Wortel, Item.Tomat, Item.Kentang, Item.Cabai };

    public void Init(NPCProfileSO profile) => prof = profile;

    void Awake()
    {
        CurrentBuyer = this;

        if (flow == null)
            flow = GetComponent<Flowchart>() ?? GetComponentInChildren<Flowchart>();
        CurrentFlow = flow;

        FindObjectOfType<FungusTradeBridge>()?.RegisterFlow(flow);
    }

    void OnDestroy()
    {
        if (CurrentFlow == flow)
            CurrentFlow = null;
        if (CurrentBuyer == this)
            CurrentBuyer = null;
    }

    void Start()
    {
        if (prof == null) prof = defaultProfile;
        if (flow == null || GameManager.Instance?.TradeManager == null)
        { Destroy(gameObject); return; }

        string itemId = PickWeightedItem();
        int qty = Random.Range(1, 6);

        int moodStart = prof.moodStart;
        int targetMood = prof.moodTarget;

        flow.SetStringVariable("ItemID", itemId);
        flow.SetIntegerVariable("Qty", qty);
        flow.SetStringVariable("Personality", prof.personality.ToString());
        flow.SetIntegerVariable("MoodStart", moodStart);
        flow.SetIntegerVariable("Mood", moodStart);
        flow.SetIntegerVariable("TargetMood", targetMood);

        int modal = GameManager.Instance.MarketManager.HargaSatuan(itemId);
        flow.SetIntegerVariable("MarketPrice", modal);

        flow.SetStringVariable("Line", DialogBank.GetAskItem(prof.personality, qty, itemId));

        GameManager.Instance.TradeManager.MulaiTrade(itemId, qty);
        flow.ExecuteBlock("StartTrade");

        FungusTradeBridge bridge = FindObjectOfType<FungusTradeBridge>();
        if (bridge != null) bridge.RegisterFlow(flow);
    }

    public void AdjustMood(int delta)
    {
        int mood = Mathf.Clamp(flow.GetIntegerVariable("Mood") + delta, 0, 100);
        flow.SetIntegerVariable("Mood", mood);

        int start = flow.GetIntegerVariable("MoodStart");
        int target = flow.GetIntegerVariable("TargetMood");

        int threshold = Mathf.Min(start, target) - 10;

        if (!flow.GetBooleanVariable("EmergencyShown") && mood <= threshold)
        {
            flow.SetBooleanVariable("EmergencyShown", true);
            flow.SetBooleanVariable("ShowEmergency", true);
        }
    }
    string PickWeightedItem()
    {
        var list = new List<string>();
        foreach (string id in productIds)
        {
            int price = GameManager.Instance.MarketManager.HargaSatuan(id);
            int weight = Mathf.Clamp(6 - price, 1, 5);
            for (int i = 0; i < weight; i++) list.Add(id);
        }
        return list[Random.Range(0, list.Count)];
    }

    public bool MoodEnough() =>
        flow.GetIntegerVariable("Mood") >= flow.GetIntegerVariable("TargetMood");

    public void FinishTrade()
    {
        OnTradeFinished?.Invoke();
        Destroy(gameObject);
    }
}
