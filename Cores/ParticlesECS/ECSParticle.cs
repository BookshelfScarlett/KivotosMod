using KivotosMod.Globals.Graphics.ParticlesECS;
using Terraria;

namespace KivotosMod.Cores.ParticlesECS
{
    /// <summary>
    /// 粒子ECS的快捷方法类，提供了创建和管理粒子实体的静态方法。
    /// <para>如果你不想在每次生成的时候都不知道自己在生成什么，最好创建粒子的时候，就往这里注册一个对应粒子的工具方法</para>
    /// </summary>
    public static class ECSParticle
    {
        /// <summary>
        /// <para><paramref name="smallDrawMult"/>用于表示重复绘制的次数，该十字星是用ex98重复绘制多次组起来的 </para>
        /// <para>更高的值代表更少的绘制次数，对于及其巨量的粒子生成情况下，减少绘制次数可以提升性能</para>
        /// </summary>
        /// <returns></returns>
        public static int ShinyCrossStarECS(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float smallDrawMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<ShinyCrossStar>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: smallDrawMult);
        }
        public static int HRShinyOrb(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float glowMult = 0.1f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<HRShinyOrb>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: glowMult);
        }
        public static int SmokeParticle(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rot, float opacity, float scale, bool alt = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            float Ai0 = alt ? 1 : 0;
            return ECSMethod.NewParticle(GetInstance<SmokeParticle>().Type, timeLeft, pos, vel, color, opacity, rot, scale, blendstate: bs, ai0: Ai0);
        }
        /// <summary>
        /// <para><paramref name="drawTime"/>用于表示重复绘制的次数，该粒子是一个重复绘制多次的ex98长棱形</para>
        /// <para>更高的值代表更少的绘制次数，对于及其巨量的粒子生成情况下，减少绘制次数可以提升性能</para>
        /// <para>当然，同样会缩短这个棱形的长度</para>
        /// </summary>
        /// <returns></returns>
        public static int LightntingGlow(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, int drawTime = 6, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<LightningGlowParticle>().Type, timeLeft, pos, vel, color, opacity, 0, scale: scale, blendstate: bs, ai0: drawTime);

        }
        /// <summary>
        /// ECS的StarShape粒子，默认以velocity的角度
        /// </summary>

        public static int StarShape(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float glowMult = 0.5f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<StarShapeECS>().Type, timeLeft, pos, vel, color, opacity, 0, scale: scale, blendstate: bs, ai0: glowMult);
        }
        /// <summary>
        /// ECS的StarShape粒子，重载自定义转向方法
        /// </summary>
        public static int StarShape(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rotation, float opacity, float scale, float glowMult = 0.5f, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<StarShapeECS>().Type, timeLeft, pos, vel, color, opacity, rotation, scale: scale, blendstate: bs, ai0: glowMult, ai1: 9);
        }
        public static int BloodDrop(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, int edgeMultTime = 0, bool fullBright = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            return ECSMethod.NewParticle(GetInstance<BloodDrop>().Type, timeLeft, pos, vel, color, opacity, 0, scale, bs, fullBright.ToInt(), edgeMultTime);
        }
        /// <summary>
        ///<para><paramref name="fullBright"/>用于表示粒子是否应该无视光照</para>
        ///<para>尽管如此，这个粒子非常暗，无视光照的情况下可能也很难在夜间看清其行为</para>
        /// </summary>
        /// <returns></returns>
        public static int LiliesFire(Vector2 pos, Vector2 vel, Color color, int timeLeft, float rot, float opacity, float scale, bool fullBright = false, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.NonPremultiplied;
            float Ai0 = fullBright ? 1 : 0;
            return ECSMethod.NewParticle(GetInstance<LiliesFire>().Type, timeLeft, pos, vel, color, opacity, rot, scale, blendstate: bs, ai0: Ai0);
        }
        /// <summary>
        /// <para><paramref name="glowMult"/>表示中心辉光的大小，<paramref name="direction"/>用于表示湍流粒子的旋转偏移量，默认为TwoPi的弧度</para>
        /// </summary>
        /// <returns></returns>
        public static int TurbulenceShinyOrb(Vector2 pos, float speed, Color color, int lifeTime, float opacity, float scale, float direction = TwoPi, float glowMult = 0, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<TurbulenceShinyOrb>().Type, lifeTime, pos, Main.rand.NextFloat(TwoPi).ToRotationVector2(), color, opacity, direction, scale, bs, speed, glowMult);
        }
        /// <summary>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int KiraStar(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, bool altVersion = false, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<KiraStar>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, fadinTime, scale, altVersion.ToInt());
        }
        /// <summary>
        /// <para>一个重载方案，跳过了<paramref name="rotation"/>（设定为0）和<paramref name="vel"/>（设定为0）的传参</para>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int KiraStar(Vector2 pos, Color color, int timeLeft, float opacity, float scale, bool altVersion = false, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<KiraStar>().Type, timeLeft, pos, Vector2.Zero, color, opacity, 0, scale, bs, fadinTime, scale, altVersion.ToInt());
        }
        /// <summary>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int CrossGlow(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<CrossGlow>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai0: fadinTime);
        }
        /// <summary>
        /// <para>一个重载方案，跳过了<paramref name="rotation"/>（设定为0）和<paramref name="vel"/>（设定为0）的传参</para>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int CrossGlow(Vector2 pos, Color color, int timeLeft, float opacity, float scale, float fadinTime = 0.4f, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<CrossGlow>().Type, timeLeft, pos, Vector2.Zero, color, opacity, 0, scale, bs, ai0: fadinTime);
        }
        /// <summary>
        /// <para>超级复杂的传参</para>
        /// <para><paramref name="floatSpeed"/>为花瓣/落叶的漂浮速度大小，一般值越大，代表其速度越快</para>
        /// <para><paramref name="alterTexture"/>True时启用另一个不同的贴图</para>
        /// <para><paramref name="fullBright"/>是否无视光照，默认为false，即受环境光影响</para>
        /// <para><paramref name="noCollision"/>是否无视物块，在原AI中，花瓣会有物块碰撞判定</para>
        /// <para><paramref name="glowMult"/>是否绘制描边且重复多少次绘制，默认为0，即不描边，取1时，绘制8次</para>
        /// </summary>
        /// <returns></returns>
        public static int LiliesPetal(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, float floatSpeed, bool noCollision = false, int glowMult = 0, bool fullBright = false, bool alterTexture = false, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            float aifloat2 = alterTexture.ToInt();
            return ECSMethod.NewParticle(GetInstance<LiliesPetal>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai0: floatSpeed, ai1: scale, ai2: aifloat2, aibool1: noCollision, aibool2: fullBright, aiint2: glowMult);
        }
        /// <summary>
        /// <para><paramref name="thunderType"/>闪电种类，1：无帧图，2：有帧图 3：四散类</para> 
        /// </summary>
        /// <returns></returns>
        public static int HighResolutionThunder(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, int thunderType = 1, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<HighResolutionThunder>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, aiint0: thunderType);
        }
        //public static int ShinyCrossStarSmall(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float scale, float rotSpeed = 0.1f, BlendState blendstate = null)
        //{
        //    BlendState bs = blendstate ?? BlendState.Additive;
        //    return ECSMethod.NewParticle(GetInstance<ShinyCrossStarSmalle>().Type, timeLeft, pos, vel, color, opacity, scale: scale, blendstate: bs, ai0: rotSpeed);
        //}
        /// <summary>
        /// <paramref name="type"/>为圆环种类。1：普通圆环，2：硬边圆环，3：模糊空圆环，其他：发光圆环<br></br>
        /// <paramref name="fadinTime"/> 为这个十字辉光的淡入时间（归一化比率），如果设置为0则无淡入
        /// </summary>
        public static int Ring(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, float fadinTime = 0.4f, int type = 0, BlendState blendState = null)
        {
            BlendState bs = blendState ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<Ring>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai0: fadinTime, aiint0: type);
        }
        /// <summary>
        /// 抓痕轨迹，这个抓痕总体较小。<br></br>
        /// <paramref name="squashScale"/>为形变的大小，如果不输入，默认为<see langword="Vector2(1.2f,0.8f)"/><br></br>
        /// <paramref name="squashSpeedX"/>为这个抓痕对于x大小的形变速度，如果什么都不输入，默认为<see langword="0"/>，即无形变速度<br></br>
        /// <paramref name="squashSpeedY"/>为这个抓痕对于y大小的形变速度，如果什么都不输入，默认为<see langword="0"/>，即无形变速度<br></br>
        /// <paramref name="scale"/>总控这个抓痕的大小，与上述的形变大小另外区分
        /// </summary>
        /// <returns></returns>
        public static int Stain(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, Vector2? squashScale = null, float squashSpeedX = 0, float squashSpeedY = 0, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            Vector2 vec = squashScale ?? new Vector2(1.2f, .8f);
            return ECSMethod.NewParticle(GetInstance<Stain>().Type, timeLeft, pos, vel, color, vec, opacity, rotation, bs, squashSpeedX, squashSpeedY, scale);
        }
        /// <summary>
        /// 抓痕轨迹，这个抓痕总体较小。<br></br>
        /// <paramref name="squashScale"/>为形变的大小，如果不输入，默认为<see langword="Vector2(1.2f,0.8f)"/><br></br>
        /// <paramref name="squashSpeedX"/>为这个抓痕对于x大小的形变速度，如果什么都不输入，默认为<see langword="0"/>，即无形变速度<br></br>
        /// <paramref name="squashSpeedY"/>为这个抓痕对于y大小的形变速度，如果什么都不输入，默认为<see langword="0"/>，即无形变速度<br></br>
        /// <paramref name="scale"/>总控这个抓痕的大小，与上述的形变大小另外区分
        /// </summary>
        /// <returns></returns>
        public static int ShrinkParticle(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, int type, Vector2? squashScale = null, float squashSpeedX = 0, float squashSpeedY = 0, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            Vector2 vec = squashScale ?? new Vector2(1.2f, .8f);
            return ECSMethod.NewParticle(GetInstance<ShrinkParticle>().Type, timeLeft, pos, vel, color, vec, opacity, rotation, bs, squashSpeedX, squashSpeedY, scale, aiint0: type);
        }

        /// <summary>
        /// 发光方块。<br></br>
        /// <paramref name="rotSpeed"/>为方块的旋转速度，默认为<see langword="0"/><br></br>
        /// <paramref name="type"/>为方块的类型。1：较大，2：边缘较厚，其余：普通，默认为<see langword="0"/><br></br>
        /// <paramref name="glowMult"/>方块的内部发光，默认为<see langword="0"/>，即无形变速度<br></br>
        /// </summary>
        /// <returns></returns>
        public static int GlowSquare(Vector2 pos, Vector2 vel, Color color, int timeLeft, float opacity, float rotation, float scale, int type = 0, float rotSpeed = 0, float glowMult = 0, BlendState blendstate = null)
        {
            BlendState bs = blendstate ?? BlendState.Additive;
            return ECSMethod.NewParticle(GetInstance<GlowSquare>().Type, timeLeft, pos, vel, color, opacity, rotation, scale, bs, ai1: glowMult, aiint0: type, ai0: rotSpeed);
        }

    }
}
