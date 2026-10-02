using KivotosMod.Content.Projs.Ranged.StudentWeapons.Highlander;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;

namespace KivotosMod.Content.Items.Weapons.Ranged.StudentWeapons.Highlander
{
    public class HikariWeapon : StudentWeaponClass
    {
        protected override void SetUpTextboxSettings(ref Color backgroundColor, ref Color backgroundEdgeColor, ref Color textColor, ref Color textEdgeColor)
        {
            backgroundColor = Color.Lerp(Color.LightGreen, Color.White, .5f) * .2f;
            backgroundEdgeColor = Color.Lerp(Color.White, Color.DodgerBlue, .14f);
            textColor = Color.Lerp(Color.LightGreen, Color.White, .5f);
            textEdgeColor = Color.Lerp(Color.DodgerBlue, Color.MidnightBlue, .85f);
        }

        protected override string Owner => "Hikari";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.ShupogakiLightGreen);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 25;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HikariWeaponHeldProj>();
        }
    }
}
