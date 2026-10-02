using KivotosMod.Content.Projs.Melee;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Items.Weapons.Melee
{
    public class PracticeHockeyClub: ModItem, ILocalizedModType
    {
        public override string LocalizationCategory => LocalizationsDatabase.Items.MeleeWeapon;
        protected virtual string AssetName => "BaseballBat";
        protected virtual int HeldProj => ProjectileType<PracticeHockeyClubHeldProj>();
        public override string Texture => KivotosContent.GetAsset(KivotosContent.GeneralWeapon, AssetName);
        public override void SetDefaults()
        {
            Item.width = Item.height = 32;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 54;
            Item.SetUpNoUseGraphicItem(true);
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
            ((PracticeHockeyClubHeldProj)proj.ModProjectile).Flip = false;
            return false;
        }
    }

}
