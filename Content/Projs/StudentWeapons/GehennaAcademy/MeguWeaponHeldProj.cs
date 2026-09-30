using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.StudentWeapons.GehennaAcademy;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.StudentWeapons.GehennaAcademy
{
    public class MeguWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<MeguWeapon>();
        public override string Texture => GetInstance<MeguWeapon>().Texture;
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
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), pos, randomVelocity * 16f, ProjectileType<MeguWeaponFlame>(), Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
            }

        }
        protected override void PreAttack()
        {
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 30;
                SoundEngine.PlaySound(KivotosSoundsAssets.FlameThrower_Release, Projectile.Center);
            }
            base.PreAttack();
        }
    }
}
