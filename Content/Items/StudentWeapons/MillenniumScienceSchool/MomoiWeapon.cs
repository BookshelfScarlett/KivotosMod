using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.StudentWeapons.MillenniumScienceSchool
{
    public class MomoiWeapon : StudentWeaponClass
    {
        protected override string Owner => "Momoi";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HoshinoPink);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 35;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            //Item.shoot = ProjectileType<EyeofHorusHeldProj>();
        }
    }

}
