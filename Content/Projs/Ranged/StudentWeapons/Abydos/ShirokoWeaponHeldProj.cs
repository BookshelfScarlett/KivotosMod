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
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Color HoldoutEdgeColor => Color.Pink;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(20, -5);
        public override float RecoilPower => base.RecoilPower;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void UpdateWeaponUsing()
        {
            Projectile.Center += Main.rand.NextVector2Circular(1.2f, 1.2f);
            base.UpdateWeaponUsing();
        }
        protected override void OnAttack()
        {
            Vector2 particleOffset = ((HoldoutOffset + new Vector2(0, -3.5f)) * new Vector2(1, Owner.direction)).RotatedBy(Projectile.rotation);
            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 pos = Projectile.Center + particleOffset + dir * 15;
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, (Projectile.rotation + Main.rand.NextFloat(ToRadians(-5), ToRadians(5))).ToRotationVector2() * 16f, ProjectileType<ShirokoWeaponBullet>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            for (int i = 0; i < 12; i++)
            {
                ECSParticle.SmokeParticle(pos.ToRandCirclePos(3), Projectile.rotation.ToRotationVector2().ToRandVelocity(ToRadians(15), .1f, 26f), RandLerpColor(Color.Pink, Color.LightPink),
                    Main.rand.Next(30, 51), RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .2f, blendstate: BlendState.Additive);
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
