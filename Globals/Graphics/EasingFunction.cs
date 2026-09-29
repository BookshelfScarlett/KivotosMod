using System;

namespace KivotosMod.Globals.Graphics
{
    public static class EasingFunction
    {
        /// <summary>
        /// <br>缓动函数：Quadratic 缓入缓出 (Ease In Out Quad)。</br>
        /// <br>曲线呈 S 形，先加速后减速，全程速度变化平滑。</br>
        /// 适合大多数 UI 动画、物体往返运动。
        /// </summary>
        public static float EaseInOutQuad(float t)
        {
            if (!(t < 0.5f))
            {
                return 1f - (-2f * t + 2f) * (-2f * t + 2f) / 2f;
            }

            return 2f * t * t;
        }

        /// <summary>
        /// <br>缓动函数：Exponential 缓出 (Ease Out Expo)。</br>
        /// <br>开始时极快减速，最后缓慢趋近终值。</br>
        /// 呈迅速停下、带有余韵的效果；适合强调“快速结束”的动画，如飞入后停留。
        /// </summary>
        public static float EaseOutExpo(float t)
        {
            if (t != 1f)
            {
                return 1f - MathF.Pow(2f, -10f * t);
            }

            return 1f;
        }

        /// <summary>
        /// <br>缓动函数：Exponential 缓入缓出 (Ease In Out Expo)。</br>
        /// <br>两端缓慢，中间突变明显（相比 Quad 更陡峭）。</br>
        /// 开始和结束都极其平滑；适合需要强烈“推进-减速”感的动画，如大型物体移动。
        /// </summary>
        public static float EaseInOutExpo(float t)
        {
            if (!(t < 0.5f))
            {
                return 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;
            }

            return 2f * t * t;
        }

        /// <summary>
        /// <br>缓动函数：Cubic 缓入 (Ease In Cubic)。</br>
        /// <br>起步非常慢，然后急剧加速。</br>
        /// 类似物体从静止被猛推出去；适合表现“重力落下”或“突然发射”的效果。
        /// </summary>
        public static float EaseInCubic(float t)
        {
            return t * t * t;
        }

        /// <summary>
        /// <br>缓动函数：Cubic 缓出 (Ease Out Cubic)。</br>
        /// <br>快速启动，随后减速滑停，比 Quadratic 缓出更“柔和”。</br>
        /// 适合物体弹出、旋转停止等自然阻尼运动。
        /// </summary>
        public static float EaseOutCubic(float t)
        {
            return (float)(1.0 - Math.Pow(1f - t, 3.0));
        }

        /// <summary>
        /// <br>缓动函数：Back 缓出 (Ease Out Back)。</br>
        /// <br>超过终值一点再弹回，产生“回弹/过冲”效果。</br>
        /// 结尾有振荡感；适合模拟弹簧、橡皮筋拉长后回缩的动画。
        /// </summary>
        public static float EaseOutBack(float t)
        {
            if (t == 1f)
            {
                return 1f;
            }

            return (float)(1.0 + 2.7015800476074219 * Math.Pow(t - 1f, 3.0) + 1.7015800476074219 * Math.Pow(t - 1f, 2.0));
        }

        /// <summary>
        /// <br>缓动函数：Back 缓入 (Ease In Back)。</br>
        /// <br>开始前先回退一点再正向加速，产生“蓄力”感觉。</br>
        /// 适合表现拉弓、蓄力攻击的预备动作。
        /// </summary>
        public static float EaseInBack(float t)
        {
            if (t == 1f)
            {
                return 1f;
            }

            return 2.70158f * t * t * t - 1.70158f * t * t;
        }

        /// <summary>
        /// <br>缓动函数：正弦缓入缓出 (Ease In Out Sin)。</br>
        /// <br>使用 sin(π * t) 曲线，全程平滑且对称，没有突变点。</br>
        /// 适合呼吸灯、周期性摆动、淡入淡出等自然柔和的效果。
        /// </summary>
        public static float EaseInOutSin(float t)
        {
            return (float)Math.Sin(MathF.PI * t);
        }
    }
}
