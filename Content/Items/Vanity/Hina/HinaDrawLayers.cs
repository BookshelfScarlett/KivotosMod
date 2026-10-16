using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Hina;

[Autoload(Side = ModSide.Client)]
public sealed class HinaWingLayer : PlayerDrawLayer
{
    private static Asset<Texture2D>? _wings;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead
           && !drawInfo.drawPlayer.ghost
           && HinaVanitySet.FullSetVisible(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new Between(PlayerDrawLayers.Wings, PlayerDrawLayers.BackAcc);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        _wings ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Hina/HinaWings",
            AssetRequestMode.ImmediateLoad);

        Texture2D texture = _wings.Value;
        int frameHeight = texture.Height / 20;
        Rectangle source = new(0, player.bodyFrame.Y, texture.Width, frameHeight);
        Vector2 position = new(
            (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f));
        position += player.bodyPosition + new Vector2(player.bodyFrame.Width / 2f, player.bodyFrame.Height / 2f);

        // Hina's integrated wings are authored at 50x56 per frame, while the rest of the
        // player-format outfit uses the standard 40x56 frame width. Without compensating for
        // that extra width, the wing sheet reads visually offset from the body. The offset
        // must also follow horizontal facing/flip: when the player faces right we shift the
        // wings left by half of the width delta, and when facing left (flipped) we shift them
        // right by that same amount.
        float horizontalCompensation = (texture.Width - player.bodyFrame.Width) * 0.5f;
        if ((drawInfo.playerEffect & SpriteEffects.FlipHorizontally) != 0)
            position.X += horizontalCompensation;
        else
            position.X -= horizontalCompensation;

        DrawData data = new(
            texture,
            position,
            source,
            drawInfo.colorArmorBody,
            player.bodyRotation,
            new Vector2(texture.Width / 2f, frameHeight / 2f),
            1f,
            drawInfo.playerEffect,
            0f)
        {
            shader = drawInfo.cWings
        };

        drawInfo.DrawDataCache.Add(data);
    }
}

[Autoload(Side = ModSide.Client)]
public sealed class HinaBodyBaseLayer : PlayerDrawLayer
{
    private static Asset<Texture2D>? _torso;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead
           && !drawInfo.drawPlayer.ghost
           && HinaVanitySet.BodyVisible(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new BeforeParent(PlayerDrawLayers.Torso);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;

        _torso ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Hina/HinaTorso",
            AssetRequestMode.ImmediateLoad);

        Rectangle source = player.bodyFrame;
        Vector2 position = new(
            (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f));
        position += player.bodyPosition + new Vector2(player.bodyFrame.Width / 2f, player.bodyFrame.Height / 2f);

        DrawData data = new(
            _torso.Value,
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
