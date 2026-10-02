using KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna;
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
            base.SetUpTextboxSettings(ref backgroundColor, ref backgroundEdgeColor, ref textColor, ref textEdgeColor);
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
