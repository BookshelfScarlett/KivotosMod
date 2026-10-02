using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticleSystem;
using KivotosMod.Globals.Methods;
using ReLogic.Content;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.Particles
{
    /// <summary>
    /// 专门给伊吹使用的粒子
    /// <br>暂时不考虑使用ECS提升性能，用面向对象的粒子更简单一些</br>
    /// </summary>
    public class IbukiCuteSymbol : BaseParticle
    {
        public override BlendState UseBlendState => BlendState.AlphaBlend;
        public bool UseAlt;
        public IbukiCuteSymbol(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useAlt = false)
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
            Velocity *= 0.93f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), LifetimeRatio);
            Scale = Lerp(Scale, Lerp(Scale, 0, .2f), LifetimeRatio);
            if (!UseAlt)
            {
                Rotation += .1f * Math.Sign(Velocity.X);
            }
            else
            {
                Velocity *= 0.98f;
                float rotOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5) * .03f;
                //最终旋转角度
                float finalRotation = Rotation + rotOffset;
                Rotation = finalRotation;
            }
        }
        // 这里采样没有问题，他贴图就是这样
        public override void Draw(SpriteBatch spriteBatch)
        {
            Asset<Texture2D> texture = UseAlt ? KivotosTextureAssets.Particle_CuteMusic.Texture : KivotosTextureAssets.Particle_CuteStar.Texture;

            Vector2 origin = texture.Size() * 0.5f;
            Vector2 pos = Position - Main.screenPosition;
            for (int i = 0; i < 8; i++)
                spriteBatch.Draw(texture.Value, pos + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, null, DrawColor.ToAddColor() * Opacity, Rotation, origin, Scale, 0, 0f);
            spriteBatch.Draw(texture.Value, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, origin, Scale, 0, 0f);
        }

    }
}
