using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet.Delivery;

public sealed class TabletDeliveryOrderSystem : ModSystem
{
    private sealed class DeliveryOrder
    {
        public int Owner;
        public Item Package;
        public int Cooldown;
        public bool InFlight;
    }

    private static readonly Dictionary<int, DeliveryOrder> Orders = new();
    private static int _nextOrderId = 1;

    public static int Enqueue(Player player, List<Item> contents)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return -1;

        Item package = new();
        package.SetDefaults(ModContent.ItemType<DeliveryPackage>());
        package.stack = 1;
        if (package.ModItem is DeliveryPackage deliveryPackage)
            deliveryPackage.SetContents(contents);

        int id = _nextOrderId++;
        if (_nextOrderId <= 0)
            _nextOrderId = 1;

        Orders[id] = new DeliveryOrder
        {
            Owner = player.whoAmI,
            Package = package,
            Cooldown = 30,
            InFlight = false,
        };
        return id;
    }

    public override void PostUpdatePlayers()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || Orders.Count == 0)
            return;

        List<int> ids = new(Orders.Keys);
        foreach (int id in ids)
        {
            if (!Orders.TryGetValue(id, out DeliveryOrder order))
                continue;
            if (order.Cooldown > 0)
                order.Cooldown--;
        }

        for (int playerIndex = 0; playerIndex < Main.maxPlayers; playerIndex++)
        {
            Player player = Main.player[playerIndex];
            if (player == null || !player.active || player.dead || player.ghost)
                continue;

            int droneType = ModContent.ProjectileType<DeliveryDroneProjectile>();
            if (player.ownedProjectileCounts[droneType] > 0)
                continue;

            int chosenId = -1;
            foreach (KeyValuePair<int, DeliveryOrder> pair in Orders)
            {
                DeliveryOrder order = pair.Value;
                if (order.Owner != playerIndex || order.Cooldown > 0 || order.InFlight)
                    continue;
                if (chosenId == -1 || pair.Key < chosenId)
                    chosenId = pair.Key;
            }

            if (chosenId < 0 || !Orders.TryGetValue(chosenId, out DeliveryOrder chosen))
                continue;

            Vector2 spawn = player.Center + new Vector2(0f, -Main.rand.Next(2000, 4001)).RotatedByRandom(MathHelper.ToRadians(75f));
            spawn.X = Math.Clamp(spawn.X, Main.leftWorld + 64f, Main.rightWorld - 64f);
            spawn.Y = Math.Clamp(spawn.Y, Main.topWorld + 64f, Main.bottomWorld - 64f);

            Projectile.NewProjectile(
                player.GetSource_Misc("KivotosTabletDroneDelivery"),
                spawn,
                Vector2.Zero,
                droneType,
                0,
                0f,
                player.whoAmI,
                chosenId,
                chosen.Package.type,
                0f);

            chosen.InFlight = true;
        }
    }

    public static bool TryDropPackage(int orderId, Vector2 position)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return false;
        if (!Orders.TryGetValue(orderId, out DeliveryOrder order))
            return false;
        if (order.Owner < 0 || order.Owner >= Main.maxPlayers)
            return false;

        Player player = Main.player[order.Owner];
        if (player == null)
            return false;

        int worldItem = Item.NewItem(
            player.GetSource_Misc("KivotosTabletDroneDelivery"),
            position,
            order.Package);

        if (worldItem >= 0 && worldItem < Main.maxItems)
            Main.item[worldItem].velocity = Vector2.Zero;

        Orders.Remove(orderId);
        return true;
    }

    public static void NotifyDroneEnded(int orderId, bool delivered)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;
        if (delivered)
            return;
        if (!Orders.TryGetValue(orderId, out DeliveryOrder order))
            return;

        order.InFlight = false;
        order.Cooldown = 90;
    }

    public override void OnWorldUnload()
    {
        Orders.Clear();
        _nextOrderId = 1;
    }

    public override void Unload()
    {
        Orders.Clear();
        _nextOrderId = 1;
    }
}
