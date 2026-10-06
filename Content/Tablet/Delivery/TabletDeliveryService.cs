using System.Collections.Generic;
using Terraria;

namespace KivotosMod.Content.Tablet.Delivery;

public static class TabletDeliveryService
{
    public static int QueueOrder(Player player, List<Item> contents)
    {
        return TabletDeliveryOrderSystem.Enqueue(player, contents);
    }
}
