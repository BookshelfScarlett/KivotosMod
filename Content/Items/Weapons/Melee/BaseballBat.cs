using KivotosMod.Content.Projs.Melee;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Weapons.Melee
{
    public class BaseballBat : ModItem, ILocalizedModType
    {
        public override string LocalizationCategory => LocalizationsDatabase.Items.MeleeWeapon;
        protected virtual string AssetName => "BaseballBat";
        protected virtual int HeldProj => ProjectileType<BaseballBatHeldProj>();
        public override string Texture => KivotosContent.GetAsset(KivotosContent.GeneralWeapon, AssetName);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 54;
            Item.SetUpNoUseGraphicItem(true);
            Item.useTime = Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.shootSpeed = 12f;
            Item.shoot = HeldProj;
        }
        public override bool CanUseItem(Player player)
        {
            return !player.HasProj(Item.shoot);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((BaseballBatHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
    }
    public class MetalBaseballBat : BaseballBat
    {
        protected override string AssetName => GetType().Name;
        protected override int HeldProj => ProjectileType<MetalBaseballBatHeldProj>();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((MetalBaseballBatHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
    }
    public class PracticeCricketBat: BaseballBat
    {
        protected override string AssetName => GetType().Name;
        protected override int HeldProj => ProjectileType<PracticeCricketBatHeldProj>();
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            ((PracticeCricketBatHeldProj)proj.ModProjectile).Flip = Main.rand.NextBool();
            return false;
        }
    }

}
