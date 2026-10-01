using KivotosMod.Assets.Register;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        /// <summary>
        /// 获取远程武器手持射弹的绘制数据
        /// <br>基本上包括了手持方向，位置，原点</br>
        /// <br>原点默认为手持射弹贴图的中心</br>
        /// </summary>
        public static void GetRangedWeaponHeldProjData(this Projectile proj, out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float drawRot, out SpriteEffects se)
        {
            tex = proj.GetTexture();
            drawPos = proj.Center - Main.screenPosition;
            drawRot = proj.rotation + proj.spriteDirection == -1 ? Pi : 0;
            rotPoint = tex.Size() / 2;
            se = proj.spriteDirection * Main.player[proj.owner].gravDir == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
        /// <summary>
        /// 快速设置手持射弹的基础属性
        /// </summary>
        public static void SetUpHeldProj(this Projectile proj, int eu = 0)
        {
            proj.ignoreWater = true;
            proj.tileCollide = false;
            proj.usesLocalNPCImmunity = true;
            proj.penetrate = -1;
            proj.extraUpdates = eu;
            proj.noEnchantmentVisuals = true;
            proj.timeLeft = 10000;
        }
        /// <summary>
        /// 判断该射弹是否属于本地玩家
        /// </summary>
        public static bool IsMe(this Projectile proj) => proj.owner == Main.myPlayer;
        /// <summary>
        /// 获取射弹的贴图
        /// </summary>
        public static Texture2D GetTexture(this Projectile proj) => TextureAssets.Projectile[proj.type].Value;
        public static bool IsOutScreen(this Projectile proj, float mult = 1f) => OutOffScreen(proj.Center, mult);

        /// <summary>
        /// 轨迹设置，一般情况下默认使用模式2
        /// </summary>
        /// <param name="proj"></param>
        /// <param name="length"></param>
        /// <param name="mode"></param>
        public static void ToTrailSetting(this Projectile proj, int length = 4, int mode = 2)
        {
            ProjectileID.Sets.TrailingMode[proj.type] = mode;
            ProjectileID.Sets.TrailCacheLength[proj.type] = length;
        }
        public static Vector2 SafeDir(this Projectile proj) => proj.velocity.ToSafeNormalize();

        public static void BounceOnTile(this Projectile proj, Vector2 oldVelocity, float xMult = 1f, float yMult = 1f)
        {
            if (proj.velocity.X != oldVelocity.X)
                proj.velocity.X = -oldVelocity.X * xMult;
            if (proj.velocity.Y != oldVelocity.Y)
                proj.velocity.Y = -oldVelocity.Y * yMult;
        }
        public static void SetCrossStar(this Projectile proj, float scale, float rot, Color mainColor, float xScale = .45f, float yScale = 1f)
        {
            Texture2D star = KivotosTextureAssets.Particle_SharpTear;
            Vector2 pos = proj.Center - Main.screenPosition;
            Vector2 starScale = new Vector2(xScale, yScale);
            Main.spriteBatch.Draw(star, pos, null, mainColor, rot, star.Size() / 2, starScale * proj.scale * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White, rot, star.Size() / 2, starScale * proj.scale * scale * .5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, mainColor, rot + PiOver2, star.Size() / 2, starScale * proj.scale * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White, rot + PiOver2, star.Size() / 2, starScale * proj.scale * scale * .5f, SpriteEffects.None, 0);
        }
        public static void SetCrossStar(this Projectile proj, Vector2 pos, float scale, float rot, Color mainColor, float xScale = .45f, float yScale = 1f)
        {
            Texture2D star = KivotosTextureAssets.Particle_SharpTear;
            pos = pos - Main.screenPosition;
            Vector2 starScale = new Vector2(xScale, yScale);
            Main.spriteBatch.Draw(star, pos, null, mainColor, rot, star.Size() / 2, starScale * proj.scale * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White, rot, star.Size() / 2, starScale * proj.scale * scale * .5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, mainColor, rot + PiOver2, star.Size() / 2, starScale * proj.scale * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(star, pos, null, Color.White, rot + PiOver2, star.Size() / 2, starScale * proj.scale * scale * .5f, SpriteEffects.None, 0);
        }
    }
}
