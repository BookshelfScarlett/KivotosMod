using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Noa;

/// <summary>
/// With the complete Noa set visible, suppress unrelated vanity/accessory layers so the
/// player model reads as one authored character with long side locks. Shoe/boot accessories and wings are the
/// deliberate exceptions. Held weapons/projectiles and gameplay readability layers remain.
/// </summary>
public sealed class NoaVanityPlayer : ModPlayer
{
    public override void HideDrawLayers(PlayerDrawSet drawInfo)
    {
        if (!NoaVanitySet.FullSetVisible(drawInfo.drawPlayer))
            return;

        // Hair/accessories that would contaminate the authored character silhouette.
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

        // Misc vanity/equipment visuals that overlap the body silhouette.
        PlayerDrawLayers.JimsCloak.Hide();
        PlayerDrawLayers.LeinforsHairShampoo.Hide();
        PlayerDrawLayers.Magiluminescence.Hide();
        PlayerDrawLayers.SkinLongCoat.Hide();
        PlayerDrawLayers.ArmorLongCoat.Hide();

        // Intentionally NOT hidden:
        // PlayerDrawLayers.Carpet                 -> functional Flying Carpet remains visible.
        // PlayerDrawLayers.Shoes                  -> footwear accessories may stay visible.
        // PlayerDrawLayers.Wings                  -> wings may stay visible.
        // HeldItem / ArmOverItem / ProjectileOverArm -> weapons continue to work visually.
        // Mounts, damage/debuff/effect layers     -> preserve gameplay state/readability.
    }
}
