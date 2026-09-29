using KivotosMod.Assets.Register;
using KivotosMod.Cores.PixelatedRender;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace HJScarletRework.Core.PixelatedRender
{
    public class PixelatedRenderManager : ModSystem
    {
        public static bool BeginDrawProj = false;
        public static RenderTarget2D BeforePlayerTarget;
        public static RenderTarget2D BeforeDustTarget;
        public static RenderTarget2D BeforeProjTarget;

        public static RenderTarget2D BeforePlayerTarget_Addictive;
        public static RenderTarget2D BeforeDustTarget_Addictive;

        public static List<IPixelatedRenderer> BeforePlayers = [];
        public static bool BeginDrawBeforePlayers = false;
        public static List<IPixelatedRenderer> BeforeDusts = [];
        public static bool BeginDrawBeforeDusts = false;
        public static List<IPixelatedRenderer> BeforeProjs = [];
        public static bool BeginDrawBeforeProjs = false;

        public static List<IPixelatedRenderer> BeforePlayers_Addictive = [];
        public static bool BeginDrawBeforePlayers_Addictive = false;
        public static List<IPixelatedRenderer> BeforeDusts_Addictive = [];
        public static bool BeginDrawBeforeDusts_Addictive = false;
        // public static Matrix PixelRenderMatrix;
        public override void Load()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                BeforePlayerTarget = KivotosMethods.NewRT2D();
                BeforeDustTarget = KivotosMethods.NewRT2D();
                BeforeProjTarget = KivotosMethods.NewRT2D();

                BeforePlayerTarget_Addictive = KivotosMethods.NewRT2D();
                BeforeDustTarget_Addictive = KivotosMethods.NewRT2D();
            });
            On_Main.CheckMonoliths += PrepareRenderTarget;
        }
        public override void Unload()
        {
            if (Main.dedServ)
                return;
            Main.QueueMainThreadAction(() =>
            {
                BeforePlayerTarget?.Dispose();
                BeforePlayerTarget = null;
                BeforeDustTarget?.Dispose();
                BeforeDustTarget = null;
                BeforeProjTarget?.Dispose();
                BeforeProjTarget = null;

                BeforeDustTarget_Addictive?.Dispose();
                BeforeDustTarget_Addictive = null;
                BeforePlayerTarget_Addictive?.Dispose();
                BeforePlayerTarget_Addictive = null;
            });
            On_Main.CheckMonoliths -= PrepareRenderTarget;
        }
        public static void PrepareRenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            if (Main.dedServ || Main.gameMenu)
            {
                orig();
                return;
            }
            orig();
            // 收集所有接口的信息
            BeforePlayers.Clear();
            BeforeDusts.Clear();
            BeforeProjs.Clear();
            BeforePlayers_Addictive.Clear();
            BeforeDusts_Addictive.Clear();
            // 创建像素化矩阵，暂时用不到
            // float pixelScale = 2f;
            // Matrix shrinkMatrix = Matrix.CreateScale(1f / pixelScale, 1f / pixelScale, 1f);
            if (BeginDrawProj)
            {
                // 检查所有弹幕，如果弹幕继承了接口，那就会把这个添加到对应图层表单中
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.ModProjectile != null && projectile.ModProjectile is IPixelatedRenderer pRPlayer)
                    {
                        if (pRPlayer.BlendState == BlendState.AlphaBlend)
                        {
                            if (pRPlayer.LayerToRenderTo.HasFlag(KivotosDrawLayer.BeforePlayer))
                                BeforePlayers.Add(pRPlayer);
                            if (pRPlayer.LayerToRenderTo.HasFlag(KivotosDrawLayer.BeforeDusts))
                                BeforeDusts.Add(pRPlayer);
                            if (pRPlayer.LayerToRenderTo.HasFlag(KivotosDrawLayer.BeforeProjectiles))
                                BeforeProjs.Add(pRPlayer);
                        }
                        if (pRPlayer.BlendState == BlendState.Additive)
                        {
                            if (pRPlayer.LayerToRenderTo.HasFlag(KivotosDrawLayer.BeforePlayer))
                                BeforePlayers_Addictive.Add(pRPlayer);
                            if (pRPlayer.LayerToRenderTo.HasFlag(KivotosDrawLayer.BeforeDusts))
                                BeforeDusts_Addictive.Add(pRPlayer);

                        }
                    }
                }

                //收集到绘制到玩家图层前的才绘制
                if (BeforePlayers.Count != 0)
                {
                    DrawToRenderTarget(BeforePlayerTarget, BeforePlayers);
                    BeginDrawBeforePlayers = true;// 打一个可以绘制出来玩家层的标记
                }
                //收集到绘制到粒子图层前的才绘制
                if (BeforeDusts.Count != 0)
                {
                    DrawToRenderTarget(BeforeDustTarget, BeforeDusts);
                    BeginDrawBeforeDusts = true;// 打一个可以绘制出来粒子层的标记
                }
                if (BeforeProjs.Count != 0)
                {
                    DrawToRenderTarget(BeforeProjTarget, BeforeProjs);
                    BeginDrawBeforeProjs = true;// 打一个可以绘制出来弹幕层的标记
                }
                if (BeforeDusts_Addictive.Count != 0)
                {
                    DrawToRenderTarget_Addictive(BeforeDustTarget_Addictive, BeforeDusts_Addictive);
                    BeginDrawBeforeDusts_Addictive = true;
                }
                if (BeforePlayers_Addictive.Count != 0)
                {
                    DrawToRenderTarget_Addictive(BeforePlayerTarget_Addictive, BeforePlayers_Addictive);
                    BeginDrawBeforePlayers_Addictive = true;
                }
                Main.graphics.GraphicsDevice.SetRenderTarget(null);
                BeginDrawProj = false;
            }
        }
        public static void DrawToRenderTarget(RenderTarget2D renderTarget, List<IPixelatedRenderer> pixelPrimitives)
        {
            renderTarget.SwapToTarget();
            if (pixelPrimitives.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
                foreach (var pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.RenderPixelated(Main.spriteBatch);
                Main.spriteBatch.End();
            }
        }
        public static void DrawToRenderTarget_Addictive(RenderTarget2D renderTarget, List<IPixelatedRenderer> pixelPrimitives)
        {
            renderTarget.SwapToTarget();
            if (pixelPrimitives.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);
                foreach (var pixelPrimitiveDrawer in pixelPrimitives)
                    pixelPrimitiveDrawer.RenderPixelated(Main.spriteBatch);
                Main.spriteBatch.End();
            }
        }

        public static void On_Main_DrawProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            // 只有当前面标记启用时才会尝试画出
            if (BeginDrawBeforeProjs)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = KivotosShaderAssets.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(KivotosMethods.GetScreenSize / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforeProjTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforeProjs = false;
            }
            orig(self);
        }


        public static void DrawTarget_BeforePlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            // 只有当前面标记启用时才会尝试画出
            if (BeginDrawBeforePlayers)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = KivotosShaderAssets.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(KivotosMethods.GetScreenSize / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforePlayerTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforePlayers = false;
            }
            if (BeginDrawBeforePlayers_Addictive)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = KivotosShaderAssets.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(KivotosMethods.GetScreenSize / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforePlayerTarget_Addictive, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforePlayers_Addictive = false;
            }

            orig(self);
        }
        public static void DrawTarget_BeforeDust(On_Main.orig_DrawDust orig, Main self)
        {
            // 只有当前面标记启用时才会尝试画出
            if (BeginDrawBeforeDusts)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = KivotosShaderAssets.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(KivotosMethods.GetScreenSize / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforeDustTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforeDusts = false;
            }
            if (BeginDrawBeforeDusts_Addictive)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
                Effect effect = KivotosShaderAssets.Pixelation;
                effect.Parameters["uTargetResolution"].SetValue(KivotosMethods.GetScreenSize / 2);
                effect.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(BeforeDustTarget_Addictive, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.End();
                BeginDrawBeforeDusts_Addictive = false;
            }
            orig(self);
        }
    }
}
