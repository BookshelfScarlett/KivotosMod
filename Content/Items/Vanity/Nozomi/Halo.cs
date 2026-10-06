using KivotosMod.Cores.Halos;
using KivotosMod.Globals.Players.Vanitys;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Nozomi
{
    public class NozomiHalo : BaseHalo
    {
        public override string HaloOwner => "Nozomi";
        public NozomiHalo(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
    public class Halo : ModItem
    {
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Orange;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            string haloName = "Nozomi";
            player.GetModPlayer<KivotosVanitysPlayer>().VanityName = haloName;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<KivotosVanitysPlayer>().VanityName = "Nozomi";
        }
    }
}
