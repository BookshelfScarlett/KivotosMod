using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Yuuka;

/// <summary>
/// Yuuka's rear hair is authored as a normal 40x1120 player-frame sheet.
/// It is kept separate from the head equip so it can sit behind wings/body/head correctly.
/// </summary>
[Autoload(Side = ModSide.Client)]
public sealed class YuukaBackHairLayer : PlayerDrawLayer
{
    private static Asset<Texture2D>? _texture;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead
           && !drawInfo.drawPlayer.ghost
           && YuukaVanitySet.HeadVisible(drawInfo.drawPlayer);

    // Wings are one of the very few external visuals intentionally kept with the full outfit.
    // Draw rear hair after wings so the hair remains part of Yuuka's silhouette instead of being
    // covered by a wing texture crossing the back of the head.
    public override Position GetDefaultPosition()
        => new Between(PlayerDrawLayers.Wings, PlayerDrawLayers.BackAcc);

    public override bool IsHeadLayer => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        _texture ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Yuuka/YuukaBackHair",
            AssetRequestMode.ImmediateLoad);

        Vector2 position = drawInfo.helmetOffset
            + new Vector2(
                (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f))
            + player.headPosition
            + drawInfo.headVect;

        DrawData data = new(
            _texture.Value,
            position,
            player.bodyFrame,
            drawInfo.colorArmorHead,
            player.headRotation,
            drawInfo.headVect,
            1f,
            drawInfo.playerEffect,
            0f)
        {
            shader = drawInfo.cHead
        };

        drawInfo.DrawDataCache.Add(data);
    }
}

/// <summary>
/// Draws the supplied legacy 40x1120 torso and coat sheets immediately before Terraria's
/// native composite-body layer. The native YuukaBody_Body texture contains the supplied
/// 360x224 arm atlas, so item use/aiming/composite-arm poses stay handled by Terraria itself.
/// </summary>
[Autoload(Side = ModSide.Client)]
public sealed class YuukaBodyBaseLayer : PlayerDrawLayer
{
    private static Asset<Texture2D>? _torso;
    private static Asset<Texture2D>? _coat;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead
           && !drawInfo.drawPlayer.ghost
           && YuukaVanitySet.BodyVisible(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new BeforeParent(PlayerDrawLayers.Torso);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        _torso ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Yuuka/YuukaTorso",
            AssetRequestMode.ImmediateLoad);
        _coat ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Yuuka/YuukaCoat",
            AssetRequestMode.ImmediateLoad);

        Rectangle source = player.bodyFrame;
        Vector2 position = new(
            (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f));
        position += player.bodyPosition + new Vector2(player.bodyFrame.Width / 2f, player.bodyFrame.Height / 2f);

        AddBodyFrame(ref drawInfo, _torso.Value, position, source);
        AddBodyFrame(ref drawInfo, _coat.Value, position, source);
    }

    private static void AddBodyFrame(ref PlayerDrawSet drawInfo, Texture2D texture, Vector2 position, Rectangle source)
    {
        Player player = drawInfo.drawPlayer;
        DrawData data = new(
            texture,
            position,
            source,
            drawInfo.colorArmorBody,
            player.bodyRotation,
            drawInfo.bodyVect,
            1f,
            drawInfo.playerEffect,
            0f)
        {
            shader = drawInfo.cBody
        };

        drawInfo.DrawDataCache.Add(data);
    }
}
