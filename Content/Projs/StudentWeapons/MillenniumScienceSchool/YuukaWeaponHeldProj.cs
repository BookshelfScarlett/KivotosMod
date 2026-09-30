using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.StudentWeapons.MillenniumScienceSchool;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.StudentWeapons.MillenniumScienceSchool
{
    public class YuukaWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<YuukaWeapon>();
        public override string Texture => GetInstance<YuukaWeapon>().Texture;
        public override float HoldoutDrawScale => 1f;
        public override Color HoldoutEdgeColor => Color.LightSkyBlue;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(15f, -5f);
        public override float RecoilPower => 5;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -3.5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 15;
            float randRot = ToRadians(10.5f);
            Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, Projectile.rotation.ToRotationVector2() * 16f, ProjectileType<YuukaWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.DigitalNumber(pos2, vel, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1, 0, 0.85f, 0.36f, Main.rand.Next(1, 3), BlendState.Additive);
            }
            for (int i = 0; i < 4; i++)
            {
                bool alt = Main.rand.NextBool();
                ECSParticle.GlowSquare(pos, Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(5), .1f, 16f), RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1,
                    RandRotTwoPi, Main.rand.NextFloat(.8f, 1.1f) * 1, rotSpeed: Main.rand.NextFloat(-.05f, .05f));
            }
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Pistol with { Pitch = 0f, Volume = .16f, PitchVariance = .1f }, Projectile.Center);
            base.PreAttack();
        }
    }
}
