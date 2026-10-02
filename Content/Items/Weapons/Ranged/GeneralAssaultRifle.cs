using KivotosMod.Content.Projs.Ranged;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Weapons.Ranged
{
    public abstract class GeneralAssaultRifle : ModItem, ILocalizedModType
    {
        public override string LocalizationCategory => LocalizationsDatabase.Items.RangedWeapon;
        public override string Texture => KivotosContent.GetAsset(KivotosContent.GeneralWeapon, GetType().Name);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.shoot = ProjectileType<GeneralAssaultRifleBullet>();
            Item.shootSpeed = 16f;
            Item.damage = 54;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Blue;
            Item.SetUpNoUseGraphicItem(true);
            Item.UseSound = SoundID.Item14;
            Item.useAmmo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (source.AmmoItemIdUsed == ItemID.MusketBall)
                type = ProjectileType<GeneralAssaultRifleBullet>();
            Projectile proj2 = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, ProjectileType<GeneralRecoilWeaponProj>(), 0, 0, player.whoAmI);
            if (proj.ModProjectile is GeneralRecoilWeaponProj holdout)
            {
                holdout.SetUpHoldoutData(Type, 7f, Item.useAnimation, new Vector2(10, 0f));
            }
            return false;
        }
    }
    #region 统一的步枪类，方便后续扩展
    /// <summary>
    /// 是的没错。这里直接新建一个空白类就能用了。
    /// </summary>
    public class GeneralAssaultRifle1 : GeneralAssaultRifle
    {
    }
    public class GeneralAssaultRifle2 : GeneralAssaultRifle
    {
    }
    public class GeneralAssaultRifle3 : GeneralAssaultRifle
    {
    }
    public class GeneralAssaultRifle4 : GeneralAssaultRifle
    {
    }
    public class GeneralAssaultRifle5 : GeneralAssaultRifle
    {
    }
    #endregion

}
