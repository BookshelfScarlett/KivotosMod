using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using KivotosMod.Content.Tablet.Currency;

namespace KivotosMod.Content.Tablet;

public static class TabletWallet
{
    public const long CreditsPerCopper = 1;
    public const long CreditsPerSilver = 100;
    public const long CreditsPerGold = 10000;
    public const long CreditsPerPyroxene = 10000;
    public const long CreditsPerPlatinum = 1000000;

    public static long GetTotalCredits(Player player)
    {
        long total = 0;
        foreach (Item item in player.inventory)
            total += GetUnitValue(item.type) * item.stack;
        return total;
    }

    public static long GetPyroxeneEquivalent(Player player) => GetTotalCredits(player) / CreditsPerPyroxene;

    public static long GetUnitValue(int itemType)
    {
        if (itemType == ModContent.ItemType<UnionCredit>()) return 1;
        if (itemType == ItemID.CopperCoin) return CreditsPerCopper;
        if (itemType == ItemID.SilverCoin) return CreditsPerSilver;
        if (itemType == ItemID.GoldCoin) return CreditsPerGold;
        if (itemType == ModContent.ItemType<Pyroxene>()) return CreditsPerPyroxene;
        if (itemType == ItemID.PlatinumCoin) return CreditsPerPlatinum;
        return 0;
    }

    public static bool TrySpend(Player player, long amount)
    {
        if (amount <= 0) return true;
        if (GetTotalCredits(player) < amount) return false;

        long paid = 0;
        int[] denominationTypes =
        {
            ModContent.ItemType<UnionCredit>(),
            ItemID.CopperCoin,
            ItemID.SilverCoin,
            ItemID.GoldCoin,
            ModContent.ItemType<Pyroxene>(),
            ItemID.PlatinumCoin,
        };

        foreach (int type in denominationTypes)
        {
            long value = GetUnitValue(type);
            if (value <= 0) continue;

            for (int i = 0; i < player.inventory.Length && paid < amount; i++)
            {
                Item item = player.inventory[i];
                if (item.type != type || item.stack <= 0) continue;

                long remaining = amount - paid;
                long needed = (remaining + value - 1) / value;
                int take = (int)Math.Min(item.stack, needed);
                if (take <= 0) continue;

                item.stack -= take;
                paid += take * value;
                if (item.stack <= 0)
                    item.TurnToAir();
            }
        }

        long change = paid - amount;
        if (change > 0)
            GiveUnionCreditChange(player, change);

        if (Main.netMode == NetmodeID.Server)
        {
            for (int i = 0; i < player.inventory.Length; i++)
                NetMessage.SendData(MessageID.SyncEquipment, -1, -1, null, player.whoAmI, i);
        }

        return true;
    }

    private static void GiveUnionCreditChange(Player player, long change)
    {
        // Return large change as Pyroxene and the remainder as Union Credits.
        // Example: paying 1 gold (10,000) for a 5,000-credit order returns 5,000 Union Credits.
        int pyroxeneType = ModContent.ItemType<Pyroxene>();
        int creditType = ModContent.ItemType<UnionCredit>();

        long pyroxene = change / CreditsPerPyroxene;
        long credits = change % CreditsPerPyroxene;

        while (pyroxene > 0)
        {
            int stack = (int)Math.Min(pyroxene, 9999);
            player.QuickSpawnItem(player.GetSource_Misc("KivotosTabletChange"), pyroxeneType, stack);
            pyroxene -= stack;
        }

        while (credits > 0)
        {
            int stack = (int)Math.Min(credits, 9999);
            player.QuickSpawnItem(player.GetSource_Misc("KivotosTabletChange"), creditType, stack);
            credits -= stack;
        }
    }
}
