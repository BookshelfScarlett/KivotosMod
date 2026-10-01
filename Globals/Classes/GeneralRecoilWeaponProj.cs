using KivotosMod.Assets.Register;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Classes
{
    /// <summary>
    /// 通用的远程类武器后坐力射弹
    /// <br>在shoot方法里面直接使用传值就可以让武器有“后坐力动画”和类似手持射弹一样旋转的动画</br>
    /// <br>使用起来足够方便，很适合堆量</br>
    /// </summary>
    public class GeneralRecoilWeaponProj : KivotosPlayerProjs
    {
        public override string LocalizationCategory => LocalizationsDatabase.ProjLocalization;
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        /// <summary>
        /// 设置后坐力武器的相关数据
        /// </summary>
        /// <param name="gunType">设置的“枪”类型，需要传入的是物品ID</param>
        /// <param name="recoilPower">后坐力强度。如果输入0，则不产生后坐力动画，使其能够支持法杖</param>
        /// <param name="lifeTime">生命周期</param>
        /// <param name="holdoutoffset">持握偏移量</param>
        public void SetUpHoldoutData(int gunType, float recoilPower, int lifeTime, Vector2 holdoutoffset)
        {
            ItemLifeTime = lifeTime;
            GunItemType = gunType;
            RecoilPower = recoilPower;
            HoldProjOffset = holdoutoffset;
        }
        /// <summary>
        /// 这把后坐力武器的”枪“的ID
        /// <br>用于给予后坐力的贴图</br>
        /// </summary>
        public int GunItemType = ItemID.BeeGun;
        public ref float RecoilPower => ref Projectile.ai[0];
        public int ItemLifeTime
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public Vector2 HoldProjOffset = Vector2.Zero;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.SetUpHeldProj();
        }
        public override void ProjAI()
        {
            int duration = Owner.itemAnimationMax; // Define the duration the projectile will exist in frames

            // Reset projectile time left if necessary
            if (Projectile.timeLeft > duration)
            {
                Projectile.timeLeft = duration;
            }
            Projectile.velocity = Owner.ToMouseVector2();
            Projectile.rotation = Projectile.velocity.ToRotation();
            int dir = Math.Sign(Main.MouseWorld.X - Owner.Center.X);
            Projectile.spriteDirection = dir;
            Owner.heldProj = Projectile.whoAmI; // Update the player's held projectile id
            Owner.ChangeDir(dir);
            Owner.ControlPlayerArm(Projectile.rotation);
            if (RecoilPower != 0)
            {
                float pullBack = RecoilPower;
                float animationProgress = 0.5f - Owner.itemTime / (float)Owner.itemTimeMax;
                if (animationProgress < .4f)
                    pullBack -= 2.75f * (float)Math.Pow((.6f - animationProgress) / .6f, 2f);
                Projectile.Center = Owner.MountedCenter + Projectile.rotation.ToRotationVector2() * pullBack;
            }
            else
                Projectile.Center = Owner.MountedCenter;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Item[GunItemType].Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 offset = HoldProjOffset * new Vector2(Owner.direction, 1);
            float drawRot = Projectile.rotation;
            if (Projectile.spriteDirection < 0)
            {
                drawRot += Pi;
            }
            drawPos += offset.RotatedBy(drawRot);
            Vector2 rotPoint = tex.Size() / 2f;
            SpriteEffects se = (Projectile.spriteDirection * Owner.gravDir) == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, Projectile.scale, se, 0);
            return false;
        }
    }
}
