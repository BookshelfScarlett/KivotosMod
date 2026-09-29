using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class HRShinyOrb : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void Update(ref ECSParticleData data)
        {
            data.Velocity *= .92f;
            data.Scale = MathHelper.Lerp(data.Scale, 0, EasingFunction.EaseInCubic(data.LifetimeRatio));
        }
        public override void Draw(ref ECSParticleData data)
        {
            //ai0用于表示中心辉光大小。
            float glowMult = data.aifloat0;
            //这里每帧的request是临时方案，因为我也懒得继续搞了（
            Texture2D orb = KivotosTextureAssets.Particle_HRShinyOrbSmall.Value;
            Main.spriteBatch.Draw(orb, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, 0, orb.Size() / 2, data.Scale, 0, 0);
            if (glowMult > 0)
            {
                Main.spriteBatch.Draw(orb, data.Position - Main.screenPosition, null, Color.White * data.Opacity, 0, orb.Size() / 2, data.Scale * glowMult, 0, 0);
            }
        }
    }
}
