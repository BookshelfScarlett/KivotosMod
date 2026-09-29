using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
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
        /// </summary>
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.HasProj(Item.shoot))
            {

            }
            return false;
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
