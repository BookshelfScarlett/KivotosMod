using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class ShinyCrossStareSmall : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData particleDate)
        {
        }
        public override void Update(ref ECSParticleData data)
        {
            //data.Scale *= .98f;
            data.Opacity *= Lerp(1f, 0f, EasingFunction.EaseInCubic(data.LifetimeRatio));
            data.Velocity *= .95f;
            data.Rotation += data.aifloat0;
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D star = KivotosTextureAssets.Particle_SharpTear;
            Vector2 pos = data.Position - Main.screenPosition;
            Vector2 starScale = new Vector2(.45f, 1f);
            //Vector2 vector = new Vector2(dust.scale / 4f, dust.scale / 2f) * num3;
            Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity, data.Rotation, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White * data.Opacity, data.Rotation, star.Size() / 2, starScale * data.Scale * .5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity, data.Rotation + PiOver2, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White * data.Opacity, data.Rotation + PiOver2, star.Size() / 2, starScale * data.Scale * .5f, SpriteEffects.None, 0);
        }
        public float GetAlphaFade(float t)
        {
            return Lerp(0.5f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.2f, 0.05f);
            return Vector2.Lerp(beginScale, starScale, t);
        }
    }
}
