using UnityEngine;
using Fungus;
using System.Collections.Generic;
using System.Linq;

public class RestockManager : MonoBehaviour
{
    const int MAX_STOCK = 25;
    const float MAX_CD = 300f;

    readonly Dictionary<string, int> harga = new()
    {
        { Item.Wortel,  3 },
        { Item.Tomat,   4 },
        { Item.Kentang, 2 },
        { Item.Cabai,   3 }
    };

    float nextReady = 0f;
    Flowchart flow;
    PlayerManager p;

    PendingData pending;

    class PendingData
    {
        public readonly Dictionary<string, int> beli;
        public readonly int biaya;

        public PendingData(Dictionary<string, int> b, int c)
        {
            beli = b;
            biaya = c;
        }
    }

    void Awake()
    {
        flow = GetComponent<Flowchart>();
        p = GameManager.Instance.PlayerManager;
    }

    public void ShowPrice()
    {
        bool cdAktif = Time.time < nextReady;
        bool uangKosong = p.Money <= 0;
        bool stokFull = HargaKekuranganPenuh() == 0;
        bool uangKurangMinimum = p.Money < HargaUnitTermurah();

        flow.SetBooleanVariable("InCooldown", cdAktif);
        flow.SetBooleanVariable("IsMoneyEmpty", uangKosong);
        flow.SetBooleanVariable("IsStockFull", stokFull);
        flow.SetBooleanVariable("IsTooPoor", uangKurangMinimum);

        if (cdAktif)
        {
            int sisa = Mathf.CeilToInt(nextReady - Time.time);
            flow.SetStringVariable("Line", $"Tunggu {sisa} detik sebelum restock lagi.");
            flow.ExecuteBlock("RestockCooldown");
            return;
        }

        if (stokFull)
        {
            flow.SetStringVariable("Line", "Stokmu sudah penuh semua.");
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }

        if (uangKosong)
        {
            flow.SetStringVariable("Line", "Kamu gak punya uang sama sekali");
            flow.SetBooleanVariable("MoneyEnough", false);
            flow.SetIntegerVariable("RestockedPercent", 0);
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }

        if (uangKurangMinimum)
        {
            flow.SetStringVariable("Line", "Bahkan uangmu tak cukup beli satu unit pun");
            flow.SetBooleanVariable("MoneyEnough", false);
            flow.SetIntegerVariable("RestockedPercent", 0);
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }

        int biayaFull = HargaKekuranganPenuh();
        flow.SetStringVariable("Line", $"Restock penuh butuh {biayaFull} koin.");
    }

    public void TryRestockFull()
    {
        if (Time.time < nextReady) { ShowPrice(); return; }
        flow.SetBooleanVariable("InCooldown", false);

        if (p.Money <= 0)
        {
            flow.SetStringVariable("Line", "Kamu gak punya uang sama sekali");
            flow.SetBooleanVariable("MoneyEnough", false);
            flow.SetIntegerVariable("RestockedPercent", 0);
            flow.ExecuteBlock("HandleRestockResult");
            pending = null;
            return;
        }

        int totalKekurangan = 0;
        var kekurangan = new Dictionary<string, int>();
        foreach (var kv in harga)
        {
            int qty = MAX_STOCK - p.GetQty(kv.Key);
            qty = Mathf.Max(0, qty);
            kekurangan[kv.Key] = qty;
            totalKekurangan += qty;
        }

        if (totalKekurangan == 0)
        {
            flow.SetStringVariable("Line", "Stokmu sudah penuh semua.");
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }

        Dictionary<string, int> left = new(kekurangan);
        Dictionary<string, int> beli = new();
        int uang = p.Money;
        int biaya = 0;
        int dibeli = 0;

        while (uang >= HargaUnitTermurah() && left.Values.Any(q => q > 0))
        {
            string target = null;
            int minQty = int.MaxValue;
            int minHrg = int.MaxValue;

            foreach (var kv in left)
            {
                int need = kv.Value;
                if (need == 0) continue;
                int h = harga[kv.Key];
                if (need < minQty || (need == minQty && h < minHrg))
                {
                    target = kv.Key;
                    minQty = need;
                    minHrg = h;
                }
            }

            if (target == null || uang < minHrg) break;

            uang -= minHrg;
            biaya += minHrg;
            left[target]--;
            dibeli++;

            if (!beli.ContainsKey(target)) beli[target] = 0;
            beli[target]++;
        }

        if (dibeli == 0)
        {
            flow.SetStringVariable("Line", "Uangmu bahkan tak cukup beli satu unit pun");
            flow.SetBooleanVariable("MoneyEnough", false);
            flow.SetIntegerVariable("RestockedPercent", 0);
            flow.ExecuteBlock("HandleRestockResult");
            pending = null;
            return;
        }

        float pctReal = dibeli / (float)totalKekurangan;
        int pctInt = Mathf.RoundToInt(pctReal * 100f);

        flow.SetBooleanVariable("MoneyEnough", pctInt == 100);
        flow.SetIntegerVariable("RestockedPercent", pctInt);

        pending = new PendingData(beli, biaya);

        if (pctInt == 100)
        {
            ConfirmPartialRestock();
            return;
        }

        flow.SetStringVariable("Line", $"Uangmu tidak cukup, mau pakai semua uangmu untuk Restock?");
        flow.ExecuteBlock("HandleRestockResult");
    }

    public void ConfirmPartialRestock()
    {
        if (pending == null) return;

        foreach (var kv in pending.beli)
            if (kv.Value > 0) p.AddStock(kv.Key, kv.Value);

        p.Money -= pending.biaya;

        int totalKekurangan = 0;
        foreach (var kv in harga)
            totalKekurangan += Mathf.Max(0, MAX_STOCK - p.GetQty(kv.Key));

        int totalDibeli = 0;
        foreach (var v in pending.beli.Values)
            totalDibeli += v;

        float pct = totalKekurangan == 0 ? 0f : totalDibeli / (float)totalKekurangan;
        nextReady = Time.time + Mathf.Lerp(0, MAX_CD, pct);

        pending = null;
        GameManager.Instance.UIManager?.UpdateAllBars();
        flow.SetStringVariable("Line", "Restock selesai!");
    }

    int HargaKekuranganPenuh()
    {
        int t = 0;
        foreach (var kv in harga)
            t += Mathf.Max(0, MAX_STOCK - p.GetQty(kv.Key)) * kv.Value;
        return t;
    }

    int HargaUnitTermurah()
    {
        int m = int.MaxValue;
        foreach (int v in harga.Values) if (v < m) m = v;
        return m;
    }
}
