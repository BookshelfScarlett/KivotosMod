using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Noa;

/// <summary>Visual-only 3-slot costume. All source frames use the vanilla 40x56
/// player sheet; arms are authored as the native 360x224 composite body texture.</summary>
public static class NoaVanitySet
{
    public static bool HeadVisible(Player player)
        => player.head == ModContent.GetInstance<NoaHead>().Item.headSlot;
    public static bool BodyVisible(Player player)
        => player.body == ModContent.GetInstance<NoaBody>().Item.bodySlot;
    public static bool BootsVisible(Player player)
        => player.legs == ModContent.GetInstance<NoaBoots>().Item.legSlot;
    public static bool FullSetVisible(Player player)
        => HeadVisible(player) && BodyVisible(player) && BootsVisible(player);
}

[AutoloadEquip(EquipType.Head)]
public sealed class NoaHead : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = false;
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = false;
        ArmorIDs.Head.Sets.PreventBeardDraw[Item.headSlot] = true;
    }
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 28;
        Item.vanity = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 1);
    }
}

[AutoloadEquip(EquipType.Body)]
public sealed class NoaBody : ModItem
{
    public override void SetStaticDefaults()
    {
        ArmorIDs.Body.Sets.HidesTopSkin[Item.bodySlot] = true;
        ArmorIDs.Body.Sets.HidesArms[Item.bodySlot] = true;
        ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = true;
    }
    public override void SetDefaults()
    {
        Item.width = 28;
        Item.height = 30;
        Item.vanity = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 1);
    }
}

[AutoloadEquip(EquipType.Legs)]
public sealed class NoaBoots : ModItem
{
    public override void SetStaticDefaults()
    {
        // Keep shoe/boot accessory layers visible, matching Yuuka's handling.
        ArmorIDs.Legs.Sets.HidesBottomSkin[Item.legSlot] = true;
    }
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.vanity = true;
        Item.rare = ItemRarityID.LightRed;
        Item.value = Item.sellPrice(gold: 1);
    }
}
