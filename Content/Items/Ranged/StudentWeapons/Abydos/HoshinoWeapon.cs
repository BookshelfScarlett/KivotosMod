using KivotosMod.Content.Projs.Ranged.StudentWeapons.Abydos;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.Localization;

namespace KivotosMod.Content.Items.Ranged.StudentWeapons.Abydos
{
    public class HoshinoWeapon : StudentWeaponClass
    {
        protected override void SetUpTextboxSettings(ref Color backgroundColor, ref Color backgroundEdgeColor, ref Color textColor, ref Color textEdgeColor)
        {
            backgroundColor = Color.Lerp(Color.Black, Color.Pink, .4f) * .44f;
            backgroundEdgeColor = Color.Lerp(Color.Pink, Color.White, .5f);
            textColor = Color.White;
            textEdgeColor = Color.Lerp(Color.HotPink, Color.Black, .35f);
        }

        protected override string Owner => "Hoshino";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HoshinoPink);
        }
        public float DefenseMult = .25f;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 35;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HoshinoWeaponHeldProj>();
        }
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DefenseMult.ToPercent());
        public override void HoldItem(Player player)
        {
            player.statDefense += player.DefenseMultiplier(DefenseMult, true);
            base.HoldItem(player);
        }
    }
}
