using KivotosMod.Assets.Register;
using KivotosMod.Cores.MetaballSystem;
using System.Collections.Generic;
using Terraria;

namespace KivotosMod.Globals.Graphics.Metaballs
{
    public class ShadowNebulaVector2 : BaseMetaball
    {
        public class ShadowNebulaParticleVector2(Vector2 center, Vector2 vel, Vector2 scale, float rotation, int lifeTime, Texture2D shapeTex)
        {
            public Vector2 Scale = scale;
            public Vector2 Velocity = vel;
            public Vector2 Center = center;
            public Texture2D ShapeTex = shapeTex;
            public int LifeTime = lifeTime;
            public float Rotation = rotation;
            public int Timer = 0;
            public void Update()
            {
                Center += Velocity;
                Velocity *= 0.9f;
                Scale *= 0.96f;
                Timer++;
            }
        }
        public override Color EdgeColor => Color.Lerp(Color.DarkViolet, Color.WhiteSmoke, 0.4f);
        public static List<ShadowNebulaParticleVector2> ParticleList = [];
        public override Texture2D BackgroundTexture => KivotosTextureAssets.Metaball_ShadowNebula.Value;
        public static void SpawnParticle(Vector2 pos, Vector2 vel, Vector2 scale, float rotation, int lifeTime, Texture2D shapeTex) => ParticleList.Add(new(pos, vel, scale, rotation, lifeTime, shapeTex));
        public override bool Active()
        {
            return ParticleList.Count != 0;
        }
        public override void Update()
        {
            for (int i = 0; i < ParticleList.Count; i++)
                ParticleList[i].Update();
            ParticleList.RemoveAll(particleList =>
            {
                return particleList.Timer / (float)particleList.LifeTime >= 1f;
            });
        }
        public override void PrepareRenderTarget()
        {
            if (ParticleList.Count == 0)
                return;
            for (int i = 0; i < ParticleList.Count; i++)
            {
                Main.spriteBatch.Draw(ParticleList[i].ShapeTex, ParticleList[i].Center - Main.screenPosition, null, Color.White, ParticleList[i].Rotation, ParticleList[i].ShapeTex.Size() / 2f, ParticleList[i].Scale, 0, 0);
            }
        }
    }
    public class ShadowNebula : BaseMetaball
    {
        public class ShadowNebulaParticle(Vector2 center, Vector2 vel, float scale, Texture2D shapeTex)
        {
            public float Scale = scale;
            public Vector2 Velocity = vel;
            public Vector2 Center = center;
            public Texture2D ShapeTex = shapeTex;
            public void Update()
            {
                Center += Velocity;
                Velocity *= 0.9f;
                Scale *= 0.95f;
            }
        }
        public override Color EdgeColor => Color.Lerp(Color.DarkViolet, Color.WhiteSmoke, 0.3f);
        public static List<ShadowNebulaParticle> ParticleList = [];
        public override Texture2D BackgroundTexture => KivotosTextureAssets.Metaball_ShadowNebula.Value;
        public static void SpawnParticle(Vector2 pos, Vector2 vel, float scale, Texture2D shapeTex) => ParticleList.Add(new(pos, vel, scale, shapeTex));
        public override bool Active()
        {
            return ParticleList.Count != 0;
        }
        public override void Update()
        {
            for (int i = 0; i < ParticleList.Count; i++)
                ParticleList[i].Update();
            ParticleList.RemoveAll(particleList =>
            {
                return particleList.Scale < 0.011f;
            });
        }
        public override void PrepareRenderTarget()
        {
            if (ParticleList.Count == 0)
                return;
            for (int i = 0; i < ParticleList.Count; i++)
            {
                Main.spriteBatch.Draw(ParticleList[i].ShapeTex, ParticleList[i].Center - Main.screenPosition, null, Color.White, ParticleList[i].Velocity.ToRotation(), ParticleList[i].ShapeTex.Size() / 2f, ParticleList[i].Scale, 0, 0);
            }
        }
    }
}
