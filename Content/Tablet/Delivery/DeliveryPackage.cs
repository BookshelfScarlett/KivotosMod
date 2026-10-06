using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KivotosMod.Content.Tablet.Delivery;

public sealed class DeliveryPackage : ModItem
{
    public override string Texture => "KivotosMod/Assets/Tablet/Delivery/DeliveryPackage";

    private List<Item> _contents = new();
    public IReadOnlyList<Item> Contents => _contents;

    public override void SetStaticDefaults()
    {
        ItemID.Sets.OpenableBag[Type] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 24;
        Item.maxStack = 1;
        Item.consumable = true;
        Item.rare = ItemRarityID.Blue;
        Item.value = 0;
    }

    public void SetContents(IEnumerable<Item> items)
    {
        _contents = new List<Item>();
        if (items == null)
            return;

        foreach (Item item in items)
        {
            if (item != null && !item.IsAir && item.stack > 0)
                _contents.Add(item.Clone());
        }
    }

    public override ModItem Clone(Item newEntity)
    {
        DeliveryPackage clone = (DeliveryPackage)base.Clone(newEntity);
        clone._contents = new List<Item>(_contents.Count);
        foreach (Item item in _contents)
            clone._contents.Add(item.Clone());
        return clone;
    }

    public override bool CanRightClick() => _contents.Count > 0;

    public override void RightClick(Player player)
    {
        if (player.whoAmI != Main.myPlayer)
            return;

        foreach (Item item in _contents)
            player.QuickSpawnItem(player.GetSource_OpenItem(Type), item, item.stack);

        _contents.Clear();
    }

    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        if (_contents.Count == 0)
            return;

        TooltipLine header = new(Mod, "DeliveryPackageContents", $"无人机包裹 · {_contents.Count} 个物品堆");
        tooltips.Add(header);

        int shown = 0;
        string icons = string.Empty;
        foreach (Item item in _contents)
        {
            if (item == null || item.IsAir)
                continue;

            icons += $"[i/s{item.stack}:{(item.ModItem == null ? item.type.ToString() : item.ModItem.FullName)}] ";
            shown++;
            if (shown >= 10)
                break;
        }

        if (!string.IsNullOrWhiteSpace(icons))
            tooltips.Add(new TooltipLine(Mod, "DeliveryPackageIcons", icons.TrimEnd()));
    }

    public override void SaveData(TagCompound tag)
    {
        if (_contents.Count > 0)
            tag["Contents"] = _contents;
    }

    public override void LoadData(TagCompound tag)
    {
        _contents = new List<Item>();
        if (!tag.ContainsKey("Contents"))
            return;

        foreach (Item item in tag.GetList<Item>("Contents"))
            _contents.Add(item);
    }

    public override void NetSend(BinaryWriter writer)
    {
        writer.Write(_contents.Count);
        foreach (Item item in _contents)
            ItemIO.Send(item, writer, true);
    }

    public override void NetReceive(BinaryReader reader)
    {
        _contents = new List<Item>();
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            _contents.Add(ItemIO.Receive(reader, true));
    }
}
