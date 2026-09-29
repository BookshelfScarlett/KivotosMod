using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.StudentWeapons.MillenniumScienceSchool
{
    /// <summary>
    /// 这玩意需要做一个双持效果，但是目前还没有实现，暂时先不做了
    /// </summary>
    public class YuukaWeapon: StudentWeaponClass
    {
        protected override string Owner => "Yuuka";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.YuukaBlue);
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
