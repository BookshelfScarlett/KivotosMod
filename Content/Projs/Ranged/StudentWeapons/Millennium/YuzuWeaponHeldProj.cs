using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Millennium;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Millennium
{
    public class YuzuWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<YuzuWeapon>();
        public override string Texture => GetInstance<YuzuWeapon>().Texture;
        public override float HoldoutDrawScale => .75f;
        public override Color HoldoutEdgeColor => Color.OrangeRed;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(0f, -5f);
        public override float RecoilPower => 20;
        public override float RecoilWeaponPullbackRatios => .3f;
        protected override void UpdateRecoil()
        {
            base.UpdateRecoil();
            float progress = Utils.GetLerpValue(AttackSpeed, 0, RecoilTimer, true);
            float rot = (Projectile.Center - Main.MouseWorld).ToRotation() * Owner.gravDir;
            float pro;
            float shakeRatios = .56f;
            if (progress > shakeRatios)
            {
                pro = (1 - progress) / (1 - shakeRatios);
            }
            else
            {
                pro = progress / shakeRatios;
            }

            Projectile.Center += Main.rand.NextVector2Circular(2.5f, 2.5f)*pro;
        }
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(10, -5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 bulletPosOffset = ((HoldoutOffset + new Vector2(1, -5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float randRot = ToRadians(4.5f);
            Vector2 bulletPos = Projectile.Center + bulletPosOffset;
            Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, randomVelocity * 16f, ProjectileType<YuzuWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = Projectile.Center + particleOffset + dir * 35;
            for (int i = 0; i < 38; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, Color.Lerp(Color.DarkOrange, Color.Orange, Main.rand.NextFloat()), timeLeft, 1, scale, .2f);
                    ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(3f), vel.ToRandVelocity(ToRadians(10),1f, 24f), RandLerpColor(Color.Orange, Color.DarkOrange), 30, 1f, Main.rand.NextFloat(.7f, 1.3f) * .1f, 0.45f);
            }
            for (int i = 0; i < 40; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 13.4f), Color.Lerp(Color.DarkOrange, Color.OrangeRed, Main.rand.NextFloat()), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.13f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }


        }
        protected override void PreAttack()
        {
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 20f, 20, RandRotTwoPi);
            SoundEngine.PlaySound(KivotosSoundsAssets.YuzuShot, Projectile.Center);
            SoundEngine.PlaySound(KivotosSoundsAssets.SharpBoom, Projectile.Center);
            base.PreAttack();
        }
    }
}
