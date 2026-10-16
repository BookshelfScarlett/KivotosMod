using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Vanity.Noa;

/// <summary>The long rear hair follows head animation and stays BEHIND the body,
/// wings, coat, and arm atlas, instead of covering the player's held weapon.</summary>
[Autoload(Side = ModSide.Client)]
public sealed class NoaBackHairLayer : PlayerDrawLayer
{
    private static Asset<Texture2D> _texture;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.ghost
            && NoaVanitySet.HeadVisible(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new Between(PlayerDrawLayers.Wings, PlayerDrawLayers.BackAcc);

    public override bool IsHeadLayer => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        _texture ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Noa/NoaBackHair", AssetRequestMode.ImmediateLoad);

        Vector2 position = drawInfo.helmetOffset
            + new Vector2(
                (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f))
            + player.headPosition + drawInfo.headVect;
        drawInfo.DrawDataCache.Add(new DrawData(
            _texture.Value, position, player.bodyFrame, drawInfo.colorArmorHead,
            player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0f)
        { shader = drawInfo.cHead });
    }
}

/// <summary>
/// Noa's side locks draw ABOVE the vanilla foreground arm while standing or walking
/// on the ground. Jumping, mounting and active item use keep the original head-layer
/// order so held-weapon animations are not permanently covered.
/// The foreground and head versions are mutually exclusive (no double rendering).
/// </summary>
[Autoload(Side = ModSide.Client)]
public sealed class NoaSideLocksLayer : PlayerDrawLayer
{
    private static Asset<Texture2D> _texture;

    internal static bool HasNoaHair(PlayerDrawSet info)
    {
        Player player = info.drawPlayer;
        return !player.dead && !player.ghost && NoaVanitySet.HeadVisible(player);
    }

    internal static bool StandingOrWalking(Player player)
    {
        // Horizontal movement must NOT change the hair's depth; otherwise the side
        // locks pop behind the foreground arm as soon as the player starts walking.
        // Only airborne, mounted and active item-use poses keep the old head order.
        return !player.mount.Active
            && player.itemAnimation <= 0
            && System.Math.Abs(player.velocity.Y) < 0.1f;
    }

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => HasNoaHair(drawInfo) && !StandingOrWalking(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new AfterParent(PlayerDrawLayers.Head);

    public override bool IsHeadLayer => true;

    internal static void DrawHair(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        _texture ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Noa/NoaSideLocks", AssetRequestMode.ImmediateLoad);

        Vector2 position = drawInfo.helmetOffset
            + new Vector2(
                (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
                (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f))
            + player.headPosition + drawInfo.headVect;
        drawInfo.DrawDataCache.Add(new DrawData(
            _texture.Value, position, player.bodyFrame, drawInfo.colorArmorHead,
            player.headRotation, drawInfo.headVect, 1f, drawInfo.playerEffect, 0f)
        { shader = drawInfo.cHead });
    }

    protected override void Draw(ref PlayerDrawSet drawInfo)
        => DrawHair(ref drawInfo);
}

/// <summary>
/// Grounded idle + walking foreground hair layer. ArmOverItem is the vanilla
/// foreground arm and BladedGlove follows it. Placing the hair between them keeps
/// Noa's side locks in front of the arm while walking, without drawing twice.
/// </summary>
[Autoload(Side = ModSide.Client)]
public sealed class NoaIdleSideLocksLayer : PlayerDrawLayer
{
    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => NoaSideLocksLayer.HasNoaHair(drawInfo)
            && NoaSideLocksLayer.StandingOrWalking(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new Between(PlayerDrawLayers.ArmOverItem, PlayerDrawLayers.BladedGlove);

    public override bool IsHeadLayer => true;

    protected override void Draw(ref PlayerDrawSet drawInfo)
        => NoaSideLocksLayer.DrawHair(ref drawInfo);
}

/// <summary>Uses the normal 20-frame torso+coat and Terraria's composite arms.
/// Both layers are drawn before Torso; the side locks remain head-controlled.</summary>
[Autoload(Side = ModSide.Client)]
public sealed class NoaBodyBaseLayer : PlayerDrawLayer
{
    private static Asset<Texture2D> _torso;
    private static Asset<Texture2D> _coat;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        => !drawInfo.drawPlayer.dead && !drawInfo.drawPlayer.ghost
            && NoaVanitySet.BodyVisible(drawInfo.drawPlayer);

    public override Position GetDefaultPosition()
        => new BeforeParent(PlayerDrawLayers.Torso);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        _torso ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Noa/NoaTorso", AssetRequestMode.ImmediateLoad);
        _coat ??= ModContent.Request<Texture2D>(
            "KivotosMod/Content/Items/Vanity/Noa/NoaCoat", AssetRequestMode.ImmediateLoad);

        Rectangle source = player.bodyFrame;
        Vector2 position = new(
            (int)(drawInfo.Position.X - Main.screenPosition.X - player.bodyFrame.Width / 2f + player.width / 2f),
            (int)(drawInfo.Position.Y - Main.screenPosition.Y + player.height - player.bodyFrame.Height + 4f));
        position += player.bodyPosition + new Vector2(player.bodyFrame.Width / 2f, player.bodyFrame.Height / 2f);

        AddPart(ref drawInfo, _torso.Value, source, position);
        AddPart(ref drawInfo, _coat.Value, source, position);
    }

    private static void AddPart(ref PlayerDrawSet drawInfo, Texture2D tex, Rectangle source, Vector2 position)
    {
        Player player = drawInfo.drawPlayer;
        drawInfo.DrawDataCache.Add(new DrawData(tex, position, source,
            drawInfo.colorArmorBody, player.bodyRotation, drawInfo.bodyVect,
            1f, drawInfo.playerEffect, 0f) { shader = drawInfo.cBody });
    }
}
