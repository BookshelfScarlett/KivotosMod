using System;
using System.Collections.Generic;

namespace KivotosMod.Content.Tablet;

public readonly struct TabletCartLine
{
    public int ItemType { get; }
    public int Quantity { get; }

    public TabletCartLine(int itemType, int quantity)
    {
        ItemType = itemType;
        Quantity = quantity;
    }
}

public static class TabletCart
{
    private const int MaxQuantityPerLine = 9999;
    private static readonly Dictionary<int, int> _quantities = new();

    public static int LineCount => _quantities.Count;
    public static int ItemCount
    {
        get
        {
            int total = 0;
            foreach (int quantity in _quantities.Values)
                total += quantity;
            return total;
        }
    }

    public static bool IsEmpty => _quantities.Count == 0;

    public static int GetQuantity(int itemType) => _quantities.TryGetValue(itemType, out int quantity) ? quantity : 0;

    public static void Add(int itemType, int quantity)
    {
        if (quantity <= 0 || !TabletShopCatalog.TryGetByItemType(itemType, out _))
            return;

        int current = GetQuantity(itemType);
        _quantities[itemType] = Math.Clamp(current + quantity, 1, MaxQuantityPerLine);
    }

    public static void SetQuantity(int itemType, int quantity)
    {
        if (!_quantities.ContainsKey(itemType))
            return;

        if (quantity <= 0)
        {
            _quantities.Remove(itemType);
            return;
        }

        _quantities[itemType] = Math.Clamp(quantity, 1, MaxQuantityPerLine);
    }

    public static void Adjust(int itemType, int delta)
    {
        if (!_quantities.TryGetValue(itemType, out int current))
            return;

        SetQuantity(itemType, current + delta);
    }

    public static void Remove(int itemType) => _quantities.Remove(itemType);

    public static void Clear() => _quantities.Clear();

    public static long GetTotalCredits()
    {
        long total = 0;
        foreach (KeyValuePair<int, int> pair in _quantities)
        {
            if (!TabletShopCatalog.TryGetByItemType(pair.Key, out TabletShopProduct product))
                continue;

            try
            {
                total = checked(total + (long)product.PriceCredits * pair.Value);
            }
            catch (OverflowException)
            {
                return long.MaxValue;
            }
        }
        return total;
    }

    public static List<TabletCartLine> Snapshot()
    {
        List<TabletCartLine> lines = new(_quantities.Count);
        foreach (KeyValuePair<int, int> pair in _quantities)
            lines.Add(new TabletCartLine(pair.Key, pair.Value));
        lines.Sort((a, b) => a.ItemType.CompareTo(b.ItemType));
        return lines;
    }
}
