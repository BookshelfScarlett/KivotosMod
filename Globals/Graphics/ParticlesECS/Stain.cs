using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class Stain : ECSParticleBehavior
    {
        public override void Update(ref ECSParticleData data)
        {
            data.Scale *= .96f;
            data.DrawColor *= Lerp(1, 0, data.LifetimeRatio);
            data.Velocity *= .96f;
            float shrinkSpeedX = data.aifloat0;
            float shrinkSpeedY = data.aifloat1;
            if (shrinkSpeedX != 0)
                data.Scale2.X *= shrinkSpeedX;
            if (shrinkSpeedY != 0)
                data.Scale2.Y *= shrinkSpeedY;
        }
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D tex = KivotosTextureAssets.Texture_BloodStain.Value;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, tex.Size() / 2f, data.Scale2 * data.aifloat2, 0, 0);
        }
    }
    public class ShrinkParticle : ECSParticleBehavior
    {
        public override void Update(ref ECSParticleData data)
        {
            data.Scale *= .96f;
            data.DrawColor *= Lerp(1, 0, data.LifetimeRatio);
            data.Velocity *= .96f;
            float shrinkSpeedX = data.aifloat0;
            float shrinkSpeedY = data.aifloat1;
            if (shrinkSpeedX != 0)
                data.Scale2.X *= shrinkSpeedX;
            if (shrinkSpeedY != 0)
                data.Scale2.Y *= shrinkSpeedY;
        }
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D tex = KivotosTextureAssets.Particle_BloodDrop.Value;
            int type = data.aiint0;
            if (type == 1)
                tex = KivotosTextureAssets.Particle_HRShinyOrbSmall.Value;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, tex.Size() / 2f, data.Scale2 * data.aifloat2, 0, 0);
        }
    }

}
