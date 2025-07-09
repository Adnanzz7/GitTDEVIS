using UnityEngine;
using Fungus;
using System.Collections.Generic;
using System.Linq;

public class RestockManager : MonoBehaviour
{
    const int MAX_STOCK = 25;
    const float MAX_CD = 300f;

    static readonly string[] itemIds =
        { Item.Wortel, Item.Tomat, Item.Kentang, Item.Cabai };

    MarketManager M => GameManager.Instance.MarketManager;

    float nextReady = 0f;
    Flowchart flow;
    PlayerManager p;

    PendingData pending;

    class PendingData
    {
        public readonly Dictionary<string, int> beli;
        public readonly int biaya;
        public PendingData(Dictionary<string, int> b, int c) { beli = b; biaya = c; }
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
        bool uangKurangMin = p.Money < HargaUnitTermurah();

        flow.SetBooleanVariable("InCooldown", cdAktif);
        flow.SetBooleanVariable("IsMoneyEmpty", uangKosong);
        flow.SetBooleanVariable("IsStockFull", stokFull);
        flow.SetBooleanVariable("IsTooPoor", uangKurangMin);

        if (cdAktif)
        {
            int sisa = Mathf.CeilToInt(nextReady - Time.time);
            flow.SetStringVariable("Line", $"Tunggu {sisa} d sebelum restock lagi.");
            flow.ExecuteBlock("RestockCooldown");
            return;
        }
        if (stokFull)
        {
            flow.SetStringVariable("Line", "Stokmu sudah penuh semua.");
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }
        if (uangKosong || uangKurangMin)
        {
            flow.SetStringVariable("Line",
                uangKosong ? "Kamu gak punya uang sama sekali"
                           : "Bahkan uangmu tak cukup beli satu unit pun");
            flow.SetBooleanVariable("MoneyEnough", false);
            flow.SetIntegerVariable("RestockedPercent", 0);
            flow.ExecuteBlock("HandleRestockResult");
            return;
        }

        flow.SetStringVariable("Line",
            $"Restock penuh butuh {HargaKekuranganPenuh()} koin.");
    }

    public void TryRestockFull()
    {
        if (Time.time < nextReady) { ShowPrice(); return; }
        flow.SetBooleanVariable("InCooldown", false);

        if (p.Money <= 0) { ShowPrice(); return; }

        var kekurangan = new Dictionary<string, int>();
        foreach (string id in itemIds)
        {
            int qty = Mathf.Clamp(MAX_STOCK - p.GetQty(id), 0, MAX_STOCK);
            kekurangan[id] = qty;
        }

        int totalKekurangan = kekurangan.Values.Sum();
        if (totalKekurangan == 0) { ShowPrice(); return; }

        var left = new Dictionary<string, int>(kekurangan);
        var beli = new Dictionary<string, int>();
        int uang = p.Money;
        int biaya = 0;
        int dibeli = 0;

        while (uang >= HargaUnitTermurah() && left.Values.Any(q => q > 0))
        {
            string target = null;
            int needMin = int.MaxValue;
            int hargaMin = int.MaxValue;

            foreach (var kv in left)
            {
                if (kv.Value == 0) continue;
                int h = M.RestockHarga(kv.Key);
                if (kv.Value < needMin || (kv.Value == needMin && h < hargaMin))
                { target = kv.Key; needMin = kv.Value; hargaMin = h; }
            }

            if (target == null || uang < hargaMin) break;

            uang -= hargaMin;
            biaya += hargaMin;
            left[target]--;
            dibeli++;
            if (!beli.ContainsKey(target)) beli[target] = 0;
            beli[target]++;
        }

        if (dibeli == 0) { ShowPrice(); return; }

        float pctReal = dibeli / (float)totalKekurangan;
        int pctInt = Mathf.RoundToInt(pctReal * 100f);
        flow.SetBooleanVariable("MoneyEnough", pctInt == 100);
        flow.SetIntegerVariable("RestockedPercent", pctInt);

        pending = new PendingData(beli, biaya);

        if (pctInt == 100) ConfirmPartialRestock();
        else
        {
            flow.SetStringVariable("Line",
                "Uangmu tidak cukup, mau pakai semua uangmu untuk Restock?");
            flow.ExecuteBlock("HandleRestockResult");
        }
    }

    public void ConfirmPartialRestock()
    {
        if (pending == null) return;

        foreach (var kv in pending.beli)
            if (kv.Value > 0) p.AddStock(kv.Key, kv.Value);

        p.Money -= pending.biaya;

        float pct = pending.biaya == 0 ? 0f : pending.biaya / (float)HargaKekuranganPenuh();
        nextReady = Time.time + Mathf.Lerp(0, MAX_CD, pct);

        pending = null;
        GameManager.Instance.UIManager?.UpdateAllBars();
        flow.SetStringVariable("Line", "Restock selesai!");
    }

    int HargaKekuranganPenuh()
    {
        int total = 0;
        foreach (string id in itemIds)
            total += Mathf.Max(0, MAX_STOCK - p.GetQty(id)) * M.RestockHarga(id);
        return total;
    }

    int HargaUnitTermurah()
    {
        int m = int.MaxValue;
        foreach (string id in itemIds)
            m = Mathf.Min(m, M.RestockHarga(id));
        return m;
    }
}
