using Terraria;

namespace KivotosMod.Globals.Methods.GlobalHandlers
{
    /// <summary>
    /// 这里主要集中的是一些全局应用
    /// <br>默认GlobalUsing</br>
    /// </summary>
    public static partial class KivotosGlobalHandlers
    {
        public static float RandRotTwoPi => Main.rand.NextFloat(TwoPi);
        public static Vector2 RandVelTwoPi(float beginSpeed = 0, float endSpeed = 1f) => RandRotTwoPi.ToRotationVector2() * Main.rand.NextFloat(beginSpeed, endSpeed);
        public static Color RandLerpColor(Color beginColor, Color endColor) => Color.Lerp(beginColor, endColor, Main.rand.NextFloat());
    }
}
