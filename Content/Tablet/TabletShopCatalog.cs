using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet;

public enum TabletShopCategory
{
    All,
    Weapons,
    Tools,
    Armor,
    Equipment,
    Consumables,
    Ammunition,
    Tiles,
    Materials,
    Other,
}

public sealed class TabletShopProduct
{
    public int ItemType { get; }
    public string InternalName { get; }
    public int PriceCredits { get; }
    public int Stack { get; }
    public TabletShopCategory Category { get; }

    public TabletShopProduct(int itemType, string internalName, int priceCredits, TabletShopCategory category, int stack = 1)
    {
        ItemType = itemType;
        InternalName = internalName;
        PriceCredits = Math.Max(1, priceCredits);
        Category = category;
        Stack = Math.Max(1, stack);
    }
}

public static class TabletShopCatalog
{
    private static readonly List<TabletShopProduct> _products = new();
    private static readonly Dictionary<TabletShopCategory, List<TabletShopProduct>> _productsByCategory = new();

    public static IReadOnlyList<TabletShopProduct> Products => _products;

    internal static void Rebuild(Mod mod)
    {
        _products.Clear();
        _productsByCategory.Clear();

        foreach (TabletShopCategory category in Enum.GetValues(typeof(TabletShopCategory)))
            _productsByCategory[category] = new List<TabletShopProduct>();

        // Kivotos weapons.
        foreach (ModItem item in mod.GetContent<ModItem>())
        {
            string ns = item.GetType().Namespace ?? string.Empty;
            if (!ns.StartsWith("KivotosMod.Content.Items.Weapons", StringComparison.Ordinal))
                continue;

            int price = item.Item.value > 0 ? item.Item.value : 10000;
            price = Math.Max(10000, price);
            Add(new TabletShopProduct(item.Type, item.Name, price, TabletShopCategory.Weapons));
        }

        // Vanilla ammunition only for now, as requested:
        // - gun bullets (AmmoID.Bullet)
        // - launcher/rocket ammunition (AmmoID.Rocket; this includes the grenade/rocket family)
        // - standalone vanilla grenade consumables such as Grenade / StickyGrenade / BouncyGrenade / Beenade.
        for (int type = 1; type < ItemID.Count; type++)
        {
            Item sample = new();
            sample.SetDefaults(type);

            bool gunAmmo = sample.ammo == AmmoID.Bullet;
            bool launcherAmmo = sample.ammo == AmmoID.Rocket;

            // Keep this data-driven instead of hard-coding every grenade ID. This catches current
            // vanilla grenade variants without accidentally adding the Grenade Launcher itself.
            string internalName = ItemID.Search.GetName(type) ?? string.Empty;
            bool throwableGrenade = sample.consumable
                && (internalName.Contains("Grenade", StringComparison.OrdinalIgnoreCase)
                    || internalName.Contains("Beenade", StringComparison.OrdinalIgnoreCase));

            if (!gunAmmo && !launcherAmmo && !throwableGrenade)
                continue;

            int price = Math.Max(10, sample.value);
            Add(new TabletShopProduct(type, internalName, price, TabletShopCategory.Ammunition));
        }

        _products.Sort(CompareProducts);
        foreach (List<TabletShopProduct> list in _productsByCategory.Values)
            list.Sort(CompareProducts);
    }

    private static int CompareProducts(TabletShopProduct a, TabletShopProduct b)
    {
        int category = a.Category.CompareTo(b.Category);
        if (category != 0)
            return category;
        return string.Compare(a.InternalName, b.InternalName, StringComparison.OrdinalIgnoreCase);
    }

    private static void Add(TabletShopProduct product)
    {
        _products.Add(product);
        _productsByCategory[TabletShopCategory.All].Add(product);
        if (product.Category != TabletShopCategory.All)
            _productsByCategory[product.Category].Add(product);
    }

    internal static void Clear()
    {
        _products.Clear();
        _productsByCategory.Clear();
    }

    public static IReadOnlyList<TabletShopProduct> GetProducts(TabletShopCategory category)
    {
        if (_productsByCategory.TryGetValue(category, out List<TabletShopProduct> products))
            return products;
        return Array.Empty<TabletShopProduct>();
    }

    public static bool TryGetByItemType(int itemType, out TabletShopProduct product)
    {
        for (int i = 0; i < _products.Count; i++)
        {
            if (_products[i].ItemType == itemType)
            {
                product = _products[i];
                return true;
            }
        }

        product = null;
        return false;
    }
}

public sealed class TabletShopCatalogSystem : ModSystem
{
    public override void PostSetupContent()
    {
        TabletShopCatalog.Rebuild(Mod);
    }

    public override void Unload()
    {
        TabletShopCatalog.Clear();
    }
}
