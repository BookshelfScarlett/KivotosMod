using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Millennium;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Millennium
{
    public class NoaWeaponHeldProj : KivotosRangedWeaponProjectile
    {
        public override int OriginalItemID => ItemType<NoaWeapon>();
        public override string Texture => GetInstance<NoaWeapon>().Texture;
        public override float HoldoutDrawScale => 0.85f;
        public override Color HoldoutEdgeColor => Color.LightSkyBlue;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(15f, -1f);
        public override float RecoilPower => HandleRecoilPower();
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        public override int AttackSpeed => (int)(base.AttackSpeed * HandleAttackSpeed());
        public bool IsRightClick = false;
        public float HandleRecoilPower()
        {
            if (IsRightClick)
                return 15;
            return 5;
        }
        public float HandleAttackSpeed()
        {
            if (Owner.channel)
            {
                return 1;
            }
            else
            {
                return 3f;
            }
        }
        protected override void UpdateGlobalReset()
        {
            base.UpdateGlobalReset();
            if (RecoilTimer == 0)
                IsRightClick = false;
        }
        protected override void PreRightAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Pistol with { Pitch = 0f, Volume = .16f, PitchVariance = .1f }, Projectile.Center);
            if (!IsRightClick)
                IsRightClick = true;
            base.PreRightAttack();
        }
        protected override void OnRightAttack()
        {
            Vector2 bulletPos = Projectile.Center + BulletOffset;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, Projectile.rotation.ToRotationVector2() * 16f, ProjectileType<NoaWeaponBulletAlt>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = bulletPos + ParticleOffset;
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                ECSParticle.DigitalNumber(pos2, vel, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1, 0, 0.85f, 0.36f, Main.rand.Next(1, 3), BlendState.Additive);
            }
            for (int i = 0; i < 16; i++)
            {
                bool alt = Main.rand.NextBool();
                ECSParticle.GlowSquare(pos, Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(10), .1f, 16f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.8f, 1.1f) * 1, rotSpeed: Main.rand.NextFloat(-.05f, .05f));
            }
        }
        protected override void PostRightAttack()
        {
            Timer = 0;
            RecoilTimer = AttackSpeed;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff with { Pitch = 0f, Volume = 1f, PitchVariance = .1f }, Projectile.Center);
            base.PreAttack();
        }
        public Vector2 BulletOffset => (HoldoutOffset + new Vector2(0, -7.5f) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
        public Vector2 ParticleOffset => BulletOffset + Projectile.rotation.ToRotationVector2() * 1;

        protected override void OnAttack()
        {
            Vector2 bulletPos = Projectile.Center + BulletOffset;
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, Projectile.rotation.ToRotationVector2() * 16f, ProjectileType<NoaWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = bulletPos + ParticleOffset;
            for (int i = 0; i < 2; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                ECSParticle.DigitalNumber(pos2, vel, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1, 0, 0.85f, 0.36f, Main.rand.Next(1, 3), BlendState.Additive);
            }
            for (int i = 0; i < 2; i++)
            {
                bool alt = Main.rand.NextBool();
                ECSParticle.GlowSquare(pos, Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(5), .1f, 16f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.8f, 1.1f) * 1, rotSpeed: Main.rand.NextFloat(-.05f, .05f));
            }
        }
    }
}
