using UnityEngine;
using Fungus;
using System.Collections;

public class FungusTradeBridge : MonoBehaviour
{
    Flowchart activeFlow;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        var tm = GameManager.Instance.TradeManager;
        tm.OnTradeSuccess += OnSuccess;
        tm.OnPriceRejected += OnPriceRejected;
        tm.OnTradeHardFail += OnHardFail;

        Debug.Log("[FTB] Listener FungusTradeBridge di‑aktifkan");
    }

    public void RegisterFlow(Flowchart f) => activeFlow = f;

    void OnDestroy()
    {
        var tm = GameManager.Instance?.TradeManager;
        if (tm != null)
        {
            tm.OnTradeSuccess -= OnSuccess;
            tm.OnTradeHardFail -= OnHardFail;
            tm.OnPriceRejected -= OnPriceRejected;
        }
    }

    void OnSuccess(int profit)
    {
        if (!activeFlow) { Debug.LogWarning("[FTB] flow null"); return; }

        var buyer = BuyerBehaviour.CurrentBuyer;
        activeFlow.SetBooleanVariable("PriceAccepted", true);
        activeFlow.SetBooleanVariable("IsProfit", profit > 0);
        activeFlow.SetStringVariable("Line",
            DialogBank.GetHappy(buyer ? buyer.PersonalityEnum : Personality.Friendly));

        StartCoroutine(ExecuteNextFrame("TradeResult"));
    }

    IEnumerator ExecuteNextFrame(string blockName)
    {
        activeFlow.StopAllBlocks();
        yield return null;
        activeFlow.ExecuteBlock(blockName);
    }

    void OnHardFail()
    {
        if (activeFlow == null) return;
        var buyer = BuyerBehaviour.CurrentBuyer;

        activeFlow.SetBooleanVariable("PriceAccepted", false);
        activeFlow.SetStringVariable("Line",
            DialogBank.GetReject(buyer != null ? buyer.PersonalityEnum : Personality.Friendly));

        StartCoroutine(ExecuteNextFrame("TradeResult"));
    }

    void OnPriceRejected()
    {
        if (activeFlow == null) return;
        var buyer = BuyerBehaviour.CurrentBuyer;

        activeFlow.SetStringVariable("Line",
            DialogBank.GetReject(buyer != null ? buyer.PersonalityEnum : Personality.Friendly));

        StartCoroutine(ExecuteNextFrame("BuyCounter"));
    }

}