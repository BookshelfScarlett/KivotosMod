using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores.ParticleSystem
{
    public partial class BaseParticleManager : ModSystem
    {
        /// <summary>
        /// 给list设定硬性上限，三万
        /// <para>通常情况下应该不会超出这个三万上限</para>
        /// 这样主要是为了保证内存连续
        /// </summary>

        private const int MaxParticleCountsSet = 30002;
        public static readonly List<BaseParticle> ActiveParticlesAlpha = new(MaxParticleCountsSet);
        public static readonly List<BaseParticle> ActiveParticlesNonPremultiplied = new List<BaseParticle>(MaxParticleCountsSet);
        public static readonly List<BaseParticle> ActiveParticlesAdditive = new(MaxParticleCountsSet);
        // 先绘制先更新的粒子
        public static readonly List<BaseParticle> PriorityActiveParticlesAlpha = new List<BaseParticle>(MaxParticleCountsSet);
        public static readonly List<BaseParticle> PriorityActiveParticlesNonPremultiplied = new List<BaseParticle>(MaxParticleCountsSet);
        public static readonly List<BaseParticle> PriorityActiveParticlesAdditive = new List<BaseParticle>(MaxParticleCountsSet);

        public int TotalDust;
        #region 加载卸载
        // 扔给统一的管理了
        public override void Load()
        {
            //On_Main.DrawDust += DrawParticles;
        }
        public override void Unload()
        {
            //On_Main.DrawDust -= DrawParticles;
        }
        #endregion
        /// <summary>
        /// 清除世界状态时调用（例如退出世界时）。
        /// </summary>
        public override void ClearWorld()
        {
            PriorityActiveParticlesAlpha.Clear();
            PriorityActiveParticlesNonPremultiplied.Clear();
            PriorityActiveParticlesAdditive.Clear();

            ActiveParticlesAlpha.Clear();
            ActiveParticlesNonPremultiplied.Clear();
            ActiveParticlesAdditive.Clear();
        }

        // 粒子更新
        public override void PostUpdateDusts()
        {
            UpdatePriorityParticles();
            UpdateParticles();
            TotalDust = ActiveParticlesAlpha.Count + ActiveParticlesNonPremultiplied.Count + ActiveParticlesAdditive.Count + PriorityActiveParticlesAlpha.Count + PriorityActiveParticlesNonPremultiplied.Count + PriorityActiveParticlesAdditive.Count;
        }
        // 绘制粒子
        public static void DrawParticles(On_Main.orig_DrawDust orig, Main self)
        {
            // 调用源
            orig(self);
            #region 渲染粒子
            #region 渲染优先粒子
            if (PriorityActiveParticlesAlpha.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesAlpha.Count; i++)
                {
                    PriorityActiveParticlesAlpha[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (PriorityActiveParticlesAdditive.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesAdditive.Count; i++)
                {
                    PriorityActiveParticlesAdditive[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (PriorityActiveParticlesNonPremultiplied.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < PriorityActiveParticlesNonPremultiplied.Count; i++)
                {
                    PriorityActiveParticlesNonPremultiplied[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #region 渲染常规粒子
            if (ActiveParticlesAlpha.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesAlpha.Count; i++)
                {
                    ActiveParticlesAlpha[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ActiveParticlesAdditive.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesAdditive.Count; i++)
                {
                    ActiveParticlesAdditive[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            if (ActiveParticlesNonPremultiplied.Count != 0)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < ActiveParticlesNonPremultiplied.Count; i++)
                {
                    ActiveParticlesNonPremultiplied[i].Draw(Main.spriteBatch);
                }
                Main.spriteBatch.End();
            }
            #endregion
            #endregion
        }
    }
}
