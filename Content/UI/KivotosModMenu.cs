using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace KivotosMod.Content.UI;

/// <summary>
/// KivotosMod title-screen theme.
///
/// The vanilla/tModLoader menu controls are intentionally left untouched. This class only
/// replaces the visual layer behind them and the menu music, which makes the mod much less
/// brittle across tModLoader updates than patching Main.DrawMenu directly.
/// </summary>
[Autoload(Side = ModSide.Client)]
public sealed class KivotosModMenu : ModMenu
{
    private static readonly string[] SlidePaths =
    {
        "KivotosMod/Assets/Menu/Slide01",
        "KivotosMod/Assets/Menu/Slide02",
        "KivotosMod/Assets/Menu/Slide03",
        "KivotosMod/Assets/Menu/Slide04"
    };

    // Every image remains on screen for 3-5 seconds, as requested.
    // The cross-fade is contained inside the final part of each duration.
    private static readonly float[] SlideDurations = { 4.2f, 3.8f, 4.8f, 4.3f };

    private const float CrossFadeSeconds = 0.85f;
    private const float MenuReadabilityShade = 0.10f;

    private readonly Stopwatch _animationClock = new();
    private readonly Asset<Texture2D>[] _slides = new Asset<Texture2D>[SlidePaths.Length];
    private bool _slidesLoaded;

    public override string DisplayName => "Kivotos";

    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Step_by_Step");

    public override void OnSelected()
    {
        // Load all four full-resolution menu images up front. This keeps PNG decode / GPU upload
        // work away from the exact moment a cross-fade begins.
        EnsureSlidesLoaded();

        _animationClock.Restart();
    }

    public override void OnDeselected()
    {
        _animationClock.Stop();
    }

    public override bool PreDrawLogo(
        SpriteBatch spriteBatch,
        ref Vector2 logoDrawCenter,
        ref float logoRotation,
        ref float logoScale,
        ref Color drawColor)
    {
        EnsureSlidesLoaded();

        if (!_animationClock.IsRunning)
            _animationClock.Restart();

        float elapsed = (float)_animationClock.Elapsed.TotalSeconds;
        GetSlideState(elapsed, out int currentIndex, out int nextIndex, out float localTime, out float currentDuration);

        // A slide first becomes visible as the incoming image during the PREVIOUS slide's
        // cross-fade. Therefore, when it becomes the current slide its motion is already
        // CrossFadeSeconds old. Keeping that age here prevents the pan/zoom from snapping
        // backwards to progress 0 on the transition boundary.
        float currentMotionTime = localTime + CrossFadeSeconds;
        // The image remains alive for its incoming cross-fade plus its own slide duration.
        // Using that full lifetime means it keeps drifting during the outgoing fade instead of
        // visibly coming to a stop just before the transition.
        float currentMotionDuration = currentDuration + CrossFadeSeconds;
        float currentProgress = MathHelper.Clamp(currentMotionTime / currentMotionDuration, 0f, 1f);

        Texture2D current = _slides[currentIndex].Value;

        DrawCover(spriteBatch, current, currentIndex, currentProgress, 1f);

        float fadeStart = Math.Max(0f, currentDuration - CrossFadeSeconds);
        if (localTime >= fadeStart)
        {
            float fade = MathHelper.Clamp((localTime - fadeStart) / CrossFadeSeconds, 0f, 1f);
            fade = SmoothStep01(fade);

            Texture2D next = _slides[nextIndex].Value;

            // Drawing the next slide over an opaque current slide with alpha performs the
            // desired interpolation without exposing the original Terraria background.
            // At fade end this reaches exactly CrossFadeSeconds of motion. On the next frame
            // the same image becomes current and currentMotionTime starts at that same value,
            // so there is no one-frame position/zoom reset.
            float nextMotionTime = localTime - fadeStart;
            float nextMotionDuration = SlideDurations[nextIndex] + CrossFadeSeconds;
            float nextProgress = MathHelper.Clamp(nextMotionTime / nextMotionDuration, 0f, 1f);
            DrawCover(spriteBatch, next, nextIndex, nextProgress, fade);
        }

        if (MenuReadabilityShade > 0f)
        {
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.Black * MenuReadabilityShade);
        }

        // true = keep the normal tModLoader/Terraria logo and menu controls.
        return true;
    }

    private void EnsureSlidesLoaded()
    {
        if (_slidesLoaded)
            return;

        for (int i = 0; i < SlidePaths.Length; i++)
            _slides[i] = ModContent.Request<Texture2D>(SlidePaths[i], AssetRequestMode.ImmediateLoad);

        _slidesLoaded = true;
    }

    private static void GetSlideState(
        float elapsed,
        out int currentIndex,
        out int nextIndex,
        out float localTime,
        out float currentDuration)
    {
        float cycleDuration = 0f;
        for (int i = 0; i < SlideDurations.Length; i++)
            cycleDuration += SlideDurations[i];

        float cycleTime = elapsed % cycleDuration;
        currentIndex = 0;

        for (int i = 0; i < SlideDurations.Length; i++)
        {
            if (cycleTime < SlideDurations[i])
            {
                currentIndex = i;
                break;
            }

            cycleTime -= SlideDurations[i];
        }

        localTime = cycleTime;
        currentDuration = SlideDurations[currentIndex];
        nextIndex = (currentIndex + 1) % SlidePaths.Length;
    }

    private static void DrawCover(
        SpriteBatch spriteBatch,
        Texture2D texture,
        int slideIndex,
        float progress,
        float alpha)
    {
        if (alpha <= 0f || Main.screenWidth <= 0 || Main.screenHeight <= 0)
            return;

        progress = CinematicEase(MathHelper.Clamp(progress, 0f, 1f));

        float coverScale = Math.Max(
            (float)Main.screenWidth / texture.Width,
            (float)Main.screenHeight / texture.Height);

        // Each slide gets its own restrained push-in. The reference video keeps the camera
        // motion subtle: the pan carries the mood while the zoom mainly provides overscan and
        // a little depth, rather than making the image visibly "breathe".
        float zoomStart;
        float zoomEnd;
        Vector2 startRatio;
        Vector2 endRatio;

        switch (slideIndex & 3)
        {
            default:
            case 0: // Slide01: the visible picture drifts RIGHT.
                // This shot is visually heavy on the right (the two girls), so the travel begins
                // farther left and ends short of the hard edge. The larger overscan gives the
                // movement enough distance to be clearly readable without losing the group.
                zoomStart = 1.055f;
                zoomEnd   = 1.085f;
                startRatio = new Vector2(-0.78f,  0.07f);
                endRatio   = new Vector2( 0.42f, -0.01f);
                break;

            case 1: // Slide02: CAMERA / VIEW moves UP (the image therefore drifts DOWN).
                // Important: this is a camera pan upward, not simply pulling the bitmap upward.
                // In screen-space drawing an upward-moving view requires the source image to
                // move downward. Keep X nearly fixed around the girl's visual center and use the
                // larger vertical overscan to reveal progressively more of the upper scenery.
                zoomStart = 1.038f;
                zoomEnd   = 1.072f;
                startRatio = new Vector2(-0.07f, -0.50f);
                endRatio   = new Vector2(-0.07f,  0.62f);
                break;

            case 2: // Slide03: visible picture drifts LEFT with a broad horizontal sweep.
                // The three figures span most of the frame, so this shot can carry the strongest
                // leftward motion while still retaining a stable visual center.
                zoomStart = 1.060f;
                zoomEnd   = 1.092f;
                startRatio = new Vector2( 0.72f,  0.05f);
                endRatio   = new Vector2(-0.62f,  0.01f);
                break;

            case 3: // Slide04: visible picture drifts LEFT, now with substantially more travel.
                // FIX2 was too timid here. The larger overscan lets the group and landscape move
                // decisively while the endpoints remain inside the crop-safe region.
                zoomStart = 1.055f;
                zoomEnd   = 1.088f;
                startRatio = new Vector2( 0.68f,  0.06f);
                endRatio   = new Vector2(-0.60f,  0.01f);
                break;
        }

        float zoom = MathHelper.Lerp(zoomStart, zoomEnd, progress);
        float scale = coverScale * zoom;

        float renderedWidth = texture.Width * scale;
        float renderedHeight = texture.Height * scale;
        float spareX = Math.Max(0f, (renderedWidth - Main.screenWidth) * 0.5f);
        float spareY = Math.Max(0f, (renderedHeight - Main.screenHeight) * 0.5f);

        // Ratios are relative to the crop-safe overscan. This means the chosen visual center and
        // travel direction survive different aspect ratios without exposing empty borders.
        Vector2 start = new(startRatio.X * spareX, startRatio.Y * spareY);
        Vector2 end   = new(endRatio.X   * spareX, endRatio.Y   * spareY);
        Vector2 pan = Vector2.Lerp(start, end, progress);
        Vector2 center = new(Main.screenWidth * 0.5f, Main.screenHeight * 0.5f);
        Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);

        spriteBatch.Draw(
            texture,
            center + pan,
            null,
            Color.White * MathHelper.Clamp(alpha, 0f, 1f),
            0f,
            origin,
            scale,
            SpriteEffects.None,
            0f);
    }

    private static float CinematicEase(float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);
        // Sine ease-in/out is gentler than a hard linear pan but spends more of the shot moving
        // than SmoothStep, matching the restrained drifting feel of the supplied reference video.
        return 0.5f - 0.5f * (float)Math.Cos(Math.PI * t);
    }

    private static float SmoothStep01(float t)
    {
        t = MathHelper.Clamp(t, 0f, 1f);
        return t * t * (3f - 2f * t);
    }
}
