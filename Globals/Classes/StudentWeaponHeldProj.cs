using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Classes
{
    /// <summary>
    /// 远程武器的手持射弹基类
    /// <br>这个基类专门用于实现类似荷鲁斯之眼<see langword="EyeofHorus"/>的手持显示效果，已经自动管理了绝大部分从攻击到绘制的内容</br>
    /// <br>一般情况下和大部分武显模组本身冲突。</br>
    /// </summary>
    public abstract class KivotosRangedWeaponProjectie: ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        /// <summary>
        /// 该射弹的原始归属物品的id
        /// <br>用于处理处死</br>
        /// </summary>
        public virtual int OriginalItemID => -1;
        public override string Texture => $"HJScarletRework/Assets/Texture/Projs/" + GetType().Name;
        /// <summary>
        /// 最低攻击频率，用于<see cref="AttackSpeed"/>
        /// </summary>
        public virtual int MinAttackRate => 5;
        /// <summary>
        /// 攻击速度，使用<see cref="Player.HeldItem"/>作为基础
        /// <br>这里的管理方案会自动将<see cref="Projectile.MaxUpdates"/>纳入计算</br>
        /// </summary>
        public virtual int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, MinAttackRate * Projectile.MaxUpdates);
        /// <summary>
        /// 额外更新，这个额外更新默认为<see langword="1"/>，即提供1额外更新
        /// <br>一般情况下会用于手持射弹本身的粒子特效</br>
        /// </summary>
        public virtual int ProjExtraUpdates => 2;
        /// <summary>
        /// 该远程武器的计时器，用于和<see cref="AttackSpeed"/>一起实际控制武器的攻击频率
        /// </summary>
        public ref float Timer => ref Projectile.ai[0];
        public ref float RecoilTimer => ref Projectile.ai[1];
        /// <summary>
        /// 后坐力动画的力度
        /// <br>如果选择复写<see cref="UpdateRecoil"/>则不会有任何作用</br>
        /// </summary>
        public virtual float RecoilPower => 10;
        /// <summary>
        /// 执行后坐力动画时，拉回武器的时刻
        /// <br>这是一个归一化比率，默认值为<see langword="0.13f"/>，即在13%进程时开始拉回</br>
        /// <br>自动管理，如果你完全复写了<see cref="UpdateRecoil"/>，则不会生效</br>
        /// </summary>
        public virtual float RecoilWeaponPullbackRatios => .13f;
        /// <summary>
        /// 手持武器的绘制偏移
        /// <br>不需要考虑玩家朝向（如果不选择复写<see cref="PreDraw(ref Color)"/>），因为会自动管理</br>
        /// </summary>
        public virtual Vector2 HoldoutOffset => Vector2.Zero;
        /// <summary>
        /// 手持射弹的大小
        /// </summary>
        public virtual float HoldoutDrawScale => 1f;
        /// <summary>
        /// 手持武器的描边颜色
        /// <br>这个描边颜色只在执行后坐力动画的时候出现</br>
        /// </summary>
        public virtual Color HoldoutEdgeColor => Color.White;
        /// <summary>
        /// 手持武器是否允许绘制描边
        /// </summary>
        public virtual bool HoldoutEdgeEnable => true;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.SetUpHeldProj(ProjExtraUpdates);
            ExSD();
        }
        protected virtual void ExSD() { }
        public override void AI()
        {
            if (!Projectile.Kivotos().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
        protected virtual void ProjAI()
        {
            UpdatePlayerState();
            //处理玩家手持该武器时的状态
            UpdateHeldProjectile();
            //后坐力动画
            UpdateRecoil();

            if (IsUsing)
            {
                Timer++;
                Owner.itemAnimation = Owner.itemTime = 2;
                UpdateWeaponUsing();
                if (Timer >= AttackSpeed && Projectile.IsMe())
                {
                    PreAttack();
                    OnAttack();
                    PostAttack();
                }
            }
            else
            {
                UpdateWeaponIdle();
            }
            UpdateGlobalReset();

        }

        protected virtual void OnFirstFrame()
        {
            Timer = (int)(AttackSpeed * .9f);
        }
        public virtual bool IsUsing => (Owner.channel) && !Owner.noItems && !Owner.CCed;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        protected virtual void UpdatePlayerState()
        {
            //手持物品不对，玩家状态不对，处死射弹
            if (Owner.IsHolding(OriginalItemID) && !Owner.CCed && !Owner.dead)
                Projectile.timeLeft = 2;

        }
        /// <summary>
        /// 在使用武器期间每帧调用，攻击判定之前。
        /// <br>可用于处理蓄力、持续消耗、状态叠加等前置逻辑。</br>
        /// </summary>
        protected virtual void UpdateWeaponUsing()
        {

        }
        /// <summary>
        /// 在执行<see cref="OnAttack"/>前执行
        /// <br>可用于一些发起攻击前的准备</br>
        /// </summary>
        protected virtual void PreAttack()
        {

        }
        /// <summary>
        /// 全局状态重置，无论状态每帧执行，且最后更新
        /// <br>可用于一些最后更新的其他功能，默认用于递减后坐力的计时器</br>
        /// </summary>

        protected virtual void UpdateGlobalReset()
        {
            if (RecoilTimer > 0)
                RecoilTimer--;
        }

        /// <summary>
        /// 仅在未攻击时的状态更新
        /// <br>仅在<see cref="IsUsing"/>为<see langword="false"/>时执行</br>
        /// <br>默认状态下让武器本身自然回复到攻击速度（<see cref="AttackSpeed"/>) </br>
        /// </summary>
        protected virtual void UpdateWeaponIdle()
        {
            //我做的不是灾厄，在没有攻击的时候计时器也会叠到attackspeed这的
            if (Timer < AttackSpeed)
                Timer++;
        }
        /// <summary>
        /// 在执行攻击之后的状态重置
        /// <br>在<see cref="OnAttack"/>后立刻执行</br>
        /// </summary>
        protected virtual void PostAttack()
        {
            Timer = 0;
            RecoilTimer = AttackSpeed;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        /// <summary>
        /// 武器的实际攻击效果
        /// <br>复写这个就可以实际进行攻击，怎么操作就看你了</br>
        /// <br>只支持左键</br>
        /// </summary>
        protected virtual void OnAttack()
        {

        }
        /// <summary>
        /// 玩家手持状态的控制
        /// </summary>
        protected virtual void UpdateHeldProjectile()
        {
            if (Projectile.IsMe())
            {
                Projectile.rotation = Owner.ToMouseVector2().ToRotation();
                Projectile.spriteDirection = Projectile.direction = (Main.MouseWorld.X > Owner.Center.X).ToDirectionInt();
                Owner.ChangeDir(Projectile.direction);
                Owner.heldProj = Projectile.whoAmI;
                Owner.ControlPlayerArm(Projectile.rotation);
                Projectile.Center = Owner.MountedCenter;
                Projectile.position.Y += Owner.gfxOffY;
            }

        }
        /// <summary>
        /// 武器后坐力动画的进程控制
        /// </summary>

        protected virtual void UpdateRecoil()
        {
            float progress = Utils.GetLerpValue(AttackSpeed, 0, RecoilTimer, true);
            float pullback;
            float rot = (Projectile.Center - Main.MouseWorld).ToRotation() * Owner.gravDir;
            if (progress > RecoilWeaponPullbackRatios)
            {
                float pro = (1 - progress) / (1 - RecoilWeaponPullbackRatios);
                pullback = Lerp(0, RecoilPower, EasingFunction.EaseOutBack(pro));
            }
            else
            {
                float pro = progress / RecoilWeaponPullbackRatios;
                pullback = Lerp(0, RecoilPower, EasingFunction.EaseOutCubic(pro));
            }
            Projectile.Center += rot.ToRotationVector2() * pullback;
        }
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetRangedWeaponHeldProjData(out Texture2D tex, out Vector2 drawPos, out Vector2 rotPoint, out float _, out SpriteEffects se);
            Vector2 offset = HoldoutOffset * new Vector2(Owner.direction, 1);
            float drawRot = Projectile.rotation + (Projectile.spriteDirection == -1 ? Pi : 0);
            drawPos += offset.RotatedBy(drawRot);

            float scale = Projectile.scale * HoldoutDrawScale;
            if (HoldoutEdgeEnable)
            {
                float progress = Utils.GetLerpValue(0, AttackSpeed, RecoilTimer, true);
                float edgeProgress = EasingFunction.EaseInCubic(progress);
                for (int i = 0; i < 8; i++)
                {
                    if (edgeProgress <= .02f)
                        break;
                    DrawWeaponEdge(tex, drawPos + (TwoPi / 8f * i).ToRotationVector2() * 2.5f * edgeProgress, drawRot, rotPoint, scale, se);
                }
            }

            SB.Draw(tex, drawPos, null, Color.White, drawRot, rotPoint, scale, se, 0);
            return false;
        }

        protected virtual void DrawWeaponEdge(Texture2D tex, Vector2 drawPos, float drawRot, Vector2 rotPoint, float scale, SpriteEffects se)
        {
            SB.FastDraw(tex, drawPos, HoldoutEdgeColor.ToAddColor(), drawRot, rotPoint, scale, se);
        }
    }
}
