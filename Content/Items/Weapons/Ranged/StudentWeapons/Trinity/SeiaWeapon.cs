using KivotosMod.Content.Projs.Ranged.StudentWeapons.Trinity;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Trinity
{
    public class SeiaWeapon : StudentWeaponClass
    {
        protected override void SetUpTextboxSettings(ref Color backgroundColor, ref Color backgroundEdgeColor, ref Color textColor, ref Color textEdgeColor)
        {
            backgroundColor = Color.White * .3f;
            backgroundEdgeColor = Color.LightGoldenrodYellow;
            textColor = Color.White;
            textEdgeColor = Color.DarkGoldenrod;
        }

        protected override string Owner => "Seia";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.IbukiYellow);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 25;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<SeiaWeaponHeldProj>();
        }
    }
}
