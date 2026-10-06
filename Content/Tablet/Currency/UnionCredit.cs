using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace KivotosMod.Content.Tablet.Currency;

public class UnionCredit : ModItem
{
    public override LocalizedText DisplayName => Language.GetOrRegister("Mods.KivotosMod.Items.UnionCredit.DisplayName", () => "联合币");
    public override LocalizedText Tooltip => Language.GetOrRegister("Mods.KivotosMod.Items.UnionCredit.Tooltip", () => "1 联合币 = 1 铜币");

    public override string Texture => "KivotosMod/Assets/Tablet/Currency/UnionCredit";

    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 14;
        Item.maxStack = 9999;
        Item.value = Item.buyPrice(copper: 1);
        Item.rare = ItemRarityID.White;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.CopperCoin).Register();
        Recipe.Create(ItemID.CopperCoin).AddIngredient<UnionCredit>().Register();
    }
}
