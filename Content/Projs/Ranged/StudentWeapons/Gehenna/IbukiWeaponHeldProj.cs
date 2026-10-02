using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Gehenna;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Graphics.Particles;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class IbukiWeaponHeldProj : KivotosRangedWeaponProjectile
    {
        public override int OriginalItemID => ItemType<IbukiWeapon>();
        public override string Texture => GetInstance<IbukiWeapon>().Texture;
        public override float HoldoutDrawScale => .60f;
        public override Color HoldoutEdgeColor => Color.Yellow;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(25f, -0f);
        public override float RecoilPower => 5;
        public override float RecoilWeaponPullbackRatios => .3f;


        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(10, -5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 bulletPosOffset = ((HoldoutOffset + new Vector2(1, -5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            float randRot = ToRadians(4.5f);
            Vector2 bulletPos = Projectile.Center + bulletPosOffset;
            Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), bulletPos, randomVelocity * 16f, ProjectileType<IbukiWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 pos = Projectile.Center + particleOffset + dir * 15;
            for (int i = 0; i < 5; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), 4f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                new IbukiCuteSymbol(pos2, vel, Color.White, timeLeft, Main.rand.NextFloat(-.1f, .1f), 1, Main.rand.NextFloat(.85f, 1.15f) * .15f, false).Spawn();
            }
            for (int i = 0; i < 3; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.Additive : BlendState.AlphaBlend;
                ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(10), 0.1f, 13.4f), Color.Lerp(Color.DarkGoldenrod, Color.LightGoldenrodYellow, Main.rand.NextFloat()), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.13f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }

        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff, Projectile.Center);
            base.PreAttack();
        }
    }
}
