using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace KivotosMod.Content.Tablet.Currency;

public class Pyroxene : ModItem
{
    public override LocalizedText DisplayName => Language.GetOrRegister("Mods.KivotosMod.Items.Pyroxene.DisplayName", () => "青辉石");
    public override LocalizedText Tooltip => Language.GetOrRegister("Mods.KivotosMod.Items.Pyroxene.Tooltip", () => "1 青辉石 = 10000 联合币 = 1 金币");

    public override string Texture => "KivotosMod/Assets/Tablet/Currency/Pyroxene";

    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 30;
        Item.maxStack = 9999;
        Item.value = Item.buyPrice(gold: 1);
        Item.rare = ItemRarityID.LightPurple;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.GoldCoin).Register();
        Recipe.Create(ItemID.GoldCoin).AddIngredient<Pyroxene>().Register();
    }
}
