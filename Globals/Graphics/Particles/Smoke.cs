using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticleSystem;
using ReLogic.Content;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.Particles
{
    public class SmokeParticle : BaseParticle
    {
        public override BlendState UseBlendState => BlendState.AlphaBlend;
        public bool UseAlt;
        public SmokeParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useAlt = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Rotation = Rot;
            Opacity = opacity;
            Scale = scale;
            UseAlt = useAlt;
        }
        public override void OnSpawn()
        {
        }

        public override void Update()
        {
            Velocity *= 0.9f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Asset<Texture2D> texture = UseAlt ? KivotosTextureAssets.Particle_SmokeAlt.Texture : KivotosTextureAssets.Particle_Smoke.Texture;

            Rectangle frame = texture.Frame(4, 4, (int)(LifetimeRatio * 16) % 4, (int)(LifetimeRatio * 4));
            Vector2 origin = frame.Size() * 0.5f;
            spriteBatch.Draw(texture.Value, Position - Main.screenPosition, frame, DrawColor * brightness * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
