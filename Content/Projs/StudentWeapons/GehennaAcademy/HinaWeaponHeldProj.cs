using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.StudentWeapons.GehennaAcademy;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.StudentWeapons.GehennaAcademy
{
    public class HinaWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<HinaWeapon>();
        public override string Texture => GetInstance<HinaWeapon>().Texture;
        public override float HoldoutDrawScale => .98f;
        public override Color HoldoutEdgeColor => Color.DarkViolet;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(15f, 5f);
        public override float RecoilPower => 5;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void UpdateHeldProjectile()
        {
            if (Projectile.IsMe())
            {
                Projectile.rotation = Owner.ToMouseVector2().ToRotation();
                Projectile.spriteDirection = Projectile.direction = (Main.MouseWorld.X > Owner.Center.X).ToDirectionInt();
                Owner.ChangeDir(Projectile.direction);
                Owner.heldProj = Projectile.whoAmI;
                float offset = Owner.direction > 0 ? PiOver4 : -PiOver4;
                Owner.ControlPlayerArm(Projectile.rotation + offset);
                Projectile.Center = Owner.MountedCenter;
                Projectile.position.Y += Owner.gfxOffY;
            }
        }
        protected override void UpdateWeaponUsing()
        {
            Projectile.Center += Main.rand.NextVector2Circular(2f, 2);
        }
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -0.5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 15;
            float randRot = ToRadians(12.5f);
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 4, 4, Main.rand.NextFloat(TwoPi));
            for (int i = 0; i < 3; i++)
            {
                Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 16f, ProjectileType<HinaWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            }
            for (int i = 0; i < 14; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, Color.Lerp(Color.Violet, Color.DarkViolet, Main.rand.NextFloat()), timeLeft, 1, scale, .2f);
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.GlowSquare(pos2, vel, Color.Violet.RandLerpColor(Color.DarkViolet), timeLeft, 1, Main.rand.NextFloat(TwoPi), scale);
            }
            for (int i = 0; i < 14; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.NonPremultiplied : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 13.4f), Color.Lerp(Color.Violet, Color.DarkViolet, Main.rand.NextFloat()), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.13f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }
            Vector2 dir2 = Projectile.rotation.ToRotationVector2() * -1;
            for (int i = 0; i < 8; i++)
            {
                Vector2 firePos = Projectile.Center + particleOffset + dir2 * 1f;
                Vector2 vel = dir2.ToRandVelocity(ToRadians(10f), 1.8f, 16.8f);
                Vector2 offset = dir2.ToRandVelocity(ToRadians(0), 6f, 9f);
                Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir2 * 0f;
                ECSParticle.ShinyCrossStarECS(firePos.ToRandCirclePos(20f) + posOffset, vel, Color.Violet.RandLerpColor(Color.DarkViolet), 40, 1f, Main.rand.NextFloat(0.5f, 0.8f) * .7f, .2f);
            }
            for (int i = 0; i < 12; i++)
            {
                Vector2 firePos = Projectile.Center + particleOffset + dir2 * 1f;
                Vector2 vel = dir2.ToRandVelocity(ToRadians(10f), 1.8f, 10.8f);
                Vector2 offset = dir2.ToRandVelocity(ToRadians(0), 6f, 9f);
                Vector2 posOffset = offset + Main.rand.NextVector2Circular(10f, 5f) + dir2 * 0f;
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.NonPremultiplied : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(firePos, vel, Color.Lerp(Color.Violet, Color.Black, Main.rand.NextFloat()), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.13f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff, Projectile.Center);
            base.PreAttack();
        }
    }
}
