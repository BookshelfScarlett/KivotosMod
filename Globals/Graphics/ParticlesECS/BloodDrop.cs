using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Methods;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class BloodDrop : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void OnKill(ref ECSParticleData data)
        {
            base.OnKill(ref data);
        }
        public override void Update(ref ECSParticleData data)
        {
            if (data.Velocity.Y < 0)
            {
                data.Velocity.Y = Lerp(data.Velocity.Y, 0, .12f);
            }
            else
            {
                data.Velocity.Y *= 1.1f;
                if (data.Velocity.Y > 16f)
                    data.Velocity.Y = 16;
            }
            if (!data.aibool1 && Math.Abs(data.Velocity.Y) < .4f)
            {
                data.aibool1 = true;
                data.Velocity.Y = .4f;

            }
            data.Velocity.X *= .9f;
            data.Rotation = data.Velocity.ToRotation();
            data.Scale *= .97f;
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool fullBright = data.aifloat0 != 0;
            int brightEdgeMult = (int)data.aifloat1;
            float brightness = (float)Math.Pow(Lighting.Brightness((int)(data.Position.X / 16f), (int)(data.Position.Y / 16f)), 0.15);
            Color c = fullBright ? data.DrawColor : data.DrawColor * brightness;
            Texture2D tex = KivotosTextureAssets.Particle_BloodDrop.Value;
            Vector2 pos = data.Position - Main.screenPosition;
            Vector2 ori = tex.Size() / 2f;
            float rot = data.Rotation + PiOver2;
            if (brightEdgeMult > 0)
            {
                for (int i = 0; i < 8 * brightEdgeMult; i++)
                    Main.spriteBatch.Draw(tex, pos + (TwoPi / (float)(8 * brightEdgeMult) * i).ToRotationVector2() * 1.15f * data.Scale, null, Color.DarkRed.ToAddColor(), rot, ori, data.Scale, 0, 0);

            }
            Main.spriteBatch.Draw(tex, pos, null, c, rot, ori, data.Scale, 0, 0);
        }
    }
}
