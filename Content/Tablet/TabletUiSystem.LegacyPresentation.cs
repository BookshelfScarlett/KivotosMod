using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet;

public sealed partial class TabletUiSystem
{
    private static readonly TabletStartupAnimation Startup = new();
    private static Asset<Texture2D> _startupBackground, _startupLogo, _startupTriangle;
    private static MiscShaderData _classroomWaterShader, _startupGlitchShader;
    private static RasterizerState _legacyPresentationRasterizer;

    private static void LoadLegacyPresentation()
    {
        const AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
        _startupBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/Startup/Background", mode);
        _startupLogo = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/Startup/Logo", mode);
        _startupTriangle = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/Startup/Triangle", mode);
        _classroomWaterShader = new MiscShaderData(ModContent.Request<Effect>("KivotosMod/Assets/Effects/TabletWaterCaustic", mode), "WaterCaustic");
        _startupGlitchShader = new MiscShaderData(ModContent.Request<Effect>("KivotosMod/Assets/Effects/TabletGlitch", mode), "Glitch");
        _legacyPresentationRasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };
    }

    private static void UnloadLegacyPresentation()
    {
        Startup.Reset();
        _startupBackground = _startupLogo = _startupTriangle = null;
        _classroomWaterShader = _startupGlitchShader = null;
        RasterizerState rasterizer = _legacyPresentationRasterizer;
        _legacyPresentationRasterizer = null;
        Main.QueueMainThreadAction(() => rasterizer?.Dispose());
    }

    private static void DrawStartupScreen(SpriteBatch sb, Layout layout)
    {
        if (Startup.Visible) DrawLegacyPresentationPass(sb, layout, startup: true);
    }

    // Reuse the original shader passes and parameters, adapting their draw order
    // to the current tablet's UI scale, viewport clipping and SpriteBatch state.
    private static void DrawLegacyPresentationPass(SpriteBatch sb, Layout layout, bool startup)
    {
        GraphicsDevice device = sb.GraphicsDevice;
        sb.End();
        Rectangle oldScissor = device.ScissorRectangle;
        RasterizerState oldRasterizer = device.RasterizerState;
        BlendState oldBlend = device.BlendState;
        DepthStencilState oldDepth = device.DepthStencilState;
        SamplerState oldSampler = device.SamplerStates[0];
        bool begun = false;
        try
        {
            Rectangle screen = layout.ToScreen(DisplayRect);
            Vector2 tl = Vector2.Transform(new Vector2(screen.Left, screen.Top), Main.UIScaleMatrix);
            Vector2 br = Vector2.Transform(new Vector2(screen.Right, screen.Bottom), Main.UIScaleMatrix);
            device.ScissorRectangle = Rectangle.Intersect(new Rectangle((int)System.MathF.Floor(tl.X), (int)System.MathF.Floor(tl.Y),
                System.Math.Max(1, (int)System.MathF.Ceiling(br.X - tl.X)), System.Math.Max(1, (int)System.MathF.Ceiling(br.Y - tl.Y))), device.Viewport.Bounds);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, _legacyPresentationRasterizer, null, Main.UIScaleMatrix);
            begun = true;
            if (startup)
            {
                // Legacy Display has -4px padding: its 522x394 unlock textures
                // extend four pixels beyond the 514x386 screen, then are clipped.
                Rectangle crop = new(4, 4, DisplayRect.Width, DisplayRect.Height);
                sb.Draw(_startupBackground.Value, screen, crop, Color.White * Startup.Opacity);
                Texture2D triangleTexture = _startupTriangle.Value;
                foreach (TabletStartupAnimation.Triangle triangle in Startup.Triangles)
                {
                    if (triangle.Opacity <= 0) continue;
                    Vector2 point = new(DisplayRect.X - 4 + triangle.X, DisplayRect.Y - 4 + triangle.Y);
                    sb.Draw(triangleTexture, layout.ToScreen(point), null, Color.White * (triangle.Opacity * Startup.Opacity),
                        triangle.Flipped ? MathHelper.Pi : 0, new Vector2(triangleTexture.Width / 2f, triangleTexture.Height / 2f),
                        layout.Scale, SpriteEffects.None, 0);
                }
                _startupGlitchShader.UseShaderSpecificData(new Vector4(5, 0.025f, 0.1f, 100));
                _startupGlitchShader.UseSaturation(0);
                _startupGlitchShader.UseOpacity(Startup.Opacity);
                _startupGlitchShader.UseImage0(_startupLogo);
                _startupGlitchShader.Apply();
                sb.Draw(_startupLogo.Value, screen, crop, Color.White);
            }
            else
            {
                _classroomWaterShader.UseColor(Color.White);
                _classroomWaterShader.UseShaderSpecificData(new Vector4(20, 0.1f, 8, 35));
                _classroomWaterShader.UseImage0(_classroomLayers[2]);
                _classroomWaterShader.UseOpacity(1);
                _classroomWaterShader.Apply();
                sb.Draw(_classroomLayers[2].Value, screen, new Rectangle(250, 0, DisplayRect.Width, DisplayRect.Height), Color.White);
            }
        }
        finally
        {
            if (begun) sb.End();
            device.ScissorRectangle = oldScissor;
            sb.Begin(SpriteSortMode.Deferred, oldBlend, oldSampler, oldDepth, oldRasterizer, null, Main.UIScaleMatrix);
        }
    }
}
