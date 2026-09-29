using KivotosMod.Content.Projs.StudentWeapons.AbydosHighSchool;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.StudentWeapons.GehennaAcademy
{
    public class HarukaWeapon : StudentWeaponClass
    {
        protected override string Owner => "Haruka";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HinaViolet);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 35;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HoshinoWeaponHeldProj>();
        }
    }
}
