using UnityEngine;
using System.Collections.Generic;

public class MarketManager
{
    readonly Dictionary<string, int> basePrice = new() {
        { Item.Wortel, 3 }, { Item.Tomat, 4 },
        { Item.Kentang, 2 }, { Item.Cabai, 3 } };

    readonly Dictionary<string, int> todayPrice = new();
    readonly Dictionary<string, int> yesterday = new();

    public IReadOnlyDictionary<string, int> Today => todayPrice;
    public IReadOnlyDictionary<string, int> Yesterday => yesterday;

    public MarketManager() => RollDailyPrices();

    public void RollDailyPrices()
    {
        yesterday.Clear();
        foreach (var kv in todayPrice) yesterday[kv.Key] = kv.Value;

        todayPrice.Clear();
        foreach (var kv in basePrice)
        {
            int delta = UnityEngine.Random.Range(-1, 2);
            todayPrice[kv.Key] = Mathf.Max(1, kv.Value + delta);
        }
    }

    public int HargaSatuan(string id) => todayPrice[id];
    public int RestockHarga(string id) => Mathf.Max(1, HargaSatuan(id) - 1);
    public int Delta(string id)
    {
        if (!yesterday.ContainsKey(id)) return 0;
        return todayPrice[id] - yesterday[id];
    }
}
