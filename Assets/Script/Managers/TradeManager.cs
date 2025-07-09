using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class TradeManager
{
    public event Action<int> OnTradeSuccess;
    public event Action OnPriceRejected;
    public event Action OnTradeHardFail;

    int hargaMinimal;
    int hargaMaxPembeli;
    string item; int qty;
    bool isDealAccepted;
    public int GetHargaMinimal() => hargaMinimal;
    public int GetHargaMaxPembeli() => hargaMaxPembeli;

    public void MulaiTrade(string id, int jumlah)
    {
        item = id;
        qty = jumlah;
        isDealAccepted = false;

        int modal = GameManager.Instance.MarketManager.HargaSatuan(id);
        hargaMinimal = modal + 1;
        hargaMaxPembeli = Mathf.RoundToInt(modal * 1.6f);

        Debug.Log($"[MulaiTrade] item: {id}, qty: {jumlah}, hargaMin: {hargaMinimal}, hargaMax: {hargaMaxPembeli}");
    }


    public void TawarkanNormal(int pricePerUnit, BuyerBehaviour buyer)
    {
        Debug.Log("[TradeManager] TawarkanNormal");
        if (buyer == null) return;

        int delta = pricePerUnit < hargaMinimal ? +10
                 : pricePerUnit > hargaMaxPembeli ? -5
                 : +5;
        buyer.AdjustMood(delta);

        if (pricePerUnit > hargaMaxPembeli)
        {
            OnPriceRejected?.Invoke();
            return;
        }

        if (pricePerUnit <= hargaMinimal)
        {
            Deal(pricePerUnit, buyer);
            return;
        }

        float chance = GetAcceptChance(buyer.PersonalityEnum);
        chance += buyer.Flow.GetIntegerVariable("Mood") / 200f;
        chance = Mathf.Clamp01(chance);

        if (Random.value <= chance)
            Deal(pricePerUnit, buyer);
        else
            OnPriceRejected?.Invoke();
    }

    public void TawarkanDarurat(int pricePerUnit, BuyerBehaviour buyer)
    {
        if (buyer == null) return;
        float chance = GetAcceptChance(buyer.PersonalityEnum);

        if (Random.value > chance)
        {
            OnTradeHardFail?.Invoke();
            return;
        }

        Deal(pricePerUnit, buyer);
    }

    public void TolakDarurat(BuyerBehaviour buyer)
    {
        Debug.Log("[TradeManager] Harga darurat ditolak.");

        var flow = buyer.Flow;
        if (flow != null)
        {
            flow.SetBooleanVariable("PriceAccepted", false);
            flow.SetStringVariable("Line", DialogBank.GetReject(buyer.PersonalityEnum));
            flow.ExecuteBlock("BuyerEvaluateSpesial");
        }
    }

    void Deal(int pricePerUnit, BuyerBehaviour buyer)
    {
        if (isDealAccepted) return;
        isDealAccepted = true;

        var player = GameManager.Instance.PlayerManager;
        if (!player.RemoveStock(item, qty))
        {
            OnTradeHardFail?.Invoke();
            return;
        }

        int modal = GameManager.Instance.MarketManager.HargaSatuan(item);
        int total = pricePerUnit * qty;
        int profit = (pricePerUnit - modal) * qty;

        player.Money += total;

        SalesStats.Buyers++;
        switch (item)
        {
            case Item.Wortel: SalesStats.SoldWortel += qty; break;
            case Item.Tomat: SalesStats.SoldTomat += qty; break;
            case Item.Kentang: SalesStats.SoldKentang += qty; break;
            case Item.Cabai: SalesStats.SoldCabai += qty; break;
        }
        SalesStats.CoinEarned += total;

        var flow = buyer.Flow;
        if (flow)
        {
            flow.SetBooleanVariable("PriceAccepted", true);
            flow.SetBooleanVariable("IsProfit", profit > 0);
        }

        OnTradeSuccess?.Invoke(profit);
    }

    static float GetAcceptChance(Personality p) => p switch
    {
        Personality.Friendly => 0.80f,
        Personality.Chill => 0.60f,
        Personality.Greedy => 0.40f,
        _ => 0.20f
    };
}
