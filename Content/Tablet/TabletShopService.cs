using Terraria;

namespace KivotosMod.Content.Tablet;

// Compatibility shim for source folders that previously contained the pre-cart TabletShopService.
// V4's real purchase flow is TabletCart -> TabletCheckoutService -> drone delivery.
public static class TabletShopService
{
    public static void RequestPurchase(int legacyValue)
    {
        if (!TryResolveLegacyValue(legacyValue, out int itemType))
            return;

        TabletCart.Add(itemType, 1);

        if (Main.netMode != Terraria.ID.NetmodeID.Server && Main.myPlayer >= 0)
            Main.NewText("已加入购物车。", new Microsoft.Xna.Framework.Color(120, 220, 255));
    }

    // Kept only so stale callers from V1-V3 source trees still compile.
    // It intentionally DOES NOT charge currency or dispatch a drone directly.
    public static bool TryPurchaseAndDispatch(Player player, int legacyValue)
    {
        if (player == null || !player.active || player.dead)
            return false;
        if (!TryResolveLegacyValue(legacyValue, out int itemType))
            return false;

        // Cart state is client-side UI state. Legacy server/direct-purchase callers are rejected
        // rather than bypassing the V4 server-validated checkout path.
        if (Main.netMode == Terraria.ID.NetmodeID.Server || player.whoAmI != Main.myPlayer)
            return false;

        TabletCart.Add(itemType, 1);
        return true;
    }

    private static bool TryResolveLegacyValue(int legacyValue, out int itemType)
    {
        // V2/V3 passed an actual item type.
        if (TabletShopCatalog.TryGetByItemType(legacyValue, out _))
        {
            itemType = legacyValue;
            return true;
        }

        // Early V1 builds passed a catalog index. Supporting both makes overwrite upgrades safe.
        if (legacyValue >= 0 && legacyValue < TabletShopCatalog.Products.Count)
        {
            itemType = TabletShopCatalog.Products[legacyValue].ItemType;
            return true;
        }

        itemType = 0;
        return false;
    }
}
