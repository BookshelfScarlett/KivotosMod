using KivotosMod.Assets.Register;
using KivotosMod.Cores.MetaballSystem;
using System.Collections.Generic;
using Terraria;

namespace KivotosMod.Globals.Graphics.Metaballs
{
    public class ShadowNebulaAlt : BaseMetaball
    {
        public class CircleParticle(Vector2 center, Vector2 velocity, float scale, int maxTime)
        {
            public float Scale = scale;
            public float BeginScale = scale;
            public Vector2 Velocity = velocity;
            public Vector2 Center = center;
            public int Time;
            public int MaxTime = maxTime;

            public void Update()
            {
                Time++;
                Center += Velocity;
                Velocity *= 0.9f;
                Scale = Lerp(BeginScale, 0f, EasingFunction.EaseOutCubic(Time / (float)MaxTime));
            }
        }
        public class SharpTearCleanParticle(Vector2 center, Vector2 velocity, float scale, int lifeTime)
        {
            public float Scale = scale;
            public float BeginScale = scale;
            public float Rot;
            public Vector2 Velocity = velocity;
            public Vector2 Center = center;
            public int Time;
            public int LifeTime = lifeTime;
            public void Update()
            {
                Time++;
                Center += Velocity;
                Velocity *= 0.9f;
                Scale = Lerp(BeginScale, 0f, EasingFunction.EaseOutCubic(Time / (float)LifeTime));
                Rot = Velocity.ToRotation() + PiOver2;
            }
        }
        public class SharpCrossStarParticle(Vector2 center, float scale, Vector2 dir, float shortMul, float longMult, int lifeTime)
        {
            public Vector2 Scale;
            public int Time = 0;
            public int MaxTime = 20;
            public float BeginScale = scale;
            public Vector2 Center = center;
            public Vector2 Direction = dir;
            public float ShortMult = shortMul;
            public float LongMult = longMult;
            public int LifeTime = lifeTime;

            public void Update()
            {
                Time++;
                Scale = new Vector2(0.5f, 0.5f) * Lerp(0f, BeginScale, EasingFunction.EaseOutBack(Time / (float)MaxTime));
            }
            public void SetUpStar()
            {
                for (int i = 0; i < 4f; i++)
                {
                    SpawnSharpTearClean(Center, Direction * i * ShortMult, BeginScale * 2.5f, LifeTime);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnSharpTearClean(Center, -Direction * i * ShortMult, BeginScale * 2.5f, LifeTime);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnSharpTearClean(Center, Direction.RotatedBy(PiOver2) * i * LongMult, BeginScale * 2.5f, LifeTime);
                }
                for (int i = 0; i < 4f; i++)
                {
                    SpawnSharpTearClean(Center, -Direction.RotatedBy(PiOver2) * i * LongMult, BeginScale * 2.5f, LifeTime);
                }
            }

        }
        public override Color EdgeColor => Color.Lerp(Color.DarkViolet, Color.GhostWhite, 0.35f);
        public override bool SetPority => false;
        public static List<SharpTearCleanParticle> SharpTearsList = [];
        public static List<SharpCrossStarParticle> SharpCrossStarList = [];
        public static List<CircleParticle> CircleList = [];
        public override Texture2D BackgroundTexture => KivotosTextureAssets.Metaball_ShadowNebula.Value;
        public static void SpawnSharpTearClean(Vector2 pos, Vector2 vel, float scale, int lifeTime) => SharpTearsList.Add(new(pos, vel, scale, lifeTime));
        public static void SpawnSharpCrossStar(Vector2 pos, float scale, Vector2 dir, float shortMult = 4f, float longMult = 2f, int lifeTime = 60) => SharpCrossStarList.Add(new(pos, scale, dir, shortMult, longMult, lifeTime));
        public static void SpawnCircle(Vector2 pos, Vector2 vel, float scale, int lifeTime = 60) => CircleList.Add(new(pos, vel, scale, lifeTime));
        public override bool Active()
        {
            int count = SharpTearsList.Count + SharpCrossStarList.Count + CircleList.Count;
            return count != 0;
        }
        public override void Update()
        {
            for (int i = 0; i < SharpCrossStarList.Count; i++)
            {
                SharpCrossStarList[i].Update();
                if (SharpCrossStarList[i].Time >= SharpCrossStarList[i].LifeTime)
                {
                    SharpCrossStarList[i].SetUpStar();
                    SharpCrossStarList.RemoveAt(i);
                }
            }

            for (int i = 0; i < SharpTearsList.Count; i++)
            {
                SharpTearsList[i].Update();
                if (SharpTearsList[i].Time >= SharpTearsList[i].LifeTime)
                    SharpTearsList.RemoveAt(i);
            }
            for (int i = 0; i < CircleList.Count; i++)
            {
                CircleList[i].Update();
                if (CircleList[i].Time >= CircleList[i].MaxTime)
                    CircleList.RemoveAt(i);
            }

            int count = SharpTearsList.Count + SharpCrossStarList.Count + CircleList.Count;
        }
        public override void PrepareRenderTarget()
        {
            if (SharpTearsList.Count != 0)
            {
                for (int i = 0; i < SharpTearsList.Count; i++)
                {
                    Main.spriteBatch.Draw(KivotosTextureAssets.Particle_SharpTearClean.Value, SharpTearsList[i].Center - Main.screenPosition, null, Color.White, SharpTearsList[i].Rot, KivotosTextureAssets.Particle_SharpTearClean.Value.Size() / 2f, SharpTearsList[i].Scale, 0, 0);
                }
            }
            if (SharpCrossStarList.Count != 0)
            {
                for (int i = 0; i < SharpCrossStarList.Count; i++)
                {
                    Main.spriteBatch.Draw(KivotosTextureAssets.Particle_KiraStar.Value, SharpCrossStarList[i].Center - Main.screenPosition, null, Color.White, 0, KivotosTextureAssets.Particle_KiraStar.Value.Size() / 2f, SharpCrossStarList[i].Scale, SpriteEffects.None, 0f);
                }
            }
            if (CircleList.Count != 0)
            {
                for (int i = 0; i < CircleList.Count; i++)
                    Main.spriteBatch.Draw(KivotosTextureAssets.Texture_WhiteCircle.Value, CircleList[i].Center - Main.screenPosition, null, Color.White, 0, KivotosTextureAssets.Texture_WhiteCircle.Value.Size() / 2f, CircleList[i].Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
