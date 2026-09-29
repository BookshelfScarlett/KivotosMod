using KivotosMod.Cores.ParticlesECS;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores
{
    /// <summary>
    /// 模组内的绘制层系统，主要用于管理和控制模组内的绘制顺序和层级。
    /// <br>这个是一个统一管理方案</br>
    /// </summary>
    public class KivotosDrawLayers : ModSystem
    {
        public override void Load()
        {
            //ECS粒子系统
            On_Main.DrawDust += ECSParticleDataManager.DrawParticle_ECS;
        }
        public override void Unload()
        {
            On_Main.DrawDust -= ECSParticleDataManager.DrawParticle_ECS;
        }
    }
}
