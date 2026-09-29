using HJScarletRework.Core.PixelatedRender;
using KivotosMod.Cores.MetaballSystem;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.ParticleSystem;
using KivotosMod.Cores.ScreenEffect;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Cores
{
    /// <summary>
    /// 模组内的绘制层系统，主要用于管理和控制模组内的绘制顺序和层级。
    /// <br>这个是一个统一管理方案</br>
    /// </summary>
    public class KivotosDrawLayersManager : ModSystem
    {
        public override void Load()
        {
            //屏幕暗化效果
            On_Main.DrawBackground += ScreenDarknessSystem.DrawScreenDarkness;
            //Metaball层级，可以考虑直接分离出去
            On_Main.DrawDust += MetaballManager.DrawRenderTarget;
            //ECS粒子系统
            On_Main.DrawDust += ECSParticleDataManager.DrawParticle_ECS;
            //使用类射弹的实例化粒子
            On_Main.DrawDust += BaseParticleManager.DrawParticles;
            //像素化渲染
            On_Main.DrawDust += PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles += PixelatedRenderManager.DrawTarget_BeforePlayers;
        }
        public override void Unload()
        {
            On_Main.DrawBackground -= ScreenDarknessSystem.DrawScreenDarkness;
            On_Main.DrawDust -= MetaballManager.DrawRenderTarget;
            On_Main.DrawDust -= ECSParticleDataManager.DrawParticle_ECS;
            On_Main.DrawDust -= BaseParticleManager.DrawParticles;
            //像素化渲染
            On_Main.DrawDust -= PixelatedRenderManager.DrawTarget_BeforeDust;
            On_Main.DrawPlayers_AfterProjectiles -= PixelatedRenderManager.DrawTarget_BeforePlayers;
        }
    }
}
