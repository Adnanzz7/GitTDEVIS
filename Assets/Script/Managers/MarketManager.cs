using UnityEngine;
using System.Collections.Generic;

public class MarketManager
{
    private Dictionary<string, int> hargaBarang = new()
    {
        { Item.Wortel,  3 },
        { Item.Tomat,   4 },
        { Item.Kentang, 2 },
        { Item.Cabai, 3 }
    };

    public int HargaSatuan(string id) => hargaBarang[id];
}
