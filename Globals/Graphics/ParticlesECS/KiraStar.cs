using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class KiraStar : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aifloat1 = data.Scale;
            if (data.aifloat0 > 0)
            {
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
                float progress = EasingFunction.EaseInBack(remaining);
                data.Scale = Lerp(beginScale, 0, progress);
            }
            data.Velocity *= .96f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool useAltTex = data.aifloat2 != 0;
            Texture2D tex = useAltTex ? KivotosTextureAssets.Particle_KiraStarGlow.Value : KivotosTextureAssets.Particle_KiraStar.Value;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, tex.Size() / 2, data.Scale, 0, 0);
        }
    }
}
