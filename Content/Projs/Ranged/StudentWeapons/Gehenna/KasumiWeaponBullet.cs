using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using System;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class KasumiWeaponBullet : KivotosPlayerProjs
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
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(12), Projectile.velocity / 1f, RandLerpColor(Color.DarkRed, Color.Brown),
                    40, 1, Main.rand.NextFloat(.9f, 1.1f) * .13f, .4f);

            }
            if (Main.rand.NextBool(3))
            {
                ECSParticle.ShinyCrossStarSmall(Projectile.Center.ToRandCirclePos(6), Projectile.velocity.ToRandVelocity(ToRadians(10), 2, 6),
                    RandLerpColor(Color.DarkRed, Color.Brown), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .3f, 0);

            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ECSParticle.ShinyCrossStarSmall(Projectile.Center, Vector2.Zero,
                RandLerpColor(Color.DarkRed, Color.Brown), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * 1.5f, 0);
            for (int i = 0; i < 26; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(10), Main.rand.NextFloat(.9f, 1.1f) * 1.5f,
                    RandLerpColor(Color.DarkRed, Color.Brown), Main.rand.Next(30, 41), 1, Main.rand.NextFloat(.8f, 1.12f) * .13f, glowMult: .6f);

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

            for (int i = 0; i < count; i++)
            {
                float progress = 1 - (float)i / count;
                float sineScale = MathF.Sin((float)Main.timeForVisualEffects * 0.45f) * 0.1f;
                Vector2 AfterImagePos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition + Main.rand.NextVector2Circular(4.5f, 4.5f); //6f
                float startScale = 1.1f + sineScale;
                float rot = Projectile.oldRot[i] + PiOver2;
                Color between = Color.Lerp(Color.DarkRed, Color.Brown, 0.15f);
                Color col = Color.Lerp(between, Color.RosyBrown, 1f - progress);
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
