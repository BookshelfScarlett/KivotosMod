using KivotosMod.Assets.Register;
using KivotosMod.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace KivotosMod.Content.Raritys.Helper
{
    public static partial class RarityDrawHelper
    {
        /// <summary>
        /// 绘制自定义的TooltipLine，带有发光描边和主文本颜色。
        /// <br>这个会包括背景</br>
        /// </summary>
        /// <param name="tooltipLine"></param>
        /// <param name="glowColor"></param>
        /// <param name="edgeColor"></param>
        /// <param name="mainColor"></param>
        /// <param name="glowScaleMult"></param>
        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine, Color glowColor, Color edgeColor, Color mainColor, float glowScaleMult = 1.2f)
        {
            string textValue = tooltipLine.Text;
            Vector2 textSize = tooltipLine.Font.MeasureString(textValue);
            Vector2 textCenter = textSize * 0.5f;
            Vector2 textPosition = new(tooltipLine.X, tooltipLine.Y);
            Vector2 glowPosition = new(tooltipLine.X + textCenter.X, tooltipLine.Y + textCenter.Y / 1.5f);
            Vector2 glowScale = new Vector2(textSize.X * 0.135f, 0.6f) * glowScaleMult;
            //绘制需要的……发光背景。
            Main.spriteBatch.Draw(KivotosTextureAssets.Texture_RarityGlow.Value, glowPosition, null, glowColor.ToAddColor() * 0.85f, 0f, KivotosTextureAssets.Texture_RarityGlow.Value.Size() / 2f, glowScale, SpriteEffects.None, 0f);

            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * .35f)) / 2);
            float sineOffset = Lerp(0.75f, 1f, sine);

            //绘制发光描边，带渐变
            for (int i = 0; i < 16; i++)
            {
                Vector2 afterimageOffset = (TwoPi * i / 16f).ToRotationVector2() * (1.5f * sineOffset);
                ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, (textPosition + afterimageOffset).RotatedBy(TwoPi * (i / 12)), edgeColor * 0.9f, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
            }

            //绘制主文本颜色
            Color mainTextColor = mainColor;
            ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, textPosition, mainTextColor, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
        }
        /// <summary>
        /// 绘制自定义的TooltipLine，带有发光描边和主文本颜色。
        /// <br>不包括背景颜色</br>
        /// </summary>
        /// <param name="tooltipLine"></param>
        /// <param name="edgeColor"></param>
        /// <param name="mainColor"></param>
        public static void DrawCustomTooltipLine(DrawableTooltipLine tooltipLine, Color edgeColor, Color mainColor)
        {
            string textValue = tooltipLine.Text;
            Vector2 textPosition = new(tooltipLine.X, tooltipLine.Y);
            float sine = (float)((1 + Math.Sin(Main.GlobalTimeWrappedHourly * 0.35f)) / 2);
            float sineOffset = Lerp(0.75f, 1f, sine);

            //绘制发光描边，带渐变
            for (int i = 0; i < 12; i++)
            {
                Vector2 afterimageOffset = (TwoPi * i / 12f).ToRotationVector2() * (1.5f * sineOffset);
                ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, (textPosition + afterimageOffset).RotatedBy(TwoPi * (i / 12)), edgeColor * 0.9f, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
            }

            //绘制主文本颜色
            Color mainTextColor = mainColor;
            ChatManager.DrawColorCodedString(Main.spriteBatch, tooltipLine.Font, textValue, textPosition, mainTextColor, tooltipLine.Rotation, tooltipLine.Origin, tooltipLine.BaseScale);
        }

        /// <summary>
        /// 炼狱复制
        /// </summary>
        public static void UpdateTooltipParticles(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> sparklesList, BlendState blendState = null)
        {
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            //手动在这里更新一下所有的draw
            for (int i = 0; i < sparklesList.Count; i++)
            {
                sparklesList[i].CustomUpdate();
                sparklesList[i].Time++;
            }
            //在需要的时候删除掉粒子
            sparklesList.RemoveAll((RaritySparkle s) => s.LifetimeRatio >= 1);
            //而后，绘制所有的粒子
            foreach (RaritySparkle sparkle in sparklesList)
                sparkle.CustomDraw(Main.spriteBatch, new Vector2(tooltipLine.X, tooltipLine.Y) + textSize * 0.5f + sparkle.Position);
        }
        public static Vector2 GetParticlePosition(DrawableTooltipLine line)
        {
            Vector2 textSize = line.Font.MeasureString(line.Text);
            return Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
        }
        public static List<RaritySparkle> RaritySparklesList = [];
        public static List<RaritySparkle> FlavorSparklesList = [];
        public static void CleanUpSparkles()
        {
            RaritySparklesList.Clear();
            FlavorSparklesList.Clear();
        }
    }
}
