using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores.MetaballSystem
{
    public class MetaballManager : ModSystem
    {
        public static List<BaseMetaball> MetaballList = [];

        #region 加载卸载
        public override void Load()
        {
            if (Main.dedServ)
                return;

            On_Main.CheckMonoliths += PrepareRenderTarget;
        }

        public override void Unload()
        {
            if (Main.dedServ)
                return;

            Main.QueueMainThreadAction(() =>
            {
                // 卸载资源
                foreach (BaseMetaball baseMetaBall in MetaballList)
                {
                    baseMetaBall.AlphaTexture?.Dispose();
                    baseMetaBall.AlphaTexture = null;
                }
            });
            MetaballList.Clear();

            On_Main.CheckMonoliths -= PrepareRenderTarget;
        }
        #endregion

        #region 更新每一个元球
        public override void PostUpdateDusts()
        {
            foreach (BaseMetaball baseMetaBall in MetaballList)
            {
                if (!baseMetaBall.Active())
                    continue;

                baseMetaBall.Update();
            }
        }
        #endregion

        #region 进行离屏渲染
        public static void PrepareRenderTarget(On_Main.orig_CheckMonoliths orig)
        {
            if (Main.dedServ)
            {
                orig();
                return;
            }

            if (Main.gameMenu)
            {
                orig();
                return;
            }

            orig();

            foreach (BaseMetaball baseMetaBall in MetaballList)
            {
                if (!baseMetaBall.Active())
                    continue;
                if (baseMetaBall.SetPority)
                    continue;

                KivotosMethods.SwapToTarget(baseMetaBall.AlphaTexture);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

                baseMetaBall.PrepareRenderTarget();

                Main.spriteBatch.End();

                Main.graphics.GraphicsDevice.SetRenderTargets(null);
            }
            foreach (BaseMetaball baseMetaBall1 in MetaballList)
            {
                if (!baseMetaBall1.Active())
                    continue;
                if (!baseMetaBall1.SetPority)
                    continue;
                KivotosMethods.SwapToTarget(baseMetaBall1.AlphaTexture);

                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null);

                baseMetaBall1.PrepareRenderTarget();

                Main.spriteBatch.End();

                Main.graphics.GraphicsDevice.SetRenderTargets(null);

            }
        }
        #endregion

        #region 最终输出渲染
        public static void DrawRenderTargetPiority(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            if (Main.dedServ)
            {
                orig(self);
                return;
            }

            orig(self);

            foreach (BaseMetaball baseMetaBall in MetaballList)
            {
                if (!baseMetaBall.Active())
                    continue;
                if (!baseMetaBall.SetPority)
                    continue;
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                if (baseMetaBall.PreDrawRT2D())
                {
                    baseMetaBall.PrepareShader();
                    Main.spriteBatch.Draw(baseMetaBall.AlphaTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }

                Main.spriteBatch.End();
            }

        }

        public static void DrawRenderTarget(On_Main.orig_DrawDust orig, Main self)
        {
            if (Main.dedServ)
            {
                orig(self);
                return;
            }

            orig(self);

            foreach (BaseMetaball baseMetaBall in MetaballList)
            {
                if (!baseMetaBall.Active())
                    continue;
                if (baseMetaBall.SetPority)
                    continue;

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                if (baseMetaBall.PreDrawRT2D())
                {
                    baseMetaBall.PrepareShader();
                    Main.spriteBatch.Draw(baseMetaBall.AlphaTexture, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                }

                Main.spriteBatch.End();
            }
        }
        #endregion
    }
}