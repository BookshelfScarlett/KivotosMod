using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;

namespace KivotosMod.Content.Tablet;

internal enum TabletNetMessage : byte
{
    CheckoutRequest = 1,
    CheckoutResult = 2,
}

public static class TabletNet
{
    public static void SendCheckoutRequest(IReadOnlyList<TabletCartLine> lines)
    {
        if (lines == null)
            return;

        var packet = global::KivotosMod.KivotosMod.Instance.GetPacket();
        packet.Write((byte)TabletNetMessage.CheckoutRequest);
        packet.Write((byte)System.Math.Min(lines.Count, TabletCheckoutService.MaxLines));
        for (int i = 0; i < lines.Count && i < TabletCheckoutService.MaxLines; i++)
        {
            packet.Write(lines[i].ItemType);
            packet.Write(lines[i].Quantity);
        }
        packet.Send();
    }

    private static void SendCheckoutResult(int toClient, bool success, string message)
    {
        var packet = global::KivotosMod.KivotosMod.Instance.GetPacket();
        packet.Write((byte)TabletNetMessage.CheckoutResult);
        packet.Write(success);
        packet.Write(message ?? string.Empty);
        packet.Send(toClient);
    }

    public static void HandlePacket(BinaryReader reader, int whoAmI)
    {
        TabletNetMessage message = (TabletNetMessage)reader.ReadByte();
        switch (message)
        {
            case TabletNetMessage.CheckoutRequest:
            {
                if (Main.netMode != NetmodeID.Server)
                    return;
                if (whoAmI < 0 || whoAmI >= Main.maxPlayers)
                    return;

                int count = reader.ReadByte();
                if (count <= 0 || count > TabletCheckoutService.MaxLines)
                {
                    SendCheckoutResult(whoAmI, false, "购物车数据无效。");
                    return;
                }

                List<TabletCartLine> lines = new(count);
                for (int i = 0; i < count; i++)
                    lines.Add(new TabletCartLine(reader.ReadInt32(), reader.ReadInt32()));

                Player player = Main.player[whoAmI];
                bool success = TabletCheckoutService.TryCheckoutAndQueue(player, lines, out string resultMessage);
                SendCheckoutResult(whoAmI, success, resultMessage);
                break;
            }

            case TabletNetMessage.CheckoutResult:
            {
                if (Main.netMode == NetmodeID.Server)
                    return;
                bool success = reader.ReadBoolean();
                string resultMessage = reader.ReadString();
                TabletUiSystem.HandleCheckoutResult(success, resultMessage);
                break;
            }
        }
    }
}
