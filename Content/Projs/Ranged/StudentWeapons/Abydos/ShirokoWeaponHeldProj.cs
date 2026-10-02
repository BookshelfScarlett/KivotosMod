using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Abydos;
using KivotosMod.Content.Projs.Ranged.StudentWeapons.Millennium;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Abydos
{
    public class ShirokoWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<ShirokoWeapon>();
        public override string Texture => GetInstance<ShirokoWeapon>().Texture;
        public override float HoldoutDrawScale => .65f;
        public override Color HoldoutEdgeColor => Color.LightSkyBlue;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(30, -5);
        public override float RecoilPower => base.RecoilPower;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void UpdateWeaponUsing()
        {
            Projectile.Center += Main.rand.NextVector2Circular(1.2f, 1.2f);
            base.UpdateWeaponUsing();
        }
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -5.5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 5;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, (Projectile.rotation + Main.rand.NextFloat(ToRadians(-5), ToRadians(5))).ToRotationVector2() * 16f, ProjectileType<ShirokoWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            Vector2 particlePos = pos + dir * 40f;
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos2 = particlePos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(10), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.12f;
                int timeLeft = Main.rand.Next(30, 45);
                Color c = Main.rand.NextBool() ? RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue) : RandLerpColor(Color.White, Color.DeepSkyBlue);
                ECSParticle.HRShinyOrb(pos2, vel, c, 45, 1, scale, .4f);
                //ECSParticle.ShinyCrossStarECS(pos2, vel, c, timeLeft, 1, scale, .2f);
            }
            for (int i = 0; i < 20; i++)
            {
                bool alt = Main.rand.NextBool();
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(10), .1f, 14.6f);
                Color c = Main.rand.NextBool() ? RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue) : RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue);
                ECSParticle.LightntingGlow(particlePos.ToRandCirclePos(2), vel, c, 40, 1, Main.rand.NextFloat(.85f, 1.16f) * .4f);
            }


            base.OnAttack();
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff, Projectile.Center);
            base.PreAttack();
        }
    }
}
