using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics.Particles;
using KivotosMod.Globals.Methods;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class IbukiWeaponBullet : KivotosPlayerProjs
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool())
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(12), Projectile.velocity / 3f, RandLerpColor(Color.Goldenrod, Color.LightGoldenrodYellow), 40, 1,
                    Main.rand.NextFloat(.9f, 1.1f) * .4f, 0.2f);
            }
            if (Main.rand.NextBool(4))
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(12), Projectile.velocity / 3f, RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 0.745f, 3,Main.rand.NextFloat(-.1f,.1f),0.4f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(12), RandVelTwoPi(2,6f), RandLerpColor(Color.Goldenrod, Color.LightGoldenrodYellow), 40, 1,
                    Main.rand.NextFloat(.9f, 1.1f) * .7f, 0.2f);
            }
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(12), RandVelTwoPi(2,6), RandLerpColor(Color.DarkGoldenrod, Color.LightGoldenrodYellow), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * 0.745f, 3, Main.rand.NextFloat(-.1f, .1f), 0.4f);
            }

                ECSParticle.ShinyCrossStarSmall(Projectile.Center, Vector2.Zero,
                    RandLerpColor(Color.DarkGoldenrod, Color.Goldenrod), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * 1.5f, 0);
                new IbukiCuteSymbol(Projectile.Center.ToRandCirclePos(5), (-Vector2.UnitY).ToRandVelocity(ToRadians(10),14,16), Color.White, 50, Main.rand.NextFloat(-.1f, .1f), 1, Main.rand.NextFloat(.85f,1.15f)*.25f, true).Spawn();

            for (int i = 0; i < 2; i++)
            {
                new IbukiCuteSymbol(Projectile.Center.ToRandCirclePos(5), RandVelTwoPi(4f, 12f), Color.White, 50, Main.rand.NextFloat(-.1f, .1f), 1, Main.rand.NextFloat(.85f,1.15f)*.5f, false).Spawn();
            }

            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.IsOutScreen())
                return false;
            Texture2D line = KivotosTextureAssets.Particle_SharpTear;
            int count = Projectile.oldPos.Length;
            float overallScale = 1f;
            float overallAlpha = 1f;

            //Copy-right:VFX
            for (int i = 0; i < count; i++)
            {
                float progress = 1 - (float)i / count;
                float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.45f) * 0.1f;
                Vector2 AfterImagePos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Main.rand.NextVector2Circular(4.5f, 4.5f); //6f
                float startScale = 1.1f + sineScale;
                float rot = Projectile.oldRot[i] + PiOver2;
                Color between = Color.Lerp(Color.DarkGoldenrod, Color.Gold, 0.15f);
                Color col = Color.Lerp(between, Color.LightGoldenrodYellow, 1f - progress);
                float easedFadeValue = progress * progress * overallAlpha;
                Vector2 lineScale = new Vector2(0.20f + 0.4f * progress, 1.25f);
                lineScale.Y *= overallScale;
                Vector2 lineScale2 = new Vector2(0.05f + 0.071f * progress, 1.25f);
                lineScale2.Y *= overallScale;
                SB.FastDraw(line, AfterImagePos, Color.Black * .4f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                SB.FastDraw(line, AfterImagePos, col.ToAddColor() * .985f * easedFadeValue, rot, line.Size() / 2f, lineScale * startScale, SpriteEffects.None);
                SB.FastDraw(line, AfterImagePos, Color.White.ToAddColor() * .95f * easedFadeValue, rot, line.Size() / 2f, lineScale2 * startScale, SpriteEffects.None);
            }
            return false;
        }
    }
}
