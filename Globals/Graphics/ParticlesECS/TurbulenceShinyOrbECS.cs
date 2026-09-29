using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class TurbulenceShinyOrb : ECSParticleBehavior
    {
        /// <summary>
        /// AI0是湍流的速度
        /// </summary>
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aifloat2 = data.Scale;
            data.aiint0 = Main.rand.Next(0, 100000);
        }
        public override void Update(ref ECSParticleData data)
        {
            float Speed = data.aifloat0;
            if (Speed != 0)
            {
                Vector2 idealVelocity = -Vector2.UnitY.RotatedBy(Lerp(-data.Rotation, data.Rotation, (float)Math.Sin(data.Time / 36f + data.aiint0) * 0.5f + 0.5f)) * Speed;
                float movementInterpolant = Lerp(0.01f, 0.25f, Utils.GetLerpValue(0, data.Lifetime / 2, data.Time, true));
                data.Velocity = Vector2.Lerp(data.Velocity, idealVelocity, movementInterpolant);
                data.Velocity = data.Velocity.SafeNormalize(-Vector2.UnitY) * Speed;
            }
            data.Velocity *= 0.9f;
            data.Scale = Lerp(data.aifloat2, 0, EasingFunction.EaseOutCubic(data.LifetimeRatio));
        }
        public override void Draw(ref ECSParticleData data)
        {
            Texture2D texture = KivotosTextureAssets.Particle_HRShinyOrbSmall.Value;
            Main.spriteBatch.Draw(texture, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation, texture.Size() / 2, data.Scale, SpriteEffects.None, 0);
            if (data.aifloat1 > 0)
                Main.spriteBatch.Draw(texture, data.Position - Main.screenPosition, null, Color.White * data.Opacity, data.Rotation, texture.Size() / 2, data.Scale * data.aifloat1, SpriteEffects.None, 0);
        }
    }
}
