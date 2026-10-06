using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Yuuka;

/// <summary>
/// Three-slot Yuuka vanity set. The visible character is assembled from multiple art layers:
/// head item = head/front hair + rear hair; body item = legacy torso + composite arms + coat;
/// boots item = legs. Only three actual equip items are registered.
/// </summary>
public static class YuukaVanitySet
{
    public static bool HeadVisible(Player player)
        => player.head == ModContent.GetInstance<YuukaHead>().Item.headSlot;

    public static bool BodyVisible(Player player)
        => player.body == ModContent.GetInstance<YuukaBody>().Item.bodySlot;

    public static bool BootsVisible(Player player)
        => player.legs == ModContent.GetInstance<YuukaBoots>().Item.legSlot;

    public static bool FullSetVisible(Player player)
        => HeadVisible(player) && BodyVisible(player) && BootsVisible(player);
}

[AutoloadEquip(EquipType.Head)]
public sealed class YuukaHead : ModItem
{
    public override void SetStaticDefaults()
    {
        // YuukaHead_Head contains the complete custom head/front-hair drawing.
        // Keep the custom equip texture, but suppress the vanilla face and hair underneath it.
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
public sealed class YuukaBody : ModItem
{
    public override void SetStaticDefaults()
    {
        // The native body equip texture is the supplied 360x224 composite arm sheet.
        // The torso and coat are drawn by YuukaBodyBaseLayer behind those arms.
        // Hide all vanilla upper-body skin so it cannot leak through the costume.
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
public sealed class YuukaBoots : ModItem
{
    public override void SetStaticDefaults()
    {
        // Hide vanilla leg skin. Do not set OverridesLegs: visible shoe/boot accessories
        // are intentionally allowed to render over this outfit.
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
