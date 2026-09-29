using KivotosMod.Assets.Register;
using KivotosMod.Content.Raritys.Helper;
using System;
using Terraria;

namespace KivotosMod.Content.Raritys.Sparkles
{

    public class RarityCube : RaritySparkle
    {
        public int BlendStateType;
        public bool GlowCenter = true;
        public float FadeOut;
        public Color InitColor;
        public float GlowCenterScale = 0.5f;
        public RarityCube(Vector2 position, Vector2 velocity, Color color, int lifeTime, float scale)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = InitColor = color;
            Lifetime = lifeTime;
            Scale = scale;
            FadeOut = 1f;
        }

        public override void CustomUpdate()
        {
            FadeOut -= 0.015f;
            Scale *= 0.99f;
            DrawColor = Color.Lerp(InitColor, InitColor * 0.2f, (float)Math.Pow(LifetimeRatio, 30));
            //减速需要更快。
            Velocity *= 0.975f;
            Position += Velocity;
        }
        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            Vector2 scale = new Vector2(1f, 1f) * Scale;
            Texture2D texture = KivotosTextureAssets.Particle_WhiteCube.Value;
            spriteBatch.Draw(texture, drawPosition, null, DrawColor, Rotation, texture.Size() * 0.5f, scale, 0, 0f);
            if (GlowCenter)
                spriteBatch.Draw(texture, drawPosition, null, Color.White * FadeOut, Rotation, texture.Size() * 0.5f, scale * GlowCenterScale, 0, 0f);
        }
    }
}
