using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Trinity;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Trinity
{
    public class SeiaWeaponHeldProj : KivotosRangedWeaponProjectile
    {
        public override int OriginalItemID => ItemType<SeiaWeapon>();
        public override string Texture => GetInstance<SeiaWeapon>().Texture;
        public override float HoldoutDrawScale => .78f;
        public override Color HoldoutEdgeColor => Color.DarkGoldenrod;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(20f, -0f);
        public override float RecoilPower => 10;
        public override float RecoilWeaponPullbackRatios => .3f;


        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(10, -10f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 bulletPosOffset = ((HoldoutOffset + new Vector2(1, -10f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float randRot = ToRadians(2.5f);
            Vector2 bulletPos = Projectile.Center + bulletPosOffset;
            Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, randomVelocity * 16f, ProjectileType<SeiaWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = Projectile.Center + particleOffset + dir * 5;
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.68f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, RandLerpColor(Color.LightGoldenrodYellow,Color.DarkGoldenrod), timeLeft, 1, scale, .2f);
            }
            for (int i = 0; i < 18; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 13.4f), RandLerpColor(Color.LightGoldenrodYellow,Color.Goldenrod), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.13f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }

        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff, Projectile.Center);
            base.PreAttack();
        }
    }
}
