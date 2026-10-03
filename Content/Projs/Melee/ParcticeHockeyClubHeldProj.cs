using KivotosMod.Content.Items.Weapons.Melee;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;


namespace KivotosMod.Content.Projs.Melee
{
    public class PracticeHockeyClubHeldProj : KivotosProjsHeld
    {
        protected override DamageClass SetDamageClass => DamageClass.Melee;
        public override string Texture => GetInstance<PracticeHockeyClub>().Texture;
        protected override int OriginalItemID => ItemType<PracticeHockeyClub>();
        protected override int ExtraUpdates => 5;
        protected virtual float SwingSpeeedRatios => 1f;
        protected virtual float BatScale => 1f;
        public AnimationStruct Helper => new AnimationStruct(2);
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
            Helper.MaxProgress[0] = (int)(AttackSpeed * SwingSpeeedRatios);
            Helper.MaxProgress[1] = (int)(AttackSpeed * .95f);
            TargetRotation = Owner.Center.GetNormalVector2(Main.MouseWorld).ToRotation();
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.velocity = Projectile.velocity.ToSafeNormalize();
            base.ProjAI();
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
        protected void SlashFunc()
        {

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
            float easedProgress = EasingFunction.EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = 180f * Flip.ToDirectionInt();
            float endAngle = -170f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Vector2 targetPos = rot.ToTargetPosByMartix(heldScale, SwordWidth, SwordHeight);
            Projectile.scale = targetPos.Length();
            Projectile.rotation = targetPos.ToRotation() + TargetRotation;
            if (easedProgress < .01f)
                TargetRotation = TargetRotation.AngleTowards((Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX).ToRotation(), .5f);
            else
            {
                //在开始挥砍之后你可以做的事情，比如说粒子生成，或者存储顶点数据
                SlashFunc();
            }
        }
        /// <summary>
        /// 看到这这里末尾动画前你应当看完了起始动画的处理
        /// <br>这里末尾动画就会开始使用动画工具里面的封装方法</br>
        /// </summary>
        public void UpdateEndAnimation()
        {
            Helper.UpdateAniState(0);
            float heldScale = Owner.HeldItem.scale * BatScale;
            float easedProgress = EasingFunction.EaseOutCubic(Helper.GetAniProgress(0));
            float beginAngle = -170f * Flip.ToDirectionInt();
            float endAngle = -180f * Flip.ToDirectionInt();
            float rot = Helper.UpdateAngle(beginAngle, endAngle, Owner.direction, easedProgress);
            Vector2 targetPos = rot.ToTargetPosByMartix(heldScale, SwordWidth, SwordHeight);
            Projectile.scale = targetPos.Length();
            Projectile.rotation = targetPos.ToRotation() + TargetRotation;
            TargetRotation = TargetRotation.AngleTowards(Projectile.Center.GetNormalVector2(Main.MouseWorld).ToRotation(), .05f);
        }
        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            if (Main.mouseLeft && OwnerIsLegal)
            {
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity, Projectile.type, Projectile.originalDamage, Projectile.knockBack, Projectile.owner);
                //下一次挥砍镜像于本次挥砍
                ((PracticeHockeyClubHeldProj)proj.ModProjectile).Flip = !Flip;
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
            bool c = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), beamBeginPos, beamEndPos, 64f, ref _);
            return c;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            Projectile.GetDrawDataMelee(out Texture2D tex, out Vector2 drawPosition, out float drawRotation, out Vector2 rotationPoint, out SpriteEffects flipSprite);
            SB.FastDraw(tex, drawPosition, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            return false;
        }
        public void UpdateHeldState()
        {
            Projectile.ControlHoldout(Owner);
            Projectile.velocity = TargetRotation.ToRotationVector2();
            Projectile.spriteDirection = Flip.ToDirectionInt() * Projectile.direction;
            Owner.ChangeDir(Projectile.direction);
            Owner.ControlPlayerArm(Projectile.rotation);
        }
    }
}
