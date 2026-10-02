using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Gehenna;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Graphics.Metaballs;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class HarukaWeaponHeldProj : KivotosRangedWeaponProjectile
    {
        public override int OriginalItemID => ItemType<HarukaWeapon>();
        public override string Texture => GetInstance<HarukaWeapon>().Texture;
        public override float HoldoutDrawScale => 1;
        public override Color HoldoutEdgeColor => Color.DarkViolet;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(15f, -5f);
        public override float RecoilPower => 15;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -3.5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 15;
            float randRot = ToRadians(20.5f);
            for (int i = 0; i < 15; i++)
            {
                Vector2 randomVelocity = dir.RotatedByRandom(randRot) * Main.rand.NextFloat(0.88f, 1.12f);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 16f, ProjectileType<HarukaWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            }
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 30, 30, RandRotTwoPi);
            for (int i = 0; i < 28; i++)
            {
                Vector2 pos2 = pos.ToRandCirclePos(8);
                Vector2 vel = Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(40), .1f, 14.6f);
                float scale = Projectile.scale * Main.rand.NextFloat(.95f, 1.15f) * 0.48f;
                int timeLeft = Main.rand.Next(30, 45);
                ECSParticle.ShinyCrossStarECS(pos2, vel, Color.Lerp(Color.DarkViolet, Color.Violet, Main.rand.NextFloat()), timeLeft, 1, scale, .2f, BlendState.NonPremultiplied);
            }
            for (int i = 0; i < 30; i++)
            {
                bool alt = Main.rand.NextBool();
                BlendState bs = alt ? BlendState.NonPremultiplied : BlendState.NonPremultiplied;
                ShadowNebulaAlt.SpawnSharpTearClean(pos, dir.ToRandVelocity(ToRadians(20), 0.1f, 33.4f), 0.74f, 40);
                //ECSParticle.SmokeParticle(pos, dir.ToRandVelocity(ToRadians(40), 0.1f, 13.4f), 
                //    Color.Lerp(Color.DarkViolet, Color.Violet, Main.rand.NextFloat()), Main.rand.Next(45, 65), Main.rand.NextFloat(TwoPi), 1, 0.2f * Main.rand.NextFloat(.95f, 1.25f), alt, bs);
            }

        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_HarukaShotgun with { Pitch = -.36f }, Projectile.Center);
            base.PreAttack();
        }
    }
}
