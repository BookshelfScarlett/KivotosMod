using KivotosMod.Content.Projs.StudentWeapons.GehennaAcademy;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Methods;
using KivotosMod.Globals.Methods.Textbox;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.StudentWeapons.GehennaAcademy
{
    public class HinaWeapon : StudentWeaponClass
    {
        protected override string Owner => "Hina";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            KivotosLists.ShinyRarityItemDictionary.Add(Type, KivotosRarityType.HinaViolet);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 6;
            Item.damage = 45;
            Item.knockBack = 1;
            Item.shootSpeed = 10f;
            Item.shoot = ProjectileType<HinaWeaponHeldProj>();
        }
        public IReadOnlyList<TooltipLine> CacheTooltipList = null;
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
                backgroundColor: Color.Black * .44f,
                backgroundEdgeColor: Color.Lerp(Color.DarkViolet,Color.Black,.5f),
                textColor: Color.White,
                textEdgeColor: Color.Lerp(Color.DarkViolet,Color.Black,.5f),
                mainText: text
                );
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            base.PostDrawTooltipLine(line);
        }
    }
}
