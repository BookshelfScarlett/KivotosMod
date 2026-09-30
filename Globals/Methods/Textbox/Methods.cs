using KivotosMod.Assets.Register;
using KivotosMod.Globals.Configs;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace KivotosMod.Globals.Methods.Textbox
{
    public static class TextboxMethods
    {
        public class TextDrawElement
        {
            public string Text;
            public Vector2 Scale;
            public Color TextColor;
            public Color EdgeColor;
            //与本元素下方的间距
            public float SpacingAfter;
            public TextDrawElement(string text, Vector2 scale, Color textColor, Color edgeColor, float spacingAfter)
            {
                Text = text;
                Scale = scale;
                TextColor = textColor;
                EdgeColor = edgeColor;
                SpacingAfter = spacingAfter;
            }
            public Vector2 Size(DynamicSpriteFont font) => ChatManager.GetStringSize(font, Text, Scale);
        }
        public static float CalcBackgroundX(float lineX, float tooltipMaxWidth, float bgWidth, float spacing = 30f)
        {
            float bgX = lineX + tooltipMaxWidth + spacing;
            if (bgX + bgWidth > Main.screenWidth)
            {
                bgX = lineX - bgWidth - spacing;
                if (bgX < 0)
                    bgX = 0;
            }
            return bgX;
        }
        public static float CalcBackgroundY(float idealY, float bgHeight, float offsetY)
        {
            float maxY = idealY + bgHeight + offsetY;
            if (maxY > Main.screenHeight)
            {
                idealY -= (maxY - Main.screenHeight);
                if (idealY < 0)
                    idealY = 0;
            }
            return idealY;
        }
        public static void DrawTextWithOutlineCustom(TextDrawElement element, Vector2 position, Vector2 offset, float alpha)
        {
            SpriteBatch sb = Main.spriteBatch;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Color edge = element.EdgeColor * alpha;
            Color text = element.TextColor * alpha;

            for (int i = 0; i < 16; i++)
                ChatManager.DrawColorCodedString(sb, font, element.Text, position + (TwoPi / 16f * i).ToRotationVector2() * 1.2f + offset, edge, 0, Vector2.Zero, element.Scale);
            ChatManager.DrawColorCodedString(sb, font, element.Text, position + offset, text, 0, Vector2.Zero, element.Scale);
        }
        public static void DrawBackground(float x, float y, float width, float height, int padding, Color fillColor, Color? edgeColor, Vector2 offset)
        {
            DrawTextboxBackground(x, y, width - padding * 2, height - padding * 2, padding,
                new Vector2(x, y), fillColor, offset, edgeColor);
        }
        /// <summary>
        /// 在tootlip的右侧绘制文本框
        /// <br>这是一个重载方案，<paramref name="forcedBackgroundWidth"/>会强制指定文本框的背景宽度</br>
        /// <br>使用<see cref="DrawMultipleTextboxes(DrawableTooltipLine, IReadOnlyList{TooltipLine}, List{TextboxSettings}, float)"/>会自动调用该方法以确保所有文本框宽度一致</br>
        /// </summary>
        public static void DrawTextboxTooltipWithBackgroundMultline(DrawableTooltipLine line, IReadOnlyList<TooltipLine> cacheTooltip,
            ref TextboxSettings textboxSettings, float extraYOffset = 0, float maxWidth = -1, float? forcedBackgroundWidth = null)
        {
            if (cacheTooltip is null || line.Index != cacheTooltip.Count - 1)
                return;
            DynamicSpriteFont font = line.Font ?? FontAssets.MouseText.Value;
            Vector2 scale = line.BaseScale;
            if (scale == Vector2.Zero)
                scale = Vector2.One;

            //计算 Tooltip 最大文本宽度（仅用于定位右侧起始点）
            if (maxWidth < 0)
            {
                maxWidth = 0;
                foreach (var t in cacheTooltip)
                {
                    Vector2 size = ChatManager.GetStringSize(font, t.Text, scale);
                    if (size.X > maxWidth)
                        maxWidth = size.X;
                }
            }
            //获取浮动偏移
            float lerpValue = TextboxManager.LerpValue;
            Vector2 posOffset = Vector2.Lerp(Vector2.UnitY * -50f, Vector2.Zero, lerpValue)
                + Vector2.UnitY * (float)Math.Sin(Main.timeForVisualEffects / 60f) * 10f;
            var element = new List<TextDrawElement>();
            if (textboxSettings.HasTitle)
            {
                element.Add(new TextDrawElement(
                    "「" + textboxSettings.TitleText + "」",
                    scale * textboxSettings.TitleTextSize * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize,
                    textboxSettings.TitleTextColor,
                    textboxSettings.TitleEdgeColor,
                    5
                    ));
            }

            element.Add(new TextDrawElement(
                textboxSettings.MainText,
                scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize,
                textboxSettings.TextColor,
                textboxSettings.TextEdgeColor,
                    5
                ));
            //计算总的尺寸
            float contentWidth = element.Max(e => e.Size(font).X);
            float contentHeight = element.Sum(e => e.Size(font).Y + e.SpacingAfter) - (element.Last().SpacingAfter);
            //处理背景绘制
            int padding = 8;
            float bgWidth = (forcedBackgroundWidth ?? contentWidth) + padding * 2;
            float bgHeight = contentHeight + padding * 2;
            //定位北京坐标为左上角
            float bgX = CalcBackgroundX(line.X, maxWidth, bgWidth);
            float bgY = CalcBackgroundY(TextboxManager.FirstLineY + extraYOffset, bgHeight, posOffset.Y);
            //绘制
            DrawBackground(bgX, bgY, bgWidth, bgHeight, padding, textboxSettings.BackgroundColor * lerpValue, textboxSettings.BackgroundEdgeColor * TextboxManager.EdgeValue, posOffset);
            //实际绘制文本
            float curY = bgY + 2;
            float curX = bgX + 1;
            foreach (var value in element)
            {
                Vector2 textPos = new Vector2(curX, curY);
                DrawTextWithOutlineCustom(value, textPos, posOffset, lerpValue);
                curY += value.Size(font).Y + value.SpacingAfter;
            }
        }
        /// <summary>
        /// 在tootlip的右侧绘制文本框
        /// </summary>
        /// <param name="line">指定的Tooltip</param>
        /// <param name="cacheTooltip">临时缓存的Tooltip列表</param>
        /// <param name="textboxSettings">设置</param>
        /// <param name="extraYOffset"></param>
        /// <param name="maxWidth"></param>
        public static void DrawTextboxTooltipWithBackground(DrawableTooltipLine line, IReadOnlyList<TooltipLine> cacheTooltip,
            ref TextboxSettings textboxSettings, float extraYOffset = 0, float maxWidth = -1)
        {
            //line.Index如果不等于最后一行我们都不会绘制，以确保其绘制一次。
            //CacheTooltipList是事先在modifyTooltipline里缓存的列表。
            if (cacheTooltip is null || line.Index != cacheTooltip.Count - 1)
                return;
            DynamicSpriteFont font = line.Font ?? FontAssets.MouseText.Value;

            Vector2 scale = line.BaseScale;
            if (scale == Vector2.Zero)
                scale = Vector2.One;

            //计算整个 Tooltip 框的最大文本宽度，用于定位右侧内容
            if (maxWidth < 0)
            {
                maxWidth = 0;
                foreach (var t in cacheTooltip)
                {
                    Vector2 size = ChatManager.GetStringSize(font, t.Text, line.BaseScale);
                    if (size.X > maxWidth)
                        maxWidth = size.X;
                }
            }
            //如果设置里有Title（标题），这里才会处理标题内容
            if (textboxSettings.HasTitle)
            {
                Vector2 titleScale = scale * textboxSettings.TitleTextSize * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize;
                float lerpValue = TextboxManager.LerpValue;
                float edgeValue = TextboxManager.EdgeValue;
                Vector2 posOffset = Vector2.Lerp(Vector2.UnitY * -50f, Vector2.Zero, lerpValue) + Vector2.UnitY * (float)Math.Sin(Main.timeForVisualEffects / 60f) * 10f;

                //标题文本
                string titleText = "「" + textboxSettings.TitleText + "」";
                //实际描述文本的大小
                Vector2 mainTextSize = ChatManager.GetStringSize(font, textboxSettings.MainText, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
                //标题文本的大小
                Vector2 titleTextSize = ChatManager.GetStringSize(font, titleText, titleScale);
                //spacing用于和原本的文本框之间的间隔
                float spacing = 30f;
                //上面的数据全部算上，才是我们需要的标题文本起始绘制的X坐标
                float titleTextDrawX = line.X + maxWidth + spacing;
                //Y坐标则为原文本框的第a一段文本Y坐标，即物品名开始的Y坐标
                float titleTextDrawY = TextboxManager.FirstLineY + textboxSettings.MultboxSpacing + extraYOffset;


                //算一下实际描述文本大小与标题文本大小，哪个宽度更大
                float minSizeX = Math.Max(mainTextSize.X, titleTextSize.X);
                //这里进行边界检查
                //默认绘制的位置为文本框右侧，如果文本框大小本身超出了屏幕宽度，重设定其标题的X坐标
                if (titleTextDrawX + minSizeX > Main.screenWidth)
                {
                    titleTextDrawX = line.X - minSizeX - spacing;
                    //如果左侧仍然突出，则设置为0
                    if (titleTextDrawX < 0)
                        titleTextDrawX = 0;
                }

                //一般情况下没什么必要，但这里只是确保一下不会出问题
                float maxSizeY = titleTextSize.Y + mainTextSize.Y;
                //上或者下的边界检查
                float edgeCheckValue = titleTextDrawY + maxSizeY + posOffset.Y;
                if (edgeCheckValue > Main.screenHeight)
                {
                    float offset = edgeCheckValue - Main.screenHeight;
                    titleTextDrawY -= offset;
                    if (titleTextDrawY < 0)
                        titleTextDrawY = 0;
                }
                //实际标题文本的坐标
                Vector2 titlePos = new Vector2(titleTextDrawX, titleTextDrawY);
                //实际描述文本的坐标
                Vector2 mainTextPos = new Vector2(titleTextDrawX, titleTextDrawY + titleTextSize.Y + 5);
                SpriteBatch sb = Main.spriteBatch;

                //一个封装的，背景框绘制方案。
                DrawTextboxBackground(titlePos, titleTextSize, mainTextPos, mainTextSize, 8, titlePos, textboxSettings.BackgroundColor * lerpValue, posOffset, textboxSettings.BackgroundEdgeColor.Value * lerpValue * edgeValue);
                //最后，我们再画需要的文本内容。
                for (int i = 0; i < 16; i++)
                    ChatManager.DrawColorCodedString(sb, font, titleText, titlePos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f + posOffset, textboxSettings.TitleEdgeColor * lerpValue, 0, Vector2.Zero, titleScale);
                ChatManager.DrawColorCodedString(sb, font, titleText, titlePos + posOffset, textboxSettings.TitleTextColor * lerpValue, 0, Vector2.Zero, titleScale);
                for (int i = 0; i < 16; i++)
                    ChatManager.DrawColorCodedString(sb, font, textboxSettings.MainText, mainTextPos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f + posOffset, textboxSettings.TextEdgeColor * lerpValue, 0, Vector2.Zero, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
                ChatManager.DrawColorCodedString(sb, font, textboxSettings.MainText, mainTextPos + posOffset, textboxSettings.TextColor * lerpValue, 0, Vector2.Zero, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
            }
            else
            {
                //Vector2 mainTextSize = ChatManager.GetStringSize(font, textboxSettings.MainText, scale);
                Vector2 mainTextSize = ChatManager.GetStringSize(font, textboxSettings.MainText, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
                float spacing = 30f;
                float mainTextDrawX = line.X + maxWidth + spacing;
                float mainTextDrawY = TextboxManager.FirstLineY + extraYOffset;

                float lerpValue = TextboxManager.LerpValue;
                float edgeValue = TextboxManager.EdgeValue;
                //这个用于把textbox漂浮。
                Vector2 posOffset = Vector2.Lerp(Vector2.UnitY * -50f, Vector2.Zero, lerpValue) + Vector2.UnitY * (float)Math.Sin(Main.timeForVisualEffects / 60f) * 10f;
                //边界检查1
                if (mainTextDrawX + mainTextSize.X > Main.screenWidth)
                {
                    mainTextDrawX = line.X - mainTextSize.X - spacing;
                    if (mainTextDrawX < 0)
                        mainTextDrawX = 0;
                }
                float maxHeight = mainTextSize.Y + mainTextDrawY + posOffset.Y;
                if (maxHeight > Main.screenHeight)
                {
                    float offset = maxHeight - Main.screenHeight;
                    mainTextDrawY -= offset;
                    if (mainTextDrawY < 0)
                        mainTextDrawY = 0;
                }
                Vector2 mainTextPos = new Vector2(mainTextDrawX, mainTextDrawY);
                SpriteBatch sb = Main.spriteBatch;
                DrawTextboxBackground(mainTextPos.X, mainTextPos.Y, mainTextSize.X, mainTextSize.Y, 8, mainTextPos, textboxSettings.BackgroundColor * lerpValue, posOffset, textboxSettings.BackgroundEdgeColor * lerpValue);
                //最后，我们再画需要的文本内容。
                for (int i = 0; i < 16; i++)
                    ChatManager.DrawColorCodedString(sb, font, textboxSettings.MainText, mainTextPos + (TwoPi / 16f * i).ToRotationVector2() * 1.2f + posOffset, textboxSettings.TextEdgeColor * lerpValue, 0, Vector2.Zero, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
                ChatManager.DrawColorCodedString(sb, font, textboxSettings.MainText, mainTextPos + posOffset, textboxSettings.TextColor * lerpValue, 0, Vector2.Zero, scale * textboxSettings.BoxSize * KivotosClientConfig.Instance.TextboxSize);
            }
        }
        public static void DrawTextboxBackground(float beginPosX, float beginPosY, float width, float height, int padding, Vector2 drawPos, Color color, Vector2? posOffset = null, Color? edgeColor = null)
        {
            int minX = (int)(beginPosX - padding);
            int minY = (int)(beginPosY - padding);
            int maxX = (int)(beginPosX + width + padding);
            int maxY = (int)(beginPosY + height + padding);
            //设定这个矩形大小
            Rectangle rec = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            Texture2D background = KivotosTextureAssets.Texture_WhiteCubeBig.Value;
            SpriteBatch sb = Main.spriteBatch;

            Vector2 offset = posOffset ?? Vector2.Zero;
            Rectangle floatingInnerRec = new Rectangle(rec.X + (int)offset.X, rec.Y + (int)offset.Y, rec.Width, rec.Height);

            //内矩形
            int bw = 2;
            Vector2 recPos = drawPos + offset - new Vector2(8) - new Vector2(2, 0);
            //绘制背景，这个背景是一个超级巨大的方块，由于已经超出屏幕，可以直接使用rec的形式随意切割来实现我们需要的效果。
            sb.Draw(background, recPos + new Vector2(bw), floatingInnerRec, color, 0, Vector2.Zero, 1, 0, 0);
            //处理描边
            if (!edgeColor.HasValue)
                return;
            Color borderColor = edgeColor.Value;
            Rectangle outerRec = rec;
            outerRec.Inflate(bw, bw);
            Rectangle floatingOuterRec = new Rectangle(outerRec.X + (int)offset.X, outerRec.Y + (int)offset.Y, outerRec.Width, outerRec.Height);
            Rectangle upRec = new Rectangle(floatingOuterRec.X, floatingOuterRec.Y, floatingOuterRec.Width, bw);
            //上边
            sb.Draw(background, recPos, upRec, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //下边
            sb.Draw(background, recPos + new Vector2(0, floatingOuterRec.Height - bw), upRec, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //左边
            Rectangle leftRec = new Rectangle(floatingOuterRec.X, floatingOuterRec.Y + bw, bw, floatingOuterRec.Height - 2 * bw);
            sb.Draw(background, recPos + new Vector2(0, bw), leftRec, borderColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            //这个好像是右边
            sb.Draw(background, recPos + new Vector2(floatingOuterRec.Width - bw, bw), leftRec, borderColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
        public static void DrawTextboxBackground(Vector2 textPos, Vector2 textSize, Vector2 altTextPos, Vector2 altTextSize, int padding, Vector2 drawPos, Color color, Vector2? posOffset = null, Color? edgeColor = null)
        {
            // 基础矩形（包含内边距）
            int minX = (int)(Math.Min(textPos.X, altTextPos.X) - padding);
            int minY = (int)(Math.Min(textPos.Y, altTextPos.Y) - padding);
            int maxX = (int)(Math.Max(textPos.X + textSize.X, altTextPos.X + altTextSize.X) + padding);
            int maxY = (int)(Math.Max(textPos.Y + textSize.Y, altTextPos.Y + altTextSize.Y) + padding);

            //设定这个矩形大小
            Rectangle rec = new Rectangle(minX, minY, maxX - minX, maxY - minY);
            Texture2D background = KivotosTextureAssets.Texture_WhiteCubeBig.Value;
            SpriteBatch sb = Main.spriteBatch;
            Vector2 offset = posOffset ?? Vector2.Zero;
            Rectangle floatingInnerRec = new Rectangle(rec.X + (int)offset.X, rec.Y + (int)offset.Y, rec.Width, rec.Height);

            //内矩形
            int bw = 2;
            Vector2 recPos = drawPos + offset - new Vector2(8) - new Vector2(2, 0);
            //绘制背景，这个背景是一个超级巨大的方块，由于已经超出屏幕，可以直接使用rec的形式随意切割来实现我们需要的效果。
            sb.Draw(background, recPos + new Vector2(bw), floatingInnerRec, color, 0, Vector2.Zero, 1, 0, 0);
            if (!edgeColor.HasValue)
                return;
            Color borderColor = edgeColor.Value;
            Rectangle outerRec = rec;
            outerRec.Inflate(bw, bw);
            Rectangle floatingOuterRec = new Rectangle(outerRec.X + (int)offset.X, outerRec.Y + (int)offset.Y, outerRec.Width, outerRec.Height);
            Rectangle upRec = new Rectangle(floatingOuterRec.X, floatingOuterRec.Y, floatingOuterRec.Width, bw);
            //上边
            sb.Draw(background, recPos, upRec, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //下边
            sb.Draw(background, recPos + new Vector2(0, floatingOuterRec.Height - bw), upRec, borderColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            //左边
            Rectangle leftRec = new Rectangle(floatingOuterRec.X, floatingOuterRec.Y + bw, bw, floatingOuterRec.Height - 2 * bw);
            sb.Draw(background, recPos + new Vector2(0, bw), leftRec, borderColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            //这个好像是右边
            sb.Draw(background, recPos + new Vector2(floatingOuterRec.Width - bw, bw), leftRec, borderColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }

        public static void DrawMultipleTextboxes(DrawableTooltipLine line, IReadOnlyList<TooltipLine> cacheTooltip, List<TextboxSettings> settingsList, float verticalSpacing = 10f)
        {
            if (cacheTooltip is null || line.Index != cacheTooltip.Count - 1)
                return;

            DynamicSpriteFont font = line.Font ?? FontAssets.MouseText.Value;
            Vector2 scale = line.BaseScale;
            if (scale == Vector2.Zero)
                scale = Vector2.One;

            // 1. 计算 maxWidth（只需一次）
            float maxWidth = 0;
            foreach (var t in cacheTooltip)
            {
                Vector2 size = ChatManager.GetStringSize(font, t.Text, scale);
                if (size.X > maxWidth)
                    maxWidth = size.X;
            }
            // 2. 计算所有文本框的内容最大宽度（用于统一背景）
            float maxContentWidth = 0;
            foreach (var set in settingsList)
            {
                Vector2 textSizeScale = scale * set.BoxSize * KivotosClientConfig.Instance.TextboxSize;
                Vector2 mainSize = ChatManager.GetStringSize(font, set.MainText, textSizeScale);
                float w = mainSize.X;
                if (set.HasTitle)
                {
                    Vector2 titleSize = ChatManager.GetStringSize(font, "「" + set.TitleText + "」", textSizeScale * set.TitleTextSize);
                    w = Math.Max(w, titleSize.X);
                }
                if (w > maxContentWidth) maxContentWidth = w;
            }
            // 2. 预计算每个块的高度和最大宽度（用于水平边界）
            var blockHeights = new List<float>();
            float totalHeight = 0;
            foreach (var set in settingsList)
            {
                Vector2 textSizeScale = scale * set.BoxSize * KivotosClientConfig.Instance.TextboxSize;
                Vector2 titleSize = set.HasTitle ? ChatManager.GetStringSize(font, "「" + set.TitleText + "」", textSizeScale * set.TitleTextSize) : Vector2.Zero;
                Vector2 mainSize = ChatManager.GetStringSize(font, set.MainText, textSizeScale);
                float blockHeight = titleSize.Y + (set.HasTitle ? 5 : 0) + mainSize.Y;
                blockHeights.Add(blockHeight);
                totalHeight += blockHeight + verticalSpacing;
            }
            totalHeight -= verticalSpacing; // 移除最后一个多余间距

            // 3. 垂直边界调整
            float startY = TextboxManager.FirstLineY;
            float offsetY = 0;
            if (startY + totalHeight > Main.screenHeight - 10)
            {
                offsetY = startY + totalHeight - (Main.screenHeight - 10);
                if (offsetY > startY) offsetY = startY;
            }
            if (startY - offsetY < 10) offsetY = startY - 10;

            float curY = startY - offsetY;

            // 4. 依次绘制每个文本框
            for (int i = 0; i < settingsList.Count; i++)
            {
                var set = settingsList[i];
                float extraY = curY - TextboxManager.FirstLineY;
                // 调用原有的单个绘制方法（传入 maxWidth 和 extraY）
                DrawTextboxTooltipWithBackgroundMultline(line, cacheTooltip, ref set, extraY, maxWidth, maxContentWidth);
                curY += blockHeights[i] + verticalSpacing;
            }
        }
    }
}
