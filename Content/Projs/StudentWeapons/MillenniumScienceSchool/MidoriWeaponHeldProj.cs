using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.StudentWeapons.MillenniumScienceSchool;
using KivotosMod.Globals.Classes;
using Terraria;
using Terraria.Audio;

namespace KivotosMod.Content.Projs.StudentWeapons.MillenniumScienceSchool
{
    public class MidoriWeaponHeldProj : KivotosRangedWeaponProjectie
    {
        public override int OriginalItemID => ItemType<MidoriWeapon>();
        public override string Texture => GetInstance<MidoriWeapon>().Texture;
        public override float HoldoutDrawScale => base.HoldoutDrawScale;
        public override Color HoldoutEdgeColor => Color.Green;
        public override bool HoldoutEdgeEnable => base.HoldoutEdgeEnable;
        public override Vector2 HoldoutOffset => new Vector2(20,-5);
        public override float RecoilPower => base.RecoilPower;
        public override float RecoilWeaponPullbackRatios => base.RecoilWeaponPullbackRatios;
        protected override void UpdateWeaponUsing()
        {
            Projectile.Center += Main.rand.NextVector2Circular(1.2f, 1.2f);
            base.UpdateWeaponUsing();
        }
        protected override void OnAttack()
        {
            base.OnAttack();
        }
        protected override void PreAttack()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.Shotgun_Mastiff, Projectile.Center);
            base.PreAttack();
        }
    }
}
