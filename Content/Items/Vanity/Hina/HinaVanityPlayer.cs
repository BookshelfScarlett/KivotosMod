using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Hina;

public sealed class HinaVanityPlayer : ModPlayer
{
    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        if (!HinaVanitySet.FullSetVisible(drawInfo.drawPlayer))
            return;

        PlayerDrawLayers.HairBack.Hide();
        PlayerDrawLayers.BackAcc.Hide();
        PlayerDrawLayers.Backpacks.Hide();
        PlayerDrawLayers.BalloonAcc.Hide();
        PlayerDrawLayers.FaceAcc.Hide();
        PlayerDrawLayers.FrontAccBack.Hide();
        PlayerDrawLayers.FrontAccFront.Hide();
        PlayerDrawLayers.HandOnAcc.Hide();
        PlayerDrawLayers.NeckAcc.Hide();
        PlayerDrawLayers.OffhandAcc.Hide();
        PlayerDrawLayers.Shield.Hide();
        PlayerDrawLayers.SolarShield.Hide();
        PlayerDrawLayers.Tails.Hide();
        PlayerDrawLayers.WaistAcc.Hide();
        PlayerDrawLayers.Wings.Hide();

        PlayerDrawLayers.JimsCloak.Hide();
        PlayerDrawLayers.LeinforsHairShampoo.Hide();
        PlayerDrawLayers.Magiluminescence.Hide();
        PlayerDrawLayers.SkinLongCoat.Hide();
        PlayerDrawLayers.ArmorLongCoat.Hide();
        // Flying Carpet and Shoes stay visible on purpose.
    }
}
