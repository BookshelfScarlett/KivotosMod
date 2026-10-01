using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using KivotosMod.Globals.Methods.Textbox;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Classes
{
    /// <summary>
    /// 学生的武器类
    /// <br>这个武器类实际上什么都不干，确切来说的话。</br>
    /// </summary>
    public abstract class StudentWeaponClass : ModItem, ILocalizedModType
    {
        public override string LocalizationCategory => LocalizationsDatabase.Items.StudentWeapons;
        public override string Texture => KivotosContent.GetAsset(KivotosContent.StudentWeapons, GetType().Name);
        /// <summary>
        /// 武器的持有者，即学生
        /// <br>需要与本地化文件的名字对应，且只考虑名字（不考虑姓氏）</br>
        /// </summary>
        protected virtual string Owner => "None";
        /// <summary>
        /// 设置文本框的样式
        /// <br>这个才是你应该复写的东西</br>
        /// </summary>
        protected virtual void SetUpTextboxSettings(ref Color backgroundColor, ref Color backgroundEdgeColor, ref Color textColor, ref Color textEdgeColor)
        {
            backgroundColor = Color.Black * .44f;
            backgroundEdgeColor = Color.Lerp(Color.DarkViolet, Color.Black, .5f);
            textColor = Color.White;
            textEdgeColor = Color.Lerp(Color.DarkViolet, Color.Black, .5f);
        }
        public override void SetStaticDefaults()
        {
            if (Owner != "None")
            {
                KivotosLists.StudentWeaponDictionary.Add(Type, Owner);
            }
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAmmo = AmmoID.Bullet;
            Item.useTime = Item.useAnimation = 20;
            Item.rare = ItemRarityID.Orange;
            Item.SetUpNoUseGraphicItem(true);
            ExSD();
        }

        /// <summary>
        /// Shoot管理方案用于控制一些与武器有关的逻辑
        /// <br>实际上我们不会真的让他发射任何子弹</br>
        /// <br></br>
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasProj(Item.shoot))
            {

            }
            return false;
        }
        public Color BackgroundColor;
        public Color BackgroundEdgeColor;
        public Color TextColor;
        public Color TextEdgeColor;
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
            //string text = this.GetLocalizationKey("FlavorTooltip").ToLangValue();
            string text = Language.GetOrRegister(this.GetLocalizationKey("FlavorTooltip"), () => "ThisIsFlavorTooltip").Value;
            SetUpTextboxSettings(ref BackgroundColor, ref BackgroundEdgeColor, ref TextColor, ref TextEdgeColor);
            TextboxSettings sets = new TextboxSettings
                (
                hasTitle: false,
                backgroundColor: BackgroundColor,
                backgroundEdgeColor: BackgroundEdgeColor,
                textColor: TextColor,
                textEdgeColor: TextEdgeColor,
                mainText: text
                );
            TextboxMethods.DrawTextboxTooltipWithBackground(line, CacheTooltipList, ref sets);
            base.PostDrawTooltipLine(line);

            base.PostDrawTooltipLine(line);
        }
        public override void HoldItem(Player player)
        {
            if (player.HasProj(Item.shoot))
                return;
            int projDamage = (int)player.GetTotalDamage<RangedDamageClass>().ApplyTo(Item.damage);
            Projectile proj = Projectile.NewProjectileDirect(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, Item.shoot, 0, Item.knockBack, player.whoAmI);
            proj.originalDamage = projDamage;
            proj.netUpdate = true;

        }
        protected virtual void ExSD() { }
    }
}
