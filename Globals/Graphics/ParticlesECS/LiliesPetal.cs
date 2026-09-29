using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Methods;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class LiliesPetal : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            data.aiint0 = Main.rand.Next(0, 10000);
            data.aiint1 = Main.rand.NextBool().ToDirectionInt();
        }
        public override void Update(ref ECSParticleData data)
        {
            float speed = data.aifloat0;
            float beginScale = data.aifloat1;
            ref bool hasLanded = ref data.aibool0;
            bool noCollision = data.aibool1;
            int seed = data.aiint0;
            int dir = data.aiint1;
            if (!hasLanded)
            {
                Point tileCoords = data.Position.ToTileCoordinates();
                Tile tile = Framing.GetTileSafely(tileCoords.X, tileCoords.Y);
                if (tile.HasTile && !tile.IsActuated && !noCollision)
                {
                    hasLanded = true;
                    data.Velocity = Vector2.Zero;
                }
                else
                {
                    if (speed != 0)
                    {
                        float randRot = .94f;
                        Vector2 idealVel = Vector2.UnitY.RotatedBy(Lerp(-randRot, randRot, (float)Math.Sin(data.Time / 36f + seed) * .5f + .5f)) * speed;
                        float moveInter = Lerp(.05f, .1f, Utils.GetLerpValue(0, data.Lifetime / 2, data.Time, true));
                        data.Velocity = Vector2.Lerp(data.Velocity, idealVel, moveInter);
                        data.Velocity = data.Velocity.SafeNormalize(-Vector2.UnitY) * speed;
                    }
                }
            }
            if (!hasLanded)
            {
                data.Rotation += .1f * dir;
            }
            data.Scale = Lerp(beginScale, 0, EasingFunction.EaseInCubic(data.LifetimeRatio));
        }
        public override void OnKill(ref ECSParticleData data)
        {
            base.OnKill(ref data);
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool fullBright = data.aibool2;
            int glowMult = data.aiint2;
            Texture2D tex = data.aifloat2 > 0 ? KivotosTextureAssets.Particle_Leafs.Value : KivotosTextureAssets.Particle_Petal.Value;
            SpriteBatch sb = Main.spriteBatch;
            Color c = data.DrawColor * data.Opacity * Lighting.Brightness((int)(data.Position.X / 16f), (int)(data.Position.Y / 16f));
            if (fullBright)
                c = data.DrawColor * data.Opacity;
            Vector2 pos = data.Position - Main.screenPosition;
            if (glowMult > 0)
            {
                int total = glowMult * 8;
                for (int i = 0; i < total; i++)
                {
                    sb.Draw(tex, pos + (TwoPi / (float)total * i).ToRotationVector2() * 1.5f, null, Color.Lerp(data.DrawColor, Color.White, 0.5f).ToAddColor(), data.Rotation, tex.Size() / 2, data.Scale, 0, 0);
                }

            }
            sb.Draw(tex, pos, null, c, data.Rotation, tex.Size() / 2, data.Scale, 0, 0);
        }
    }
}
