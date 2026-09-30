using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class CrossGlow : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
            if (data.aifloat0 > 0)
            {
                data.aifloat1 = data.Scale;
                data.Scale = 0;
            }
        }
        public override void Update(ref ECSParticleData data)
        {
            float fadeInTime = data.aifloat0;
            float beginScale = data.aifloat1;
            fadeInTime = Clamp(fadeInTime, 0.0f, 1.0f);
            if (data.LifetimeRatio < fadeInTime && fadeInTime > 0)
            {
                //归一化
                float progress = data.LifetimeRatio / fadeInTime;
                data.Scale = Lerp(0f, beginScale, EasingFunction.EaseOutCubic(progress));
            }
            else
            {
                //归一化进度=(当前比例 - 淡入比例) / (1 - 淡入比例)
                float remaining = (data.LifetimeRatio - fadeInTime) / (1f - fadeInTime);
                //防止为0
                remaining = Clamp(remaining, 0f, 1f);
                data.Scale = Lerp(data.Scale, 0, remaining);
            }
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D tex = data.aibool0 ? KivotosTextureAssets.Particle_OpticalLineGlow.Value : KivotosTextureAssets.Particle_CrossGlow.Value;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, tex.Size() / 2, data.Scale, 0, 0);
        }
    }
}
