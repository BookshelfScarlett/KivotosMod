using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class HighResolutionThunder : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            int type = data.aiint0;

            switch (type)
            {
                case 1:
                    break;
                case 2:
                    data.aiint1 = Main.rand.Next(0, 2);
                    data.aiint2 = Main.rand.Next(0, 2);
                    break;
                default:
                    data.aiint1 = Main.rand.Next(0, 4);
                    data.aiint2 = Main.rand.Next(0, 2);
                    break;
            }
        }
        public override void Update(ref ECSParticleData data)
        {
            data.Opacity = Lerp(1, 0, EasingFunction.EaseOutCubic(data.LifetimeRatio));
            data.Opacity = MathF.Pow(data.Opacity, 0.5f);
        }
        public override void OnKill(ref ECSParticleData data)
        {
            base.OnKill(ref data);
        }
        public override void Draw(ref ECSParticleData data)
        {
            //类型
            int type = data.aiint0;
            int frameX = data.aiint1;
            int frameY = data.aiint2;
            Texture2D tex = type switch
            {
                1 => KivotosTextureAssets.Particle_Lightning1.Value,
                2 => KivotosTextureAssets.Particle_Lightning2.Value,
                _ => KivotosTextureAssets.Particle_Lightning0.Value
            };
            Rectangle rec = type switch
            {
                1 => tex.Frame(),
                2 => tex.Frame(2, 2, frameX, frameY),
                _ => tex.Frame(4, 2, frameX, frameY),
            };
            Vector2 ori = rec.Size() / 2;
            SpriteBatch sb = Main.spriteBatch;
            Vector2 pos = data.Position - Main.screenPosition;
            switch (type)
            {
                case 1:
                    sb.Draw(tex, pos, rec, data.DrawColor * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    sb.Draw(tex, pos, rec, Color.White * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    break;
                case 2:
                    sb.Draw(tex, pos, rec, Color.White * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    sb.Draw(tex, pos, rec, data.DrawColor * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    break;
                default:
                    sb.Draw(tex, pos, rec, data.DrawColor * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    sb.Draw(tex, pos, rec, data.DrawColor * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    sb.Draw(tex, pos, rec, Color.White * data.Opacity, data.Rotation, ori, data.Scale, SpriteEffects.None, 0f);
                    break;

            }
        }
    }
}
