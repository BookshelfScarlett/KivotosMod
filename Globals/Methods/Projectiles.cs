using KivotosMod.Assets.Register;
using KivotosMod.Content.Projs.Typeless;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

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
            proj.ownerHitCheck = true;
            proj.timeLeft = 10000;
        }
        public static void ControlHoldout(this Projectile proj, Player Owner, bool SetHeldProj = true, bool SetOwnerDir = true)
        {
            proj.Center = Owner.MountedCenter;
            proj.position.Y += Owner.gfxOffY;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            if (SetOwnerDir)
                Owner.ChangeDir(Main.MouseWorld.X > Owner.Center.X ? 1 : -1);
            if (SetHeldProj)
                Owner.heldProj = proj.whoAmI;
            if (Owner.dead)
                proj.Kill();
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

        /// <summary>
        /// 与Resize(int newWidth, int newHeight)不同的是，Resize(int newSize)会将射弹的宽高同时设置为newSize
        /// <br>作为一个同名的重载方案</br>
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="newSize"></param>
        public static void Resize(this Projectile projectile, int newSize)
        {
            projectile.Resize(newSize, newSize);
        }
        /// <summary>
        /// 与Resize(int newWidth, int newHeight)不同的是，Resize(float expandRatio)会将射弹的宽高同时按expandRatio进行缩放
        /// <br>作为一个同名的重载方案</br>
        /// </summary>
        /// <param name="projectile"></param>
        /// <param name="expandRatio"></param>
        public static void Resize(this Projectile projectile, float expandRatio)
        {
            projectile.Resize((int)((float)projectile.width * expandRatio), (int)((float)projectile.height * expandRatio));
        }
        public static bool IsFinalHit(this Projectile proj) => proj.penetrate == 0;
        public static void SetupImmnuity(this Projectile proj, int hitCooldown, bool useIDStatic = false)
        {
            if (useIDStatic)
            {
                proj.usesIDStaticNPCImmunity = true;
                proj.idStaticNPCHitCooldown = hitCooldown;

            }
            else
            {
                proj.usesLocalNPCImmunity = true;
                proj.localNPCHitCooldown = hitCooldown;
            }
        }
                /// <summary>
        /// 用于手持弹幕，获取斜45°近战武器的挥舞
        /// </summary>
        public static void GetDrawDataMelee(this Projectile proj, out Texture2D texture, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite)
        {
            texture = TextureAssets.Projectile[proj.type].Value;
            drawPosition = proj.Center - Main.screenPosition;
            drawRotation = proj.rotation + (proj.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
            rotationPoint = proj.spriteDirection == -1 ? new Vector2(texture.Width, texture.Height) : new Vector2(0, texture.Height);
            flipSprite = proj.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
        }
        #region 隐形爆炸生成
        /// <summary>
        /// 不可见爆炸射弹的生成参数
        /// <br>所有字段均有合理默认值，调用时只需指定需要的部分</br>
        /// </summary>
        public record InvisBoomOptions
        {
            /// <summary>
            /// 伤害类型
            /// </summary>
            public DamageClass DamageClass { get; init; } = DamageClass.Generic;

            ///<summary>
            ///伤害倍率
            ///</summary>
            public float DamageRatio { get; init; } = 1f;

            /// <summary>
            /// 爆炸的时长
            /// </summary>
            public int LifeTime { get; init; } = 40;

            /// <summary>
            /// 局部无敌帧
            /// </summary>
            public int HitCooldown { get; init; } = 40;

            /// <summary>
            /// 碰撞体积的边长
            /// </summary>
            public int Resize { get; init; } = 100;

            /// <summary>
            /// 命中次数（穿透次数），默认-1
            /// </summary>
            public int HitTime { get; init; } = -1;

            /// <summary>
            /// 命中时附加的Buff，如果为-1，表述没有
            /// </summary>
            public int BuffID { get; init; } = -1;

            /// <summary>
            /// Buff持续时间，如果为-1，表述不存在
            /// </summary>
            public int BuffTime { get; init; } = -1;

            /// <summary>
            /// 最大更新
            /// </summary>
            public int MaxUpdates { get; init; } = -1;

            /// <summary>
            /// 要挂载的目标
            /// 默认为null
            /// </summary>
            public NPC TargetMounted { get; init; } = null;
        }
        /// <summary>
        /// 快速生成一个不可见的爆炸射弹
        /// <br>这个传参太多了，我比较建议指定部分参数传入进去</br>
        /// <br>或者使用重载方案</br>
        /// </summary>
        /// <param name="proj"></param>
        /// <returns></returns>
        public static Projectile SpawnInvisBoom(this Projectile proj, DamageClass damageClass, float damageRatios = 1,
            int lifeTime = 40, int hitCD = 40, int resize = 100, int hitTime = -1, int buffID = -1, int buffTime = -1, int maxUpdates = -1, NPC targetMounted = null)
        {
            Projectile boom = Projectile.NewProjectileDirect(proj.GetSource_FromThis(), proj.Center, Vector2.Zero, ProjectileType<InvisBoom>(), (int)(proj.damage * damageRatios), 0, proj.owner);
            boom.Resize(resize);
            if (maxUpdates > 0)
                boom.MaxUpdates = maxUpdates;
            if (proj.ModProjectile is InvisBoom boom1)
            {
                boom1.SetUpBoom(buffID, buffTime, lifeTime, hitCD, hitTime, damageClass, targetMounted);
            }
            return boom;
        }
        /// <summary>
        /// 快速生成一个不可见的爆炸射弹。
        /// <br>通过 <see cref="InvisBoomOptions"/> 指定参数，未指定的部分使用默认值。</br>
        /// </summary>
        public static Projectile SpawnInvisBoom(this Projectile proj, InvisBoomOptions options)
        {
            Projectile boom = Projectile.NewProjectileDirect(
                proj.GetSource_FromThis(),
                proj.Center,
                Vector2.Zero,
                ProjectileType<InvisBoom>(),
                (int)(proj.damage * options.DamageRatio),
                0,
                proj.owner);

            boom.Resize(options.Resize);

            if (options.MaxUpdates > 0)
                boom.MaxUpdates = options.MaxUpdates;

            if (boom.ModProjectile is InvisBoom boomMod)
            {
                boomMod.SetUpBoom(
                    options.BuffID,
                    options.BuffTime,
                    options.LifeTime,
                    options.HitCooldown,
                    options.HitTime,
                    options.DamageClass,
                    options.TargetMounted);
            }

            return boom;
        }
        #endregion
    }
}
