using KivotosMod.Assets.Register;
using KivotosMod.Cores.MetaballSystem;
using System.Collections.Generic;
using Terraria;

namespace KivotosMod.Globals.Graphics.Metaballs
{
    public class BloodyMetaball : BaseMetaball
    {
        public class BloodMetaballParticle(Vector2 center, Vector2 velocity, float scale, float rot, bool UseBall)
        {
            public float Scale = scale;
            public Vector2 Velocity = velocity;
            public Vector2 Center = center;
            public float Rot = rot;
            public bool UseBall = UseBall;
            public void Update()
            {
                Center += Velocity;
                Velocity *= 0.9f;

                if (UseBall)
                    Scale *= 0.9f;
                else
                    Scale *= 0.96f;
            }
        }
        public override int MetaballTimer => 64;
        public override Color EdgeColor => Color.Lerp(Color.Red, Color.DarkRed, .54f) with { A = 255 };
        public static List<BloodMetaballParticle> Particles = [];
        public override Texture2D BackgroundTexture => KivotosTextureAssets.Noise_Misc.Value;
        public static void SpawnParticle(Vector2 position, Vector2 velocity, float size, float rot, bool UseBall = false) => Particles.Add(new(position, velocity, size, rot, UseBall));
        public override bool Active()
        {
            if (Particles.Count == 0)
                return false;
            else
                return true;
        }

        public override void Update()
        {
            for (int i = 0; i < Particles.Count; i++)
            {
                Particles[i].Update();
            }

            // 移除生命周期已结束的粒子
            Particles.RemoveAll(particle =>
            {
                if (particle.Scale < 0.01f)
                {
                    return true;
                }
                return false;
            });
        }

        public override void PrepareRenderTarget()
        {
            if (Particles.Count != 0)
            {
                for (int i = 0; i < Particles.Count; i++)
                {
                    if (Particles[i].UseBall)
                    {
                        Main.spriteBatch.Draw(KivotosTextureAssets.Texture_WhiteCircle.Value, Particles[i].Center - Main.screenPosition, null, Color.White, Particles[i].Rot, KivotosTextureAssets.Texture_WhiteCircle.Value.Size() / 2, Particles[i].Scale, SpriteEffects.None, 0f);
                        continue;
                    }

                    Main.spriteBatch.Draw(KivotosTextureAssets.Texture_BloodStain.Value, Particles[i].Center - Main.screenPosition, null, Color.White, Particles[i].Rot, KivotosTextureAssets.Texture_BloodStain.Value.Size() / 2, Particles[i].Scale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
