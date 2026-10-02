using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Highlander;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Highlander
{
    public class HikariWeaponHeldProj : KivotosRangedWeaponProjectile
    {
        public override int OriginalItemID => ItemType<HikariWeapon>();
        public override string Texture => GetInstance<HikariWeapon>().Texture;
        public override float HoldoutDrawScale => .65f;
        public override Color HoldoutEdgeColor => Color.MintCream;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(21f, -2f);
        public override float RecoilPower => 5;
        public override float RecoilWeaponPullbackRatios => .3f;


        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(30, -10f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 bulletPosOffset = ((HoldoutOffset + new Vector2(1, -10f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float randRot = ToRadians(4.5f);
            Vector2 bulletPos = Projectile.Center + bulletPosOffset;
            Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, randomVelocity * 16f, ProjectileType<HikariWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = Projectile.Center + particleOffset + dir * 10;
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(10), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                Color c = Main.rand.NextBool() ? RandLerpColor(Color.LightGreen, Color.Green) : RandLerpColor(Color.SkyBlue, Color.MidnightBlue);
                ECSParticle.ShinyCrossStarECS(pos2, vel, c, timeLeft, 1, scale, .2f);
            }
            for (int i = 0; i < 20; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(10), .1f, 14.6f);
                Color c = Main.rand.NextBool() ? RandLerpColor(Color.LightGreen, Color.Green) : RandLerpColor(Color.SkyBlue, Color.MidnightBlue);
                ECSParticle.LightntingGlow(pos.ToRandCirclePos(2), vel, c, 40, 1, Main.rand.NextFloat(.85f, 1.16f) * .4f);
            }
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Pistol with { Volume = .3f }, Projectile.Center);
            base.PreAttack();
        }

    }
}
