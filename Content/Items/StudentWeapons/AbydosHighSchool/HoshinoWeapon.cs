using KivotosMod.Content.Projs.StudentWeapons.AbydosHighSchool;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Methods;
using KivotosMod.Globals.Methods.Textbox;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.StudentWeapons.AbydosHighSchool
{
    public class HoshinoWeapon : StudentWeaponClass
    {
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
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
        public override void HoldItem(Player player)
        {
            player.statDefense += player.DefenseMultiplier(DefenseMult, true);
            base.HoldItem(player);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            CacheTooltipList = tooltips;
        }
        public override void PostDrawTooltipLine(DrawableTooltipLine line)
        {
            if (line.IsItemName())
            {
                TextboxManager.FirstLineY = line.Y;
            }
            string text = this.GetLocalizationKey("FlavorTooltip").ToLangValue();
            TextboxSettings sets = new TextboxSettings
                (
                hasTitle: false,
                backgroundColor: Color.Lerp(Color.Black,Color.Pink,.4f) * .44f,
                backgroundEdgeColor: Color.Lerp(Color.Pink, Color.White, .5f),
                textColor: Color.White,
                textEdgeColor: Color.Lerp(Color.HotPink, Color.Black, .35f),
                mainText: text
                );
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            base.PostDrawTooltipLine(line);
        }
    }
}
