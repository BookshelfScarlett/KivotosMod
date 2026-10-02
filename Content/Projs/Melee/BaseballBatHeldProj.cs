using JetBrains.Annotations;
using KivotosMod.Assets.Register;
using KivotosMod.Content.Items.Weapons.Melee;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using Steamworks;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Melee
{
    public class BaseballBatHeldProj : KivotosProjsHeld
    {
        protected override DamageClass SetDamageClass => DamageClass.Melee;
        public override string Texture => GetInstance<BaseballBat>().Texture;
        protected override int OriginalItemID => ItemType<BaseballBat>();
        protected override int ExtraUpdates => 5;
        protected virtual float SwingSpeeedRatios => 1f;
        protected virtual float BatScale => 1f;
        public AnimationStruct Helper = new AnimationStruct(2);
        public float TargetRotation = 0;
        public bool Flip = false;
        public float SwordHeight = 1f;
        public float SwordWidth = 1.2f;
        public float StopTiming = 0;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(9);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.SetupImmnuity(-1);
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnFirstFrame()
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.BaseballBatSwing, Projectile.Center);
            Helper.MaxProgress[0] = (int)(AttackSpeed * SwingSpeeedRatios * .5f);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .5f * SwingSpeeedRatios);
            TargetRotation = Owner.Center.GetNormalVector2(Main.MouseWorld).ToRotation();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            UpdateAnimation();
            UpdateHeldState();
        }
        public void UpdateHeldState()
        {
            Projectile.ControlHoldout(Owner);
            Projectile.Center = Owner.MountedCenter;
            Projectile.position.Y += Owner.gfxOffY;

            Projectile.velocity = TargetRotation.ToRotationVector2();
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
        }

        public void UpdateAnimation()
        {
            if (StopTiming > 0)
            {
                StopTiming--;
                return;
            }
            if (!Helper.IsDone[0])
            {
                UpdateBeginAnimation();
            }
            else if (!Helper.IsDone[1])
            {
                UpdateEndAnimation();
            }
            else
            {
                Projectile.Kill();
            }
        }
        protected virtual void SlashFunc(Vector2 tarPos)
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .81f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection) * Main.rand.NextFloat() * 3f;
                Dust d = Dust.NewDustPerfect(pos, DustID.UnusedBrown);
                d.velocity = vel;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(.9f, 1f);
            }
        }

        #region 挥砍动画的具体案例
        /// <summary>
        /// 这里的案例专门写具体，用于让你看懂这里面每一行码在做什么
        /// <br>这样，如果你需要使用这种动画工具去做自定义挥砍，会比较方便</br>
        /// <br>这个挥砍的写法最大问题应该是容易臃肿，但可以比较细致地控制你需要的动画进程</br>
        /// </summary>
        public void UpdateBeginAnimation()
        {
            Helper.UpdateAniState(0);
            float heldScale = Owner.HeldItem.scale * BatScale;
            //利用这个动画工具，我们获得其归一化比率，并且使用缓动函数去获得我们需要的变化速率
            //这个动画缓动进程为武器挥砍动画的核心
            float easedProgress = EasingFunction.EaseOutExpo(Helper.GetAniProgress(0));
            //设置起始角度
            float beginAngle = 180f * Flip.ToDirectionInt();
            //设置末尾角度
            float endAngle = -160f * Flip.ToDirectionInt();

            //将起始角度和末尾角度转化为弧度
            float startAngleOffset = ToRadians(beginAngle);
            float endAngleOffset = ToRadians(endAngle);
            //在起止角度之间做线性插值，得到当前帧应该到达的旋转角
            float baseRotation = Lerp(startAngleOffset, endAngleOffset, easedProgress);
            //如果有翻转，则翻转。
            if (Owner.direction == -1)
                baseRotation *= Owner.direction;

            //把旋转角和椭圆缩放组合成一个2D变换矩阵：
            //旋转用Z轴，即泰拉的2D 平面上用不到的轴表示
            //SwordWidth与SwordHeight则控制挥砍轨迹的形状：
            //两者相等是正圆，不等就是椭圆，可以做出横向/纵向拉长的挥砍
            Matrix transForm = Matrix.CreateRotationZ(baseRotation) * Matrix.CreateScale(SwordWidth, SwordHeight, 1);

            //把单位向量X应用这个变换，得到当前帧刀光的"方向+长度"复合向量
            //乘以heldScale是让刀光长度跟随武器缩放
            //targetPos此时携带了模长，不是单位向量
            Vector2 targetPos = Vector2.Transform(Vector2.UnitX, transForm) * heldScale;
            //此时，targetPos的模长就是当前射弹的大小（Scale）
            Projectile.scale = targetPos.Length();
            //最后，实际修改射弹的转角
            //这里的修改是玩家在初始状态下设定的基准角度后，通过控制起始角度到末尾角度差的递增来实现角度变化
            Projectile.rotation = targetPos.ToRotation() + TargetRotation;
            //在实际开始挥砍之前略微修正玩家的指向，发起挥砍之后我们不会再试图进行修改
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards((Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX).ToRotation(), .5f);
            else
            {
            //利用这个动画工具，我们获得其归一化比率，并且使用缓动函数去获得我们需要的变化速率
            //这个动画缓动进程为武器挥砍动画的核心
            //设置起始角度
            easedProgress = EasingFunction.EaseOutExpo(Helper.GetAniProgress(0));
            float slashbeginAngle = 190f * Flip.ToDirectionInt();
            //设置末尾角度
            float slashendAngle = -180f * Flip.ToDirectionInt();
            //开始应用封装工具
            float slashrot = Helper.UpdateAngle(slashbeginAngle, slashendAngle, Owner.direction, easedProgress);
            //直接获得结果的模长targetPos
            Vector2 slashtargetPos = slashrot.ToTargetPosByMartix(heldScale, SwordWidth, SwordHeight);

                //在开始挥砍之后你可以做的事情，比如说粒子生成，或者存储顶点数据
                if(easedProgress<Main.rand.NextFloat(.9f,1f))
                SlashFunc(slashtargetPos);
            }
        }
        /// <summary>
        /// 看到这这里末尾动画前你应当看完了起始动画的处理
        /// <br>这里末尾动画就会开始使用动画工具里面的封装方法</br>
        /// </summary>
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(1);
            float heldScale = Owner.HeldItem.scale * BatScale;
            //利用这个动画工具，我们获得其归一化比率，并且使用缓动函数去获得我们需要的变化速率
            //这个动画缓动进程为武器挥砍动画的核心
            float easedProgress = EasingFunction.EaseInOutExpo(Helper.GetAniProgress(1));
            //设置起始角度
            float beginAngle = -160f * Flip.ToDirectionInt();
            //设置末尾角度
            float endAngle = -180f * Flip.ToDirectionInt();
            //开始应用封装工具
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            //直接获得结果的模长targetPos
            Vector2 targetPos = rot.ToTargetPosByMartix(heldScale, SwordWidth, SwordHeight);
            //此时，targetPos的模长就是当前射弹的大小（Scale）
            Projectile.scale = targetPos.Length();
            //最后，实际修改射弹的转角
            //这里的修改是玩家在初始状态下设定的基准角度后，通过控制起始角度到末尾角度差的递增来实现角度变化
            Projectile.rotation = targetPos.ToRotation() + TargetRotation;
            //因为已经退出了挥砍，这里只是为了让玩家手持的武器继续旋转一段时间
            TargetRotation = TargetRotation.AngleTowards(Projectile.Center.GetNormalVector2(Main.MouseWorld).ToRotation(), .05f);
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.BaseballBatHit with { Variants = [1] }, target.Center);
            Vector2 puncDir = Projectile.Center.GetNormalVector2(target.Center);
            ScreenShakeSystem.AddScreenShakes(target.Center, 10, 10, puncDir.ToRotation(), 0.1f);
            target.PunchTarget(puncDir, 10);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(2), RandVelTwoPi(2, 12), RandLerpColor(Color.DarkOrange, Color.Orange),
                    Main.rand.Next(30, 51), RandRotTwoPi, 1, Main.rand.NextFloat(.85f, 1.15f) * .3f, false, BlendState.Additive);
            }
        }
        public override void OnKill(int timeLeft)
        {

            //我们会在kill的时候刷新一个全新的射弹
            //这样做有一定的好处就是可以完全重置掉射弹当前的动画状态（处死的方式），从而省去大量的重置步骤
            //坏处就是，如果需要进行极其精细的调整（如特效相关），直接处死会打断这些东西
            //但对于大多数而且，处死会更为方便
            if (Main.mouseLeft && OwnerIsLegal)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                //下一次挥砍镜像于本次挥砍
                ((BaseballBatHeldProj)proj.ModProjectile).Flip = !Flip;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            float easedProgress = EasingFunction.EaseOutCubic(Helper.GetAniProgress(0));
            if (easedProgress < 0.01f)
                return false;
            float _ = float.NaN;
            Vector2 beamBeginPos = Owner.Center;
            Vector2 beamEndPos = Projectile.Center + (Projectile.rotation).ToRotationVector2() * Projectile.scale * 98;
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 54f, ref _);
            return c;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            Projectile.GetDrawDataMelee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            int length = Projectile.oldPos.Length - 2;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float rot = Projectile.oldRot[i] + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
                float opac = Lerp(0.05f, 1f, ratios) * .30f;
                Color c = Color.Lerp(Color.White, Color.Brown, ratios).ToAddColor(75);
                SB.FastDraw(tex, pos, c * opac, rot, rotationPoint, Projectile.scale, flipSprite);
            }
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
    public class MetalBaseballBatHeldProj : BaseballBatHeldProj
    {
        protected override int OriginalItemID => ItemType<MetalBaseballBat>();
        public override string Texture => GetInstance<MetalBaseballBat>().Texture;
        protected override float BatScale => 1.2f;
        protected override float SwingSpeeedRatios => 1.1f;
        protected override void SlashFunc(Vector2 tarPos)
        {
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Vector2.Lerp(Projectile.Center, Projectile.Center + tarPos.RotatedBy(TargetRotation) * 90, Main.rand.NextFloat(0.41f, .81f));
                Vector2 dir = (pos - Projectile.Center).ToSafeNormalize(Vector2.UnitX);
                Vector2 vel = dir.RotatedBy(PiOver2 * Projectile.spriteDirection) * Main.rand.NextFloat() * 3f;
                Dust d = Dust.NewDustPerfect(pos, DustID.SilverCoin);
                d.velocity = vel;
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(.9f, 1f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft && OwnerIsLegal)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                //下一次挥砍镜像于本次挥砍
                ((MetalBaseballBatHeldProj)proj.ModProjectile).Flip = !Flip;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(KivotosSoundsAssets.BaseballBatHit with { Variants = [2],Volume=.5f,Pitch=-.3f,PitchVariance=.1f }, target.Center);
            Vector2 puncDir = Projectile.Center.GetNormalVector2(target.Center);
            ScreenShakeSystem.AddScreenShakes(target.Center, 14, 14, puncDir.ToRotation(), 0.1f);
            target.PunchTarget(puncDir, 10);
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.SmokeParticle(target.Center.ToRandCirclePos(2), RandVelTwoPi(2, 12), RandLerpColor(Color.White, Color.DarkGray),
                    Main.rand.Next(30, 51), RandRotTwoPi, 1, Main.rand.NextFloat(.85f, 1.15f) * .3f, false, BlendState.AlphaBlend);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            Projectile.GetDrawDataMelee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            int length = Projectile.oldPos.Length - 2;
            for (int i = length - 1; i >= 0; i--)
            {
                float ratios = 1 - i / (float)length;
                Vector2 pos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float rot = Projectile.oldRot[i] + (Projectile.spriteDirection == -1 ? PiOver2 + PiOver4 : PiOver4);
                float opac = Lerp(0.05f, 1f, ratios) * .30f;
                Color c = Color.Lerp(Color.White, Color.DarkGray, ratios).ToAddColor(75);
                SB.FastDraw(tex, pos, c * opac, rot, rotationPoint, Projectile.scale, flipSprite);
            }
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
    }
    public class PracticeCricketBatHeldProj : BaseballBatHeldProj
    {
        protected override int OriginalItemID => ItemType<PracticeCricketBat>();
        public override string Texture => GetInstance<PracticeCricketBat>().Texture;
        protected override float BatScale => .9f;
        protected override float SwingSpeeedRatios => .9f;
        protected override void SlashFunc(Vector2 tarPos)
        {
            base.SlashFunc(tarPos);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return base.PreDraw(ref lightColor);
        }
    }
}

