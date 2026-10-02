using KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Gehenna
{
    public class HarukaWeapon : StudentWeaponClass
    {
        protected override void SetUpTextboxSettings(ref Color backgroundColor, ref Color backgroundEdgeColor, ref Color textColor, ref Color textEdgeColor)
        {
            backgroundColor = Color.Black * .44f;
            backgroundEdgeColor = Color.Lerp(Color.DarkViolet, Color.Black, .5f);
            textColor = Color.White;
            textEdgeColor = Color.Lerp(Color.DarkViolet, Color.Black, .5f);
        }

        protected override string Owner => "Haruka";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HinaViolet);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 60;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HarukaWeaponHeldProj>();
        }
    }
}
