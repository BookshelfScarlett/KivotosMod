using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Independent, tablet-only portrait rig used by the clean-room UI rewrite.
/// It uses the supplied portrait art as data, but owns its own transform tree,
/// idle animation, eye tracking and draw scheduler. No legacy portrait classes,
/// Student/NPC systems or old UI classes are referenced.
/// </summary>
internal static class TabletDynamicPortraitRenderer
{
    private sealed class Joint
    {
        public readonly string Name;
        public readonly Joint Parent;
        public Vector2 Local;
        public float Rotation;
        public float Scale = 1f;

        public Vector2 World;
        public float WorldRotation;
        public float WorldScale;

        public Joint(string name, Joint parent, Vector2 local)
        {
            Name = name;
            Parent = parent;
            Local = local;
        }

        public void Solve()
        {
            if (Parent == null)
            {
                World = Local;
                WorldRotation = Rotation;
                WorldScale = Scale;
                _solved = true;
                return;
            }

            Parent.SolveIfNeeded();
            World = Parent.World + Rotate(Local * Parent.WorldScale, Parent.WorldRotation);
            WorldRotation = Parent.WorldRotation + Rotation;
            WorldScale = Parent.WorldScale * Scale;
            _solved = true;
        }

        private bool _solved;
        public void ResetSolve() => _solved = false;
        private void SolveIfNeeded()
        {
            if (!_solved)
            {
                Solve();
                _solved = true;
            }
        }
    }

    private static readonly Dictionary<string, Asset<Texture2D>> Assets = new(StringComparer.Ordinal);
    private static Rectangle? _activeClipBounds;

    public static void Unload()
    {
        Assets.Clear();
        _activeClipBounds = null;
    }

    public static bool Draw(
        SpriteBatch sb,
        TabletStudentId id,
        Func<Vector2, Vector2> toScreen,
        float uiScale,
        Rectangle tabletBounds,
        Vector2 mouseTablet,
        float portraitScale,
        float rootYOffset = 0f,
        Rectangle? clipBounds = null)
    {
        Rectangle? previousClip = _activeClipBounds;
        _activeClipBounds = clipBounds;
        try
        {
            switch (id)
            {
                case TabletStudentId.Arona:
                    DrawArona(sb, toScreen, uiScale, tabletBounds, mouseTablet, portraitScale, rootYOffset);
                    return true;
                case TabletStudentId.Sora:
                    DrawSora(sb, toScreen, uiScale, tabletBounds, mouseTablet, portraitScale, rootYOffset);
                    return true;
                default:
                    return false;
            }
        }
        finally
        {
            _activeClipBounds = previousClip;
        }
    }

    private static Texture2D Texture(TabletStudentId id, string name)
    {
        string student = id == TabletStudentId.Arona ? "Arona" : "Sora";
        string path = $"KivotosMod/Assets/Tablet/Students/{student}/Portrait/{name}";
        if (!Assets.TryGetValue(path, out Asset<Texture2D> asset))
        {
            asset = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
            Assets[path] = asset;
        }
        return asset.Value;
    }

    private static Vector2 Rotate(Vector2 value, float radians)
    {
        float c = MathF.Cos(radians);
        float s = MathF.Sin(radians);
        return new Vector2(value.X * c - value.Y * s, value.X * s + value.Y * c);
    }

    private static Vector2 LookOffset(Vector2 eyeTablet, Vector2 mouseTablet, float maxX, float maxY)
    {
        Vector2 delta = mouseTablet - eyeTablet;
        if (delta.LengthSquared() < 0.001f)
            return Vector2.Zero;

        delta.Normalize();
        return new Vector2(delta.X * maxX, delta.Y * maxY);
    }

    private static Rectangle FrameX(Texture2D texture, int frames, int frame)
    {
        frames = Math.Max(1, frames);
        int width = texture.Width / frames;
        frame = Math.Clamp(frame, 0, frames - 1);
        return new Rectangle(width * frame, 0, width, texture.Height);
    }

    private static Rectangle FrameXY(Texture2D texture, int columns, int rows, int x, int y)
    {
        columns = Math.Max(1, columns);
        rows = Math.Max(1, rows);
        int width = texture.Width / columns;
        int height = texture.Height / rows;
        x = Math.Clamp(x, 0, columns - 1);
        y = Math.Clamp(y, 0, rows - 1);
        return new Rectangle(x * width, y * height, width, height);
    }

    private static void DrawPart(
        SpriteBatch sb,
        Texture2D texture,
        Joint joint,
        Vector2 rootTablet,
        Func<Vector2, Vector2> toScreen,
        float uiScale,
        float portraitScale,
        Vector2 origin,
        Rectangle? source = null,
        SpriteEffects effects = SpriteEffects.None,
        Color? color = null,
        Vector2? extraLocal = null,
        float extraRotation = 0f,
        float extraScale = 1f)
    {
        Vector2 local = joint.World + Rotate(extraLocal ?? Vector2.Zero, joint.WorldRotation) * joint.WorldScale;
        Vector2 tabletPosition = rootTablet + local * portraitScale;

        Rectangle drawSource = source ?? texture.Bounds;
        Vector2 drawOrigin = origin;
        float localScale = portraitScale * joint.WorldScale * extraScale;

        // The legacy portrait renderer composed each character into a bounded surface before
        // presenting it on the tablet. Our direct bone renderer must reproduce that clipping
        // explicitly; otherwise out-of-bounds pieces (especially Arona/Sora halos) can draw
        // above the tablet shell. Vertical source clipping is enough for the authored rigs
        // because their idle rotations are intentionally very small.
        if (_activeClipBounds is Rectangle clip && localScale > 0.0001f)
        {
            float top = tabletPosition.Y - drawOrigin.Y * localScale;
            float bottom = top + drawSource.Height * localScale;
            float clippedTop = MathF.Max(top, clip.Top);
            float clippedBottom = MathF.Min(bottom, clip.Bottom);

            if (clippedBottom <= clippedTop)
                return;

            float v0 = MathHelper.Clamp((clippedTop - top) / (drawSource.Height * localScale), 0f, 1f);
            float v1 = MathHelper.Clamp((clippedBottom - top) / (drawSource.Height * localScale), 0f, 1f);
            int cropTop = Math.Clamp((int)MathF.Floor(drawSource.Height * v0), 0, Math.Max(0, drawSource.Height - 1));
            int cropBottom = Math.Clamp((int)MathF.Ceiling(drawSource.Height * v1), cropTop + 1, drawSource.Height);

            drawSource = new Rectangle(
                drawSource.X,
                drawSource.Y + cropTop,
                drawSource.Width,
                cropBottom - cropTop);
            drawOrigin.Y -= cropTop;
        }

        sb.Draw(
            texture,
            toScreen(tabletPosition),
            drawSource,
            color ?? Color.White,
            joint.WorldRotation + extraRotation,
            drawOrigin,
            uiScale * localScale,
            effects,
            0f);
    }

    private static void SolveAll(params Joint[] joints)
    {
        foreach (Joint joint in joints)
            joint.ResetSolve();
        foreach (Joint joint in joints)
            joint.Solve();
    }

    private static void DrawArona(
        SpriteBatch sb,
        Func<Vector2, Vector2> toScreen,
        float uiScale,
        Rectangle bounds,
        Vector2 mouseTablet,
        float scale,
        float rootYOffset)
    {
        float time = (float)(Main.GameUpdateCount % 3600) / 60f;
        float breath = MathF.Sin(time * 1.35f);
        float sway = MathF.Sin(time * 0.72f);
        bool blink = Main.GameUpdateCount % 181 >= 174;

        Vector2 rootTablet = new(bounds.Center.X, bounds.Bottom - 2 + rootYOffset);

        Joint root = new("root", null, Vector2.Zero);
        Joint belly = new("belly", root, new Vector2(0, -264 + breath * 1.2f));
        Joint torso = new("torso", belly, new Vector2(0, -14));
        Joint neck = new("neck", torso, new Vector2(0, -54));
        Joint head = new("head", neck, new Vector2(0, -16));
        Joint halo = new("halo", head, Vector2.Zero);
        Joint hairTail = new("hairTail", head, new Vector2(31, -28));
        Joint hairBangL = new("hairBangL", head, new Vector2(-12, -77));
        Joint hairBangR = new("hairBangR", head, new Vector2(24, -77));
        Joint eyeL = new("eyeL", head, new Vector2(-17, -25));
        Joint eyeR = new("eyeR", head, new Vector2(17, -25));

        Joint shoulderL = new("shoulderL", torso, new Vector2(-35, -46));
        Joint shoulderR = new("shoulderR", torso, new Vector2(35, -46));
        Joint elbowL = new("elbowL", shoulderL, new Vector2(-14, 40));
        Joint elbowR = new("elbowR", shoulderR, new Vector2(14, 40));
        Joint wristL = new("wristL", elbowL, new Vector2(-14, 44));
        Joint wristR = new("wristR", elbowR, new Vector2(14, 44));

        Joint hipL = new("hipL", belly, new Vector2(-24, 19));
        Joint hipR = new("hipR", belly, new Vector2(24, 19));
        Joint kneeL = new("kneeL", hipL, new Vector2(4, 102));
        Joint kneeR = new("kneeR", hipR, new Vector2(-4, 102));

        Joint skirt1 = new("skirt1", belly, new Vector2(-29, 0));
        Joint skirt2 = new("skirt2", belly, new Vector2(-15, 6));
        Joint skirt3 = new("skirt3", belly, new Vector2(0, 8));
        Joint skirt4 = new("skirt4", belly, new Vector2(15, 6));
        Joint skirt5 = new("skirt5", belly, new Vector2(29, 0));
        Joint ribbonTie = new("ribbonTie", torso, new Vector2(3, -18));
        Joint ribbon1 = new("ribbon1", ribbonTie, Vector2.Zero);
        Joint ribbon2 = new("ribbon2", ribbonTie, Vector2.Zero);
        Joint ribbon3 = new("ribbon3", ribbonTie, Vector2.Zero);
        Joint ribbon4 = new("ribbon4", ribbonTie, Vector2.Zero);
        Joint bowL = new("bowL", head, new Vector2(0, -79));
        Joint bowR = new("bowR", head, new Vector2(10, -77));

        // Independent idle rig: subtle center-of-mass motion plus secondary hair/cloth motion.
        belly.Rotation = sway * 0.010f;
        torso.Rotation = -sway * 0.013f;
        head.Rotation = sway * 0.018f;
        shoulderL.Rotation = 0.03f + breath * 0.006f;
        shoulderR.Rotation = -0.03f - breath * 0.006f;
        elbowL.Rotation = -0.08f + sway * 0.012f;
        elbowR.Rotation = 0.08f - sway * 0.012f;
        hairTail.Rotation = -0.05f + MathF.Sin(time * 1.65f) * 0.045f;
        hairBangL.Rotation = MathF.Sin(time * 1.45f + 0.4f) * 0.018f;
        hairBangR.Rotation = MathF.Sin(time * 1.45f + 1.0f) * 0.018f;
        halo.Rotation = -head.Rotation * 0.45f;
        skirt1.Rotation = MathF.Sin(time * 1.2f + 0.1f) * 0.018f;
        skirt2.Rotation = MathF.Sin(time * 1.2f + 0.5f) * 0.014f;
        skirt3.Rotation = MathF.Sin(time * 1.2f + 0.9f) * 0.010f;
        skirt4.Rotation = MathF.Sin(time * 1.2f + 1.3f) * 0.014f;
        skirt5.Rotation = MathF.Sin(time * 1.2f + 1.7f) * 0.018f;
        ribbon1.Rotation = MathF.Sin(time * 1.8f) * 0.030f;
        ribbon2.Rotation = -MathF.Sin(time * 1.8f + 0.4f) * 0.030f;
        ribbon3.Rotation = MathF.Sin(time * 1.6f + 0.8f) * 0.022f;
        ribbon4.Rotation = -MathF.Sin(time * 1.6f + 1.2f) * 0.022f;

        SolveAll(root, belly, torso, neck, head, halo, hairTail, hairBangL, hairBangR,
            shoulderL, shoulderR, elbowL, elbowR, wristL, wristR, hipL, hipR, kneeL, kneeR,
            skirt1, skirt2, skirt3, skirt4, skirt5, ribbonTie, ribbon1, ribbon2, ribbon3, ribbon4,
            bowL, bowR, eyeL, eyeR);

        // Eye tracking is solved in tablet coordinates so it follows the cursor at every UI scale.
        Vector2 headTablet = rootTablet + head.World * scale;
        Vector2 eyeShift = LookOffset(headTablet, mouseTablet, 2.4f, 1.7f);
        eyeL.Local += eyeShift / scale;
        eyeR.Local += eyeShift / scale;
        SolveAll(root, belly, torso, neck, head, halo, hairTail, hairBangL, hairBangR,
            shoulderL, shoulderR, elbowL, elbowR, wristL, wristR, hipL, hipR, kneeL, kneeR,
            skirt1, skirt2, skirt3, skirt4, skirt5, ribbonTie, ribbon1, ribbon2, ribbon3, ribbon4,
            bowL, bowR, eyeL, eyeR);

        // Backmost details and limbs.
        DrawPart(sb, Texture(TabletStudentId.Arona, "Halo"), halo, rootTablet, toScreen, uiScale, scale, new Vector2(15, 132));

        Texture2D armUpper = Texture(TabletStudentId.Arona, "ArmUpper");
        Texture2D armLower = Texture(TabletStudentId.Arona, "ArmLower");
        Texture2D hand = Texture(TabletStudentId.Arona, "Hand");
        Texture2D sleeveUpper = Texture(TabletStudentId.Arona, "Sailor_SleeveUpper");
        Texture2D sleeveLower = Texture(TabletStudentId.Arona, "Sailor_SleeveLower");

        DrawPart(sb, armUpper, shoulderR, rootTablet, toScreen, uiScale, scale, new Vector2(12, 10), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, sleeveUpper, shoulderR, rootTablet, toScreen, uiScale, scale, new Vector2(12, 14), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, armLower, elbowR, rootTablet, toScreen, uiScale, scale, new Vector2(10, 8), FrameX(armLower, 2, 1), SpriteEffects.FlipHorizontally);
        DrawPart(sb, hand, wristR, rootTablet, toScreen, uiScale, scale, new Vector2(22, 6), FrameXY(hand, 9, 2, 0, 1), SpriteEffects.FlipHorizontally);
        DrawPart(sb, sleeveLower, elbowR, rootTablet, toScreen, uiScale, scale, new Vector2(14, 10), FrameX(sleeveLower, 2, 1));

        DrawPart(sb, armUpper, shoulderL, rootTablet, toScreen, uiScale, scale, new Vector2(24, 10));
        DrawPart(sb, sleeveUpper, shoulderL, rootTablet, toScreen, uiScale, scale, new Vector2(30, 14));
        DrawPart(sb, armLower, elbowL, rootTablet, toScreen, uiScale, scale, new Vector2(24, 8), FrameX(armLower, 2, 0));
        DrawPart(sb, hand, wristL, rootTablet, toScreen, uiScale, scale, new Vector2(22, 6), FrameXY(hand, 9, 2, 0, 1));
        DrawPart(sb, sleeveLower, elbowL, rootTablet, toScreen, uiScale, scale, new Vector2(32, 10), FrameX(sleeveLower, 2, 0));

        DrawPart(sb, Texture(TabletStudentId.Arona, "LegRight"), kneeR, rootTablet, toScreen, uiScale, scale, new Vector2(17, 17));
        DrawPart(sb, Texture(TabletStudentId.Arona, "ThighRightShadow"), hipR, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));
        DrawPart(sb, Texture(TabletStudentId.Arona, "ThighRight"), hipR, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));
        DrawPart(sb, Texture(TabletStudentId.Arona, "LegLeft"), kneeL, rootTablet, toScreen, uiScale, scale, new Vector2(23, 17));
        DrawPart(sb, Texture(TabletStudentId.Arona, "ThighLeftShadow"), hipL, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));
        DrawPart(sb, Texture(TabletStudentId.Arona, "ThighLeft"), hipL, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));

        DrawPart(sb, Texture(TabletStudentId.Arona, "HairBack"), head, rootTablet, toScreen, uiScale, scale, new Vector2(61, 84));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Belly"), belly, rootTablet, toScreen, uiScale, scale, new Vector2(43, 44));

        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtLower1"), skirt1, rootTablet, toScreen, uiScale, scale, new Vector2(48, 8));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtLower5"), skirt5, rootTablet, toScreen, uiScale, scale, new Vector2(6, 6));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtLower4"), skirt4, rootTablet, toScreen, uiScale, scale, new Vector2(20, 6));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtLower2"), skirt2, rootTablet, toScreen, uiScale, scale, new Vector2(36, 8));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtLower3"), skirt3, rootTablet, toScreen, uiScale, scale, new Vector2(7, 4));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_SkirtUpper"), belly, rootTablet, toScreen, uiScale, scale, new Vector2(39, 12));

        DrawPart(sb, Texture(TabletStudentId.Arona, "Neck"), neck, rootTablet, toScreen, uiScale, scale, new Vector2(13, 28));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Choker"), neck, rootTablet, toScreen, uiScale, scale, new Vector2(13, 14));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Torso"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(39, 62));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Shirt"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(41, 66));

        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Ribbon4"), ribbon4, rootTablet, toScreen, uiScale, scale, new Vector2(36, 4));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Ribbon3"), ribbon3, rootTablet, toScreen, uiScale, scale, new Vector2(4, 4));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Ribbon2"), ribbon2, rootTablet, toScreen, uiScale, scale, new Vector2(36, 4));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_Ribbon1"), ribbon1, rootTablet, toScreen, uiScale, scale, new Vector2(2, 4));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_RibbonTie"), ribbonTie, rootTablet, toScreen, uiScale, scale, new Vector2(8, 6));
        DrawPart(sb, Texture(TabletStudentId.Arona, "HairTail"), hairTail, rootTablet, toScreen, uiScale, scale, new Vector2(16, 8));

        // Face stack.
        Texture2D eyeWhite = Texture(TabletStudentId.Arona, "EyeWhite");
        Texture2D eyeball = Texture(TabletStudentId.Arona, "Eyeball");
        Texture2D eyelidBack = Texture(TabletStudentId.Arona, "EyelidBack");
        Texture2D eyelidFront = Texture(TabletStudentId.Arona, "EyelidFront");
        Texture2D brow = Texture(TabletStudentId.Arona, "Brow");
        Texture2D mouth = Texture(TabletStudentId.Arona, "Mouth");
        int lidFrame = blink ? 3 : 0;

        DrawPart(sb, eyeWhite, head, rootTablet, toScreen, uiScale, scale, new Vector2(-3, 40), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyeWhite, head, rootTablet, toScreen, uiScale, scale, new Vector2(33, 40));
        DrawPart(sb, eyeball, eyeR, rootTablet, toScreen, uiScale, scale, new Vector2(8, 9), FrameX(eyeball, 2, 0));
        DrawPart(sb, eyeball, eyeL, rootTablet, toScreen, uiScale, scale, new Vector2(8, 9), FrameX(eyeball, 2, 0));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Face"), head, rootTablet, toScreen, uiScale, scale, new Vector2(45, 88));
        DrawPart(sb, mouth, head, rootTablet, toScreen, uiScale, scale, new Vector2(13, 14), FrameX(mouth, 10, 6));
        DrawPart(sb, eyelidBack, head, rootTablet, toScreen, uiScale, scale, new Vector2(-3, 44), FrameX(eyelidBack, 6, lidFrame), SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyelidBack, head, rootTablet, toScreen, uiScale, scale, new Vector2(35, 44), FrameX(eyelidBack, 6, lidFrame));
        DrawPart(sb, Texture(TabletStudentId.Arona, "FaceShadow"), head, rootTablet, toScreen, uiScale, scale, new Vector2(45, 88));
        DrawPart(sb, eyelidFront, head, rootTablet, toScreen, uiScale, scale, new Vector2(-3, 44), FrameX(eyelidFront, 6, lidFrame), SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyelidFront, head, rootTablet, toScreen, uiScale, scale, new Vector2(35, 44), FrameX(eyelidFront, 6, lidFrame));
        DrawPart(sb, brow, head, rootTablet, toScreen, uiScale, scale, new Vector2(-7, 50), FrameX(brow, 5, 0), SpriteEffects.FlipHorizontally);
        DrawPart(sb, Texture(TabletStudentId.Arona, "HairFront"), head, rootTablet, toScreen, uiScale, scale, new Vector2(55, 94));
        DrawPart(sb, brow, head, rootTablet, toScreen, uiScale, scale, new Vector2(25, 50), FrameX(brow, 5, 0));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_BowtieBand"), head, rootTablet, toScreen, uiScale, scale, new Vector2(33, 88));
        DrawPart(sb, Texture(TabletStudentId.Arona, "HairBangRight"), hairBangR, rootTablet, toScreen, uiScale, scale, new Vector2(3, 3));
        DrawPart(sb, Texture(TabletStudentId.Arona, "HairBangLeft"), hairBangL, rootTablet, toScreen, uiScale, scale, new Vector2(31, 3));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_BowtieRight"), bowR, rootTablet, toScreen, uiScale, scale, new Vector2(3, 23));
        DrawPart(sb, Texture(TabletStudentId.Arona, "Sailor_BowtieLeft"), bowL, rootTablet, toScreen, uiScale, scale, new Vector2(25, 53));
    }

    private static void DrawSora(
        SpriteBatch sb,
        Func<Vector2, Vector2> toScreen,
        float uiScale,
        Rectangle bounds,
        Vector2 mouseTablet,
        float scale,
        float rootYOffset)
    {
        float time = (float)(Main.GameUpdateCount % 3600) / 60f;
        float breath = MathF.Sin(time * 1.18f);
        float sway = MathF.Sin(time * 0.63f + 0.7f);
        bool blink = Main.GameUpdateCount % 197 >= 190;
        Vector2 rootTablet = new(bounds.Center.X, bounds.Bottom - 2 + rootYOffset);

        Joint root = new("root", null, Vector2.Zero);
        Joint belly = new("belly", root, new Vector2(0, -264 + breath * 1.1f));
        Joint torso = new("torso", belly, new Vector2(0, -14));
        Joint neck = new("neck", torso, new Vector2(0, -54));
        Joint head = new("head", neck, new Vector2(0, -16));
        Joint halo = new("halo", head, Vector2.Zero);
        Joint eyeL = new("eyeL", head, new Vector2(-17, -25));
        Joint eyeR = new("eyeR", head, new Vector2(17, -25));
        Joint bangMid = new("bangMid", head, new Vector2(0, -73));
        Joint bangRight = new("bangRight", head, new Vector2(1, -76));
        Joint tailL = new("tailL", head, new Vector2(-41, -36));
        Joint tailR = new("tailR", head, new Vector2(41, -36));
        Joint hairBackMiddle = new("hairBackMiddle", head, new Vector2(0, -28));
        Joint hairBackL2 = new("hairBackL2", head, new Vector2(-24, -48));
        Joint hairBackR2 = new("hairBackR2", head, new Vector2(24, -48));
        Joint hairBackL1 = new("hairBackL1", head, new Vector2(-20, -56));
        Joint hairBackR1 = new("hairBackR1", head, new Vector2(20, -56));

        Joint shoulderL = new("shoulderL", torso, new Vector2(-35, -46));
        Joint shoulderR = new("shoulderR", torso, new Vector2(35, -46));
        Joint elbowL = new("elbowL", shoulderL, new Vector2(-14, 40));
        Joint elbowR = new("elbowR", shoulderR, new Vector2(14, 40));
        Joint wristL = new("wristL", elbowL, new Vector2(-14, 44));
        Joint wristR = new("wristR", elbowR, new Vector2(14, 44));
        Joint hipL = new("hipL", belly, new Vector2(-24, 19));
        Joint hipR = new("hipR", belly, new Vector2(24, 19));
        Joint kneeL = new("kneeL", hipL, new Vector2(4, 102));
        Joint kneeR = new("kneeR", hipR, new Vector2(-4, 102));
        Joint wingL = new("wingL", belly, new Vector2(-27, -22));
        Joint wingR = new("wingR", belly, new Vector2(27, -22));

        Joint idCard = new("idCard", torso, new Vector2(25, -19));
        Joint shirtRibbon1 = new("shirtRibbon1", torso, new Vector2(0, -43));
        Joint shirtRibbon2 = new("shirtRibbon2", shirtRibbon1, Vector2.Zero);
        Joint shirtRibbon3 = new("shirtRibbon3", shirtRibbon1, Vector2.Zero);
        Joint shirtRibbon4 = new("shirtRibbon4", shirtRibbon1, Vector2.Zero);
        Joint shirtRibbon5 = new("shirtRibbon5", shirtRibbon1, Vector2.Zero);
        Joint apronRibbon1 = new("apronRibbon1", torso, new Vector2(24, -31));
        Joint apronRibbon2 = new("apronRibbon2", apronRibbon1, Vector2.Zero);
        Joint apronRibbon3 = new("apronRibbon3", apronRibbon1, Vector2.Zero);
        Joint apronArm1 = new("apronArm1", belly, new Vector2(-6, 13));
        Joint apronArm2 = new("apronArm2", apronArm1, new Vector2(-26, -46));
        Joint apronArm3 = new("apronArm3", apronArm2, new Vector2(-36, 10));
        Joint apronBL1 = new("apronBL1", belly, new Vector2(-17, 6));
        Joint apronBL2 = new("apronBL2", apronBL1, new Vector2(-17, 25));
        Joint apronBM1 = new("apronBM1", belly, new Vector2(0, 6));
        Joint apronBM2 = new("apronBM2", apronBM1, new Vector2(0, 31));
        Joint apronBR1 = new("apronBR1", belly, new Vector2(17, 6));
        Joint apronBR2 = new("apronBR2", apronBR1, new Vector2(17, 25));

        belly.Rotation = sway * 0.008f;
        torso.Rotation = -sway * 0.012f;
        head.Rotation = sway * 0.017f;
        shoulderL.Rotation = 0.025f + breath * 0.004f;
        shoulderR.Rotation = -0.025f - breath * 0.004f;
        elbowL.Rotation = -0.06f + sway * 0.01f;
        elbowR.Rotation = 0.06f - sway * 0.01f;
        halo.Rotation = -head.Rotation * 0.5f;
        tailL.Rotation = MathF.Sin(time * 1.55f + 0.2f) * 0.040f;
        tailR.Rotation = -MathF.Sin(time * 1.55f + 0.7f) * 0.040f;
        bangMid.Rotation = MathF.Sin(time * 1.4f + 0.3f) * 0.014f;
        bangRight.Rotation = MathF.Sin(time * 1.4f + 0.8f) * 0.018f;
        hairBackL1.Rotation = MathF.Sin(time * 1.05f + 0.2f) * 0.020f;
        hairBackL2.Rotation = MathF.Sin(time * 1.05f + 0.55f) * 0.024f;
        hairBackR1.Rotation = -MathF.Sin(time * 1.05f + 0.9f) * 0.020f;
        hairBackR2.Rotation = -MathF.Sin(time * 1.05f + 1.25f) * 0.024f;
        hairBackMiddle.Rotation = MathF.Sin(time * 0.9f) * 0.010f;
        wingL.Rotation = MathF.Sin(time * 1.25f) * 0.025f;
        wingR.Rotation = -MathF.Sin(time * 1.25f + 0.3f) * 0.025f;
        shirtRibbon2.Rotation = MathF.Sin(time * 1.8f) * 0.025f;
        shirtRibbon3.Rotation = -MathF.Sin(time * 1.8f + 0.4f) * 0.025f;
        shirtRibbon4.Rotation = MathF.Sin(time * 1.6f + 0.8f) * 0.020f;
        shirtRibbon5.Rotation = -MathF.Sin(time * 1.6f + 1.1f) * 0.020f;
        apronRibbon2.Rotation = MathF.Sin(time * 1.5f + 0.5f) * 0.035f;
        apronRibbon3.Rotation = -MathF.Sin(time * 1.5f + 1f) * 0.035f;

        Joint[] all = { root, belly, torso, neck, head, halo, eyeL, eyeR, bangMid, bangRight, tailL, tailR,
            hairBackMiddle, hairBackL2, hairBackR2, hairBackL1, hairBackR1, shoulderL, shoulderR, elbowL,
            elbowR, wristL, wristR, hipL, hipR, kneeL, kneeR, wingL, wingR, idCard,
            shirtRibbon1, shirtRibbon2, shirtRibbon3, shirtRibbon4, shirtRibbon5,
            apronRibbon1, apronRibbon2, apronRibbon3, apronArm1, apronArm2, apronArm3,
            apronBL1, apronBL2, apronBM1, apronBM2, apronBR1, apronBR2 };
        SolveAll(all);

        Vector2 headTablet = rootTablet + head.World * scale;
        Vector2 eyeShift = LookOffset(headTablet, mouseTablet, 2.3f, 1.6f);
        eyeL.Local += eyeShift / scale;
        eyeR.Local += eyeShift / scale;
        SolveAll(all);

        // Back hair/halo/wings and rear arms.
        DrawPart(sb, Texture(TabletStudentId.Sora, "Halo"), halo, rootTablet, toScreen, uiScale, scale, new Vector2(47, 138));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBackMiddle"), hairBackMiddle, rootTablet, toScreen, uiScale, scale, new Vector2(39, 40));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBackRight1"), hairBackR1, rootTablet, toScreen, uiScale, scale, new Vector2(25, 4));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBackLeft1"), hairBackL1, rootTablet, toScreen, uiScale, scale, new Vector2(43, 4));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBackRight2"), hairBackR2, rootTablet, toScreen, uiScale, scale, new Vector2(19, 4));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBackLeft2"), hairBackL2, rootTablet, toScreen, uiScale, scale, new Vector2(57, 4));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBack"), head, rootTablet, toScreen, uiScale, scale, new Vector2(43, 92));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Wing"), wingR, rootTablet, toScreen, uiScale, scale, new Vector2(6, 14), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, Texture(TabletStudentId.Sora, "Wing"), wingL, rootTablet, toScreen, uiScale, scale, new Vector2(68, 14));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronArmPiece3"), apronArm3, rootTablet, toScreen, uiScale, scale, new Vector2(7, 19));

        Texture2D armUpper = Texture(TabletStudentId.Sora, "ArmUpper");
        Texture2D armLower = Texture(TabletStudentId.Sora, "ArmLower");
        Texture2D hand = Texture(TabletStudentId.Sora, "Hand");
        Texture2D shirtSleeve = Texture(TabletStudentId.Sora, "Clerk_ShirtSleeve");
        Texture2D armBand = Texture(TabletStudentId.Sora, "Clerk_ArmBand");

        DrawPart(sb, armUpper, shoulderR, rootTablet, toScreen, uiScale, scale, new Vector2(12, 10), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, armLower, elbowR, rootTablet, toScreen, uiScale, scale, new Vector2(10, 8), FrameX(armLower, 2, 1), SpriteEffects.FlipHorizontally);
        DrawPart(sb, hand, wristR, rootTablet, toScreen, uiScale, scale, new Vector2(22, 6), FrameXY(hand, 9, 2, 0, 1), SpriteEffects.FlipHorizontally);
        DrawPart(sb, armBand, elbowR, rootTablet, toScreen, uiScale, scale, new Vector2(10, 8), FrameX(armBand, 2, 1), SpriteEffects.FlipHorizontally);
        DrawPart(sb, shirtSleeve, shoulderR, rootTablet, toScreen, uiScale, scale, new Vector2(16, 12), effects: SpriteEffects.FlipHorizontally);

        DrawPart(sb, armUpper, shoulderL, rootTablet, toScreen, uiScale, scale, new Vector2(24, 10));
        DrawPart(sb, armLower, elbowL, rootTablet, toScreen, uiScale, scale, new Vector2(24, 8), FrameX(armLower, 2, 0));
        DrawPart(sb, hand, wristL, rootTablet, toScreen, uiScale, scale, new Vector2(22, 6), FrameXY(hand, 9, 2, 0, 1));
        DrawPart(sb, shirtSleeve, shoulderL, rootTablet, toScreen, uiScale, scale, new Vector2(26, 12));

        // Lower body and uniform.
        DrawPart(sb, Texture(TabletStudentId.Sora, "LegRight"), kneeR, rootTablet, toScreen, uiScale, scale, new Vector2(17, 17));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_SockRight"), kneeR, rootTablet, toScreen, uiScale, scale, new Vector2(9, -51));
        DrawPart(sb, Texture(TabletStudentId.Sora, "ThighRight"), hipR, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));
        DrawPart(sb, Texture(TabletStudentId.Sora, "LegLeft"), kneeL, rootTablet, toScreen, uiScale, scale, new Vector2(23, 17));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_SockLeft"), kneeL, rootTablet, toScreen, uiScale, scale, new Vector2(21, -51));
        DrawPart(sb, Texture(TabletStudentId.Sora, "ThighLeft"), hipL, rootTablet, toScreen, uiScale, scale, new Vector2(23, 23));

        DrawPart(sb, Texture(TabletStudentId.Sora, "Belly"), belly, rootTablet, toScreen, uiScale, scale, new Vector2(43, 44));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtBottom"), belly, rootTablet, toScreen, uiScale, scale, new Vector2(41, 48));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomMiddle2"), apronBM2, rootTablet, toScreen, uiScale, scale, new Vector2(43, 31));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomRight2"), apronBR2, rootTablet, toScreen, uiScale, scale, new Vector2(15, 17));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomLeft2"), apronBL2, rootTablet, toScreen, uiScale, scale, new Vector2(17, 17));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomMiddle1"), apronBM1, rootTablet, toScreen, uiScale, scale, new Vector2(37, 16));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomRight1"), apronBR1, rootTablet, toScreen, uiScale, scale, new Vector2(-2, 12));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomLeft1"), apronBL1, rootTablet, toScreen, uiScale, scale, new Vector2(30, 12));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomBadgeGreen"), apronBL2, rootTablet, toScreen, uiScale, scale, new Vector2(-1, -3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronBottomBadgeRed"), apronBM2, rootTablet, toScreen, uiScale, scale, new Vector2(11, -3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronMiddle"), belly, rootTablet, toScreen, uiScale, scale, new Vector2(39, 46));

        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtNeck"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(15, 72));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Neck"), neck, rootTablet, toScreen, uiScale, scale, new Vector2(13, 28));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Torso"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(39, 62));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtTop"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(39, 68));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronTop"), torso, rootTablet, toScreen, uiScale, scale, new Vector2(31, 64));

        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronRibbon3"), apronRibbon3, rootTablet, toScreen, uiScale, scale, new Vector2(3, 15));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronRibbon2"), apronRibbon2, rootTablet, toScreen, uiScale, scale, new Vector2(19, 3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronRibbon1"), apronRibbon1, rootTablet, toScreen, uiScale, scale, new Vector2(7, 7));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtRibbon5"), shirtRibbon5, rootTablet, toScreen, uiScale, scale, new Vector2(17, 3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtRibbon4"), shirtRibbon4, rootTablet, toScreen, uiScale, scale, new Vector2(1, 3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtRibbon3"), shirtRibbon3, rootTablet, toScreen, uiScale, scale, new Vector2(21, 3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtRibbon2"), shirtRibbon2, rootTablet, toScreen, uiScale, scale, new Vector2(3, 3));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ShirtRibbon1"), shirtRibbon1, rootTablet, toScreen, uiScale, scale, new Vector2(7, 7));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronArmPiece2"), apronArm2, rootTablet, toScreen, uiScale, scale, new Vector2(41, 7));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_ApronArmPiece1"), apronArm1, rootTablet, toScreen, uiScale, scale, new Vector2(33, 51));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Clerk_IDCard"), idCard, rootTablet, toScreen, uiScale, scale, new Vector2(16, 9));

        // Face and foreground hair.
        Texture2D eyeWhite = Texture(TabletStudentId.Sora, "EyeWhite");
        Texture2D eyeball = Texture(TabletStudentId.Sora, "Eyeball");
        Texture2D eyelidBack = Texture(TabletStudentId.Sora, "EyelidBack");
        Texture2D eyelidFront = Texture(TabletStudentId.Sora, "EyelidFront");
        Texture2D brow = Texture(TabletStudentId.Sora, "Brow");
        Texture2D mouth = Texture(TabletStudentId.Sora, "Mouth");
        int lidFrame = blink ? 3 : 0;

        DrawPart(sb, eyeWhite, head, rootTablet, toScreen, uiScale, scale, new Vector2(-5, 36), effects: SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyeWhite, head, rootTablet, toScreen, uiScale, scale, new Vector2(33, 36));
        DrawPart(sb, eyeball, eyeR, rootTablet, toScreen, uiScale, scale, new Vector2(8, 9), FrameX(eyeball, 3, 0));
        DrawPart(sb, eyeball, eyeL, rootTablet, toScreen, uiScale, scale, new Vector2(8, 9), FrameX(eyeball, 3, 0));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Face"), head, rootTablet, toScreen, uiScale, scale, new Vector2(41, 88));
        DrawPart(sb, Texture(TabletStudentId.Sora, "Blush"), head, rootTablet, toScreen, uiScale, scale, new Vector2(25, 18));
        DrawPart(sb, mouth, head, rootTablet, toScreen, uiScale, scale, new Vector2(9, 12), FrameX(mouth, 5, 1));
        DrawPart(sb, eyelidBack, head, rootTablet, toScreen, uiScale, scale, new Vector2(-3, 44), FrameX(eyelidBack, 3, lidFrame == 3 ? 2 : 0), SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyelidBack, head, rootTablet, toScreen, uiScale, scale, new Vector2(35, 44), FrameX(eyelidBack, 3, lidFrame == 3 ? 2 : 0));
        DrawPart(sb, Texture(TabletStudentId.Sora, "FaceShadow"), head, rootTablet, toScreen, uiScale, scale, new Vector2(41, 88));
        DrawPart(sb, eyelidFront, head, rootTablet, toScreen, uiScale, scale, new Vector2(-3, 44), FrameX(eyelidFront, 3, lidFrame == 3 ? 2 : 0), SpriteEffects.FlipHorizontally);
        DrawPart(sb, eyelidFront, head, rootTablet, toScreen, uiScale, scale, new Vector2(35, 44), FrameX(eyelidFront, 3, lidFrame == 3 ? 2 : 0));
        DrawPart(sb, brow, head, rootTablet, toScreen, uiScale, scale, new Vector2(-7, 52), FrameX(brow, 4, 0), SpriteEffects.FlipHorizontally);
        DrawPart(sb, brow, head, rootTablet, toScreen, uiScale, scale, new Vector2(25, 52), FrameX(brow, 4, 0));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairTailRight"), tailR, rootTablet, toScreen, uiScale, scale, new Vector2(4, 10));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairTailLeft"), tailL, rootTablet, toScreen, uiScale, scale, new Vector2(24, 10));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairFront"), head, rootTablet, toScreen, uiScale, scale, new Vector2(45, 96));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBangRight"), bangRight, rootTablet, toScreen, uiScale, scale, new Vector2(4, 14));
        DrawPart(sb, Texture(TabletStudentId.Sora, "HairBangMiddle"), bangMid, rootTablet, toScreen, uiScale, scale, new Vector2(7, 3));
    }
}
