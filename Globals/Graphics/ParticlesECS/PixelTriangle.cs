using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class PixelTriangle : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData particleDate)
        {
            if (particleDate.aifloat0 > 0)
            {
                particleDate.aifloat1 = particleDate.Scale;
                particleDate.Scale = 0;
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
                //data.Scale = Lerp(data.Scale, 0, remaining);
                data.Opacity = Lerp(data.Opacity, 0, EasingFunction.EaseInCubic(remaining));
            }
            //data.DrawColor *= Lerp(1f, .2f, (float)Math.Pow(data.LifetimeRatio, 30));
            data.Velocity *= .98f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            int type = data.aiint0;
            Texture2D star = type switch
            {
                1 => KivotosTextureAssets.Particle_PixelTriangle.Value,
                _ => KivotosTextureAssets.Particle_PixelTriangle.Value,
            };
            Vector2 pos = data.Position - Main.screenPosition;
            Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity, data.Rotation, star.Size() / 2f, data.Scale, 0, 0);
        }
    }
}
