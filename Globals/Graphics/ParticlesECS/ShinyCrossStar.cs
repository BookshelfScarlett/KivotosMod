using KivotosMod.Cores.ParticlesECS;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class ShinyCrossStar : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData particleDate)
        {
        }
        public override void Update(ref ECSParticleData data)
        {
            data.Scale *= .93f;
            data.DrawColor *= Lerp(1f, .2f, (float)Math.Pow(data.LifetimeRatio, 30));
            data.Velocity *= .95f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D star = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 pos = data.Position - Main.screenPosition;
            float drawMinor = data.aifloat0;
            for (float i = 0; i < 1f; i += drawMinor)
            {
                Vector2 starScale = GetScale(i);
                float colorAlpha = GetAlphaFade(1 - i);
                Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity * colorAlpha, 0, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(star, pos, null, data.DrawColor * data.Opacity * colorAlpha, 0 + MathHelper.PiOver2, star.Size() / 2, starScale * data.Scale, SpriteEffects.None, 0);
            }

        }
        public float GetAlphaFade(float t)
        {
            return MathHelper.Lerp(0.5f, 1f, t);
        }
        public Vector2 GetScale(float t)
        {
            Vector2 starScale = new(1.2f, 0.8f);
            Vector2 beginScale = new(0.2f, 0.05f);
            return Vector2.Lerp(beginScale, starScale, t);
        }
    }
}
