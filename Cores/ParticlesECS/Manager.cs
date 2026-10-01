using KivotosMod.Globals.Methods;
using ReLogic.Threading;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores.ParticlesECS
{
    /// <summary>
    /// ECS架构的粒子系统的数据管理器，负责管理<see cref="ECSParticleData"/>数据的生命周期和更新。
    /// <br>比面向对象的粒子系统更高效，适合大量粒子的场景。</br>
    /// <br>运用得当的话可以得到比ModDust系统更好的性能，并且比ModDust的使用更自由</br>
    /// </summary>
    public class ECSParticleDataManager : ModSystem
    {
        public const int MaxParticle = 20000;
        public static List<ECSParticleBehavior> PAICollection = [];
        public static ECSParticleData TempleParticle = new ECSParticleData();
        public static ECSParticleData[] particleData_alpha = new ECSParticleData[MaxParticle];
        public static int activePoint_alpha;
        public static ECSParticleData[] particleData_add = new ECSParticleData[MaxParticle];
        public static int activePoint_add;
        public static ECSParticleData[] particleData_nopremult = new ECSParticleData[MaxParticle];
        public static int activePoint_Nonmult;
        public override void Load()
        {
            activePoint_alpha = 0;
            activePoint_add = 0;
            activePoint_Nonmult = 0;
            particleData_alpha = new ECSParticleData[MaxParticle];
            particleData_add = new ECSParticleData[MaxParticle];
            particleData_nopremult = new ECSParticleData[MaxParticle];
        }
        public override void Unload()
        {
            //On_Main.DrawDust -= DrawParticle;
            activePoint_alpha = 0;
            activePoint_add = 0;
            activePoint_Nonmult = 0;
            particleData_alpha = null;
            particleData_add = null;
            particleData_nopremult = null;
            PAICollection.Clear();
        }
        public override void PostUpdateDusts()
        {
            UpdateParticle(ref activePoint_alpha, particleData_alpha);
            UpdateParticle(ref activePoint_add, particleData_add);
            UpdateParticle(ref activePoint_Nonmult, particleData_nopremult);
        }
        public static void UpdateParticle(ref int point, ECSParticleData[] particleDates)
        {
            if (point == 0)
                return;
            FastParallel.For(0, point, (j, k, callback) =>
            {
                for (int i = j; i < k; i++)
                {
                    ref ECSParticleData particle = ref particleDates[i];
                    if (particle.Active)
                    {
                        PAICollection[particle.Type].Update(ref particle);
                        particle.Position += particle.Velocity;
                        particle.Time++;
                        // 交换删除
                        if (particle.Time >= particle.Lifetime)
                            particle.Active = false;
                    }
                }
            });
            for (int i = 0; i < point; i++)
            {
                if (!particleDates[i].Active)
                {
                    PAICollection[particleDates[i].Type].OnKill(ref particleDates[i]);
                    particleDates[i] = particleDates[point - 1];
                    point--;
                }
            }
        }
        public static void DrawParticle_ECS(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            DrawParticles(activePoint_alpha, particleData_alpha, BlendState.AlphaBlend);
            DrawParticles(activePoint_add, particleData_add, BlendState.Additive);
            DrawParticles(activePoint_Nonmult, particleData_nopremult, BlendState.NonPremultiplied);
        }
        public static void DrawParticles(int point, ECSParticleData[] particleDates, BlendState bl)
        {
            if (point == 0)
                return;
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, bl, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < point; i++)
            {
                ECSParticleData particle = particleDates[i];
                if (KivotosMethods.OutOffScreen(particle.Position, particle.ScreenCut))
                    continue;
                PAICollection[particle.Type].Draw(ref particle);
            }
            Main.spriteBatch.End();
        }
    }
}
