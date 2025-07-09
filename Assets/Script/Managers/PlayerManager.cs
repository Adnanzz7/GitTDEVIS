using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerManager
{
    public event Action<int> OnMoneyChanged;
    public event Action<string, int> OnStockChanged;

    private int money;
    private Dictionary<string, int> stock = new Dictionary<string, int>();

    public int Money
    {
        get => money;
        set
        {
            if (money == value) return;
            money = Math.Max(0, value);
            OnMoneyChanged?.Invoke(money);
            GameManager.Instance.SaveManager.MarkDirty();
        }
    }

    public void AddStock(string id, int qty)
    {
        if (!stock.ContainsKey(id)) stock[id] = 0;
        stock[id] += qty;
        OnStockChanged?.Invoke(id, stock[id]);
        GameManager.Instance.SaveManager.MarkDirty();
    }

    public bool RemoveStock(string id, int qty)
    {
        if (!stock.ContainsKey(id) || stock[id] < qty) return false;
        stock[id] -= qty;
        OnStockChanged?.Invoke(id, stock[id]);
        GameManager.Instance.SaveManager.MarkDirty();
        return true;
    }

    public int GetQty(string id) => stock.TryGetValue(id, out var q) ? q : 0;

    [Serializable]
    public class SaveData
    {
        public int money;
        public List<string> itemKeys = new List<string>();
        public List<int> itemQtys = new List<int>();
    }

    public SaveData CreateSave()
    {
        var data = new SaveData { money = money };
        foreach (var kvp in stock)
        {
            data.itemKeys.Add(kvp.Key);
            data.itemQtys.Add(kvp.Value);
        }
        return data;
    }

    public void LoadSave(SaveData data)
    {
        money = data.money;
        stock.Clear();
        for (int i = 0; i < data.itemKeys.Count; i++)
            stock[data.itemKeys[i]] = data.itemQtys[i];
        OnMoneyChanged?.Invoke(money);
        foreach (var kvp in stock)
            OnStockChanged?.Invoke(kvp.Key, kvp.Value);
    }
}
