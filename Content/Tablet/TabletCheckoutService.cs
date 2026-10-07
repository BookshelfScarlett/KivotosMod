using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using KivotosMod.Content.Tablet.Delivery;

namespace KivotosMod.Content.Tablet;

public static class TabletCheckoutService
{
    public const int MaxLines = 32;
    public const int MaxQuantityPerLine = 9999;

    public static void RequestCheckout(IReadOnlyList<TabletCartLine> lines)
    {
        if (lines == null || lines.Count == 0)
        {
            TabletUiSystem.HandleCheckoutResult(false, "购物车是空的。");
            return;
        }

        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            TabletNet.SendCheckoutRequest(lines);
            return;
        }

        bool success = TryCheckoutAndQueue(Main.LocalPlayer, lines, out string message);
        TabletUiSystem.HandleCheckoutResult(success, message);
    }

    public static bool TryCheckoutAndQueue(Player player, IReadOnlyList<TabletCartLine> lines, out string message)
    {
        message = "订单失败。";
        if (player == null || !player.active || player.dead)
        {
            message = "当前无法下单。";
            return false;
        }

        if (lines == null || lines.Count == 0 || lines.Count > MaxLines)
        {
            message = "购物车数据无效。";
            return false;
        }

        long totalCredits = 0;
        List<Item> contents = new();
        HashSet<int> seen = new();

        foreach (TabletCartLine line in lines)
        {
            if (!seen.Add(line.ItemType))
            {
                message = "购物车存在重复商品。";
                return false;
            }

            if (line.Quantity < 1 || line.Quantity > MaxQuantityPerLine)
            {
                message = "商品数量无效。";
                return false;
            }

            if (!TabletShopCatalog.TryGetByItemType(line.ItemType, out TabletShopProduct product))
            {
                message = "购物车里有已经下架的商品。";
                return false;
            }

            try
            {
                totalCredits = checked(totalCredits + (long)product.PriceCredits * line.Quantity);
            }
            catch (OverflowException)
            {
                message = "订单金额过大。";
                return false;
            }

            int totalStack;
            try
            {
                totalStack = checked(product.Stack * line.Quantity);
            }
            catch (OverflowException)
            {
                message = "商品数量过大。";
                return false;
            }

            Item template = new();
            template.SetDefaults(product.ItemType);
            int maxStack = Math.Max(1, template.maxStack);
            int remaining = totalStack;
            while (remaining > 0)
            {
                Item packed = template.Clone();
                packed.stack = Math.Min(maxStack, remaining);
                contents.Add(packed);
                remaining -= packed.stack;
            }
        }

        if (contents.Count == 0)
        {
            message = "没有可配送的商品。";
            return false;
        }

        if (!TabletWallet.TrySpend(player, totalCredits))
        {
            message = "余额不足。";
            return false;
        }

        TabletDeliveryOrderSystem.Enqueue(player, contents);
        message = "订单已确认。无人机将携带整单包裹送达。";
        return true;
    }
}
