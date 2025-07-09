using UnityEngine;
using Fungus;
using System;
using System.Collections.Generic;

public class PlayerManagerBridge : MonoBehaviour
{
    public static PlayerManagerBridge Instance;

    void Awake() => Instance = this;

    Flowchart GetFlow() => FindObjectOfType<BuyerBehaviour>()?.Flow;

    public void CheckStock_Flowchart()
    {
        try
        {
            var flow = GetFlow();
            if (!flow) return;

            string item = flow.GetStringVariable("ItemID");
            int qty = flow.GetIntegerVariable("Qty");
            var player = GameManager.Instance.PlayerManager;

            int available = player.GetQty(item);
            flow.SetIntegerVariable("StockAmount", available);

            bool hasStock = available > 0;
            bool partial = hasStock && available < qty;

            flow.SetBooleanVariable("StockAvailable", hasStock);
            flow.SetBooleanVariable("StockPartial", partial);

            string[] allIds = { Item.Wortel, Item.Tomat, Item.Kentang };
            List<string> stillHave = new();
            foreach (var id in allIds)
                if (id != item && player.GetQty(id) > 0) stillHave.Add(id);

            string alt = stillHave.Count > 0 ? string.Join(" / ", stillHave) : "barang lain";
            flow.SetStringVariable("AltItems", alt);
        }
        catch (Exception ex) { Debug.LogError("[CheckStock] " + ex); }
    }

    public void BuyerDecideAlternative()
    {
        try
        {
            var bh = FindObjectOfType<BuyerBehaviour>();
            var flow = bh?.Flow; if (!flow) return;

            string altItems = flow.GetStringVariable("AltItems");
            string[] cand = altItems.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (cand.Length == 0) { flow.SetBooleanVariable("BuyerBuyAlternative", false); return; }

            string item = cand[UnityEngine.Random.Range(0, cand.Length)].Trim();
            float chance = bh.PersonalityEnum switch
            {
                Personality.Friendly => .85f,
                Personality.Chill => .65f,
                Personality.Greedy => .50f,
                _ => .20f
            };
            bool ok = UnityEngine.Random.value < chance;
            flow.SetBooleanVariable("BuyerBuyAlternative", ok);

            if (ok)
            {
                flow.SetStringVariable("ItemID", item);
                CheckStock_Flowchart();
                int qty = flow.GetIntegerVariable("Qty");
                flow.SetStringVariable("Line", DialogBank.GetAskItem(bh.PersonalityEnum, qty, item));
                flow.ExecuteBlock("CheckStock");
            }
        }
        catch (Exception ex) { Debug.LogError("[BuyerAlt] " + ex); }
    }

    public void BuyerDecidePartialQty()
    {
        try
        {
            var bh = FindObjectOfType<BuyerBehaviour>();
            var flow = bh?.Flow; if (!flow) return;

            int avail = flow.GetIntegerVariable("StockAmount");
            if (avail <= 0) { flow.SetBooleanVariable("BuyerBuyPartial", false); return; }

            float chance = bh.PersonalityEnum switch
            {
                Personality.Friendly => .85f,
                Personality.Chill => .65f,
                Personality.Greedy => .45f,
                _ => .20f
            };
            bool ok = UnityEngine.Random.value < chance;
            flow.SetBooleanVariable("BuyerBuyPartial", ok);

            if (ok)
            {
                flow.SetIntegerVariable("Qty", avail);
                flow.SetStringVariable("Line", DialogBank.GetPartialAccept(bh.PersonalityEnum));
            }
            else flow.SetStringVariable("Line", DialogBank.GetPartialReject(bh.PersonalityEnum));
        }
        catch (Exception ex) { Debug.LogError("[BuyerPartial] " + ex); }
    }

    public void Tawarkan()
    {
        try
        {
            var buyer = FindObjectOfType<BuyerBehaviour>(); if (!buyer) return;
            var flow = buyer.Flow; if (!flow) return;
            int offer = flow.GetIntegerVariable("OfferPrice");
            GameManager.Instance.TradeManager.TawarkanNormal(offer, buyer);
        }
        catch (Exception ex) { Debug.LogError("[Tawarkan] " + ex); }
    }

    public void HargaDaruratDanTawar()
    {
        try
        {
            var buyer = FindObjectOfType<BuyerBehaviour>(); if (!buyer) return;
            var flow = buyer.Flow; if (!flow) return;
            int price = flow.GetIntegerVariable("BuyerOffer");
            flow.SetIntegerVariable("OfferPrice", price);
            GameManager.Instance.TradeManager.TawarkanDarurat(price, buyer);
        }
        catch (Exception ex) { Debug.LogError("[DaruratTawar] " + ex); }
    }

    int GetCounterPrice(int offer, int min, int max, Personality p)
    {
        float k = p switch
        {
            Personality.Friendly => .6f,
            Personality.Chill => .55f,
            Personality.Greedy => .5f,
            _ => .45f
        };
        return Mathf.Clamp(Mathf.RoundToInt(Mathf.Lerp(min, offer, k)), min, max);
    }

    public void HitungBuyerCounter()
    {
        try
        {
            var flow = GetFlow();
            var buyer = FindObjectOfType<BuyerBehaviour>();
            if (!flow || !buyer) return;

            int offer = flow.GetIntegerVariable("OfferPrice");
            var tm = GameManager.Instance.TradeManager;
            int buyerOffer = GetCounterPrice(offer, tm.GetHargaMinimal(), tm.GetHargaMaxPembeli(), buyer.PersonalityEnum);

            if (offer >= 8) buyerOffer = UnityEngine.Random.Range(offer - 3, offer);
            else if (offer >= 6) buyerOffer = UnityEngine.Random.Range(offer - 2, offer);
            else buyerOffer = Mathf.Max(1, offer - 1);

            flow.SetIntegerVariable("BuyerOffer", buyerOffer);
        }
        catch (Exception ex) { Debug.LogError("[HitungCounter] " + ex); }
    }

    public void LebihkanSedikit()
    {
        try
        {
            var bh = FindObjectOfType<BuyerBehaviour>(); if (!bh) return;
            var flow = bh.Flow; if (!flow) return;

            int buyerOffer = flow.GetIntegerVariable("BuyerOffer");
            int market = flow.GetIntegerVariable("MarketPrice");
            flow.SetIntegerVariable("OfferPrice", Mathf.Max(buyerOffer + 1, market + 1));
            bh.AdjustMood(-5);
        }
        catch (Exception ex) { Debug.LogError("[Lebihkan] " + ex); }
    }

    public void LebihkanDanTawar()
    {
        LebihkanSedikit();
        Tawarkan();
    }

    public void CobaEmergencyDanTawar()
    {
        try
        {
            var bh = FindObjectOfType<BuyerBehaviour>(); if (!bh) return;
            var flow = bh.Flow; if (!flow) return;

            float chance = bh.PersonalityEnum switch
            {
                Personality.Friendly => .80f,
                Personality.Chill => .60f,
                Personality.Greedy => .50f,
                _ => .30f
            };
            bool ok = UnityEngine.Random.value < chance;
            flow.SetBooleanVariable("EmergencyAccepted", ok);

            int price = flow.GetIntegerVariable("BuyerOffer");
            flow.SetIntegerVariable("OfferPrice", price);

            if (ok) GameManager.Instance.TradeManager.TawarkanDarurat(price, bh);
            else GameManager.Instance.TradeManager.TolakDarurat(bh);
        }
        catch (Exception ex) { Debug.LogError("[EmergencyTawar] " + ex); }
    }
}
