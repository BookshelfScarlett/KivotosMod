using KivotosMod.Globals.Graphics.ParticlesECS;

namespace KivotosMod.Cores.ParticlesECS
{
    /// <summary>
    /// 粒子ECS的快捷方法类，提供了创建和管理粒子实体的静态方法。
    /// <para>如果你不想在每次生成的时候都不知道自己在生成什么，最好创建粒子的时候，就往这里注册一个对应粒子的工具方法</para>
    /// </summary>
    public static class ECSParticle
    {
        /// <summary>
        /// <para><paramref name="smallDrawMult"/>用于表示重复绘制的次数，该十字星是用ex98重复绘制多次组起来的 </para>
        /// <para>更高的值代表更少的绘制次数，对于及其巨量的粒子生成情况下，减少绘制次数可以提升性能</para>
        /// </summary>
        /// <returns></returns>
        public static int ShinyCrossStarECS(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float smallDrawMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<ShinyCrossStar>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: smallDrawMult);
        }
        public static int HRShinyOrb(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float glowMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<HRShinyOrb>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: glowMult);
        }
        public static int SmokeParticle(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rot, float opacity, float scale, bool alt = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            float Ai0 = alt ? 1 : 0;
            return ECSMethod.NewParticle(GetInstance<SmokeParticle>().Type, timeLeft, pos, vel, color, opacity, rot, scale, blendstate: bs, ai0: Ai0);
        }
        /// <summary>
        /// <para><paramref name="drawTime"/>用于表示重复绘制的次数，该粒子是一个重复绘制多次的ex98长棱形</para>
        /// <para>更高的值代表更少的绘制次数，对于及其巨量的粒子生成情况下，减少绘制次数可以提升性能</para>
        /// <para>当然，同样会缩短这个棱形的长度</para>
        /// </summary>
        /// <returns></returns>
        public static int LightntingGlow(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, int drawTime = 6, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<LightningGlowParticle>().Type, timeLeft, pos, vel, color, opacity, 0, scale: scale, blendstate: bs, ai0: drawTime);

        }
    }
}
