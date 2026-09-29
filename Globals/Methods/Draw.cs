using Terraria;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        public static bool OutOffScreen(Vector2 pos)
        {
            if (pos.X < Main.screenPosition.X - Main.screenWidth / 2)
                return true;

            if (pos.Y < Main.screenPosition.Y - Main.screenHeight / 2)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth * 1.5f)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight * 1.5f)
                return true;

            return false;
        }
        public static bool OutOffScreen(Vector2 pos, float areamult = 1f)
        {
            float halfwidth = Main.screenWidth / 2;
            float halfheight = Main.screenHeight / 2;
            if (pos.X < Main.screenPosition.X - halfwidth * areamult)
                return true;

            if (pos.Y < Main.screenPosition.Y - halfheight * areamult)
                return true;

            if (pos.X > Main.screenPosition.X + Main.screenWidth + halfwidth * areamult)
                return true;
            if (pos.Y > Main.screenPosition.Y + Main.screenHeight + halfheight * areamult)
                return true;

            return false;
        }
        /// <summary>
        /// 将<paramref name="color"/>的Alpha值设置为<paramref name="alphaValue"/>，并返回新的Color对象。
        /// </summary>
        /// <param name="color"></param>
        /// <param name="alphaValue"></param>
        /// <returns></returns>
        public static Color ToAddColor(this Color color, byte alphaValue = 0) => color with { A = alphaValue };
        /// <summary>
        /// 快速绘制方案，省略了<paramref name="sourceRectangle"/>参数，直接使用<see langword="null"/>。
        /// </summary>
        public static void FastDraw(this SpriteBatch sb, Texture2D tex, Vector2 pos, Color c, float rotation, Vector2 origin, float scale, SpriteEffects se, int wtfisthis = 0)
        {
            sb.Draw(tex, pos, null, c, rotation, origin, scale, se, wtfisthis);
        }
        /// <summary>
        /// 快速绘制方案，省略了<paramref name="sourceRectangle"/>参数，直接使用<see langword="null"/>。
        /// <br>重载大小为Vector2</br>
        /// </summary>
        public static void FastDraw(this SpriteBatch sb, Texture2D tex, Vector2 pos, Color c, float rotation, Vector2 origin, Vector2 scale, SpriteEffects se, int wtfisthis = 0)
        {
            sb.Draw(tex, pos, null, c, rotation, origin, scale, se, wtfisthis);
        }
        /// <summary>
        /// 获取从<paramref name="beginPos"/>指向<paramref name="endPos"/>的单位向量，如果该向量为零，则返回<paramref name="normalvalue"/>（默认为Vector2.UnitX）。
        /// </summary>
        public static Vector2 GetNormalVector2(this Vector2 beginPos, Vector2 endPos, Vector2? normalvalue = null) => (endPos - beginPos).ToSafeNormalize(normalvalue);
        /// <summary>
        /// 将<paramref name="srcVel"/>归一化，如果该向量为零，则返回<paramref name="what"/>（默认为Vector2.UnitX）。
        /// </summary>
        public static Vector2 ToSafeNormalize(this Vector2 srcVel, Vector2? what = null) => srcVel.SafeNormalize(what ?? Vector2.UnitX);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2 ToRandCirclePosEdge(this Vector2 pos, float valueX = 2f, float? valueY = null)
        {
            float edgeY = valueY ?? valueX;
            return pos + Main.rand.NextVector2CircularEdge(valueX, edgeY);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2 ToRandCirclePos(this Vector2 pos, float valueX = 2f, float? valueY = null)
        {
            float edgeY = valueY ?? valueX;
            return pos + Main.rand.NextVector2Circular(valueX, edgeY);
        }
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float speed = 1f) => srcVel.ToSafeNormalize().RotatedBy(Main.rand.NextFloat(randRads) * Main.rand.NextBool().ToDirectionInt()) * speed;
        public static Vector2 ToRandVelocity(this Vector2 srcVel, float randRads, float minSpeed, float maxSpeed)
        {
            return srcVel.ToRandVelocity(randRads, Main.rand.NextFloat(minSpeed, maxSpeed));
        }
        public static void EnterShaderArea(this SpriteBatch SB)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EnterShaderArea(this SpriteBatch SB, SpriteSortMode mode, BlendState blendState)
        {
            SB.End();
            SB.Begin(mode, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EnterShaderArea(this SpriteBatch SB, BlendState blendState)
        {
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public static void EndShaderArea(this SpriteBatch SB)
        {
            SB.End();
            SB.BeginDefault();
        }
        public static void BeginDefault(this SpriteBatch SB) =>
          SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        public static Color RandLerpColor(this Color c1, Color c2) => Color.Lerp(c1, c2, Main.rand.NextFloat());
        public static Vector2 RandVector2() => Main.rand.NextFloat(TwoPi).ToRotationVector2();
         public static RenderTarget2D NewRT2D(float Mult = 1f)
        {
            return new RenderTarget2D(Main.graphics.GraphicsDevice, (int)(Main.screenWidth * Mult), (int)(Main.screenHeight * Mult));
        }
        /// <summary>
        /// 将当前渲染目标设置为提供的渲染目标。
        /// </summary>
        /// <param name="rt">要交换到的渲染目标</param>
        public static bool SwapToTarget(this RenderTarget2D rt)
        {
            GraphicsDevice gD = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Main.gameMenu || Main.dedServ || spriteBatch is null || rt is null || gD is null)
                return false;

            gD.SetRenderTarget(rt);
            gD.Clear(Color.Transparent);
            return true;
        }
        public static void ResetRT2D(this RenderTarget2D rt)
        {
            Vector2 size = rt.Size();
            Vector2 ScreenSize = new Vector2(Main.screenWidth, Main.screenHeight);
            if (size != ScreenSize)
            {
                Main.QueueMainThreadAction(() =>
                {
                    rt = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                });
            }
        }
        public static Vector2 GetScreenSize
        {
            get
            {
                return new Vector2(Main.screenWidth, Main.screenHeight);
            }
        }
    }
}
