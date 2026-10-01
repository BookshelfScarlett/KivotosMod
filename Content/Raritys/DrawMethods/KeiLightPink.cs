using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Content.Raritys.Sparkles;
using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;


namespace KivotosMod.Content.Raritys.DrawMethods
{
    public static class KeiLightPink
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.Green, Color.Lime.ToAddColor(), Color.DarkGreen, 1.1f);
        }
        public static void DrawFlavorNameRarity(DrawableTooltipLine drawableTooltipLine, ref List<RaritySparkle> flavorSparkles)
        {
            RarityDrawHelper.DrawCustomTooltipLine(drawableTooltipLine, Color.DarkGreen, Color.LimeGreen.ToAddColor());
        }
        public static void DrawFlavorNameParticle(ref List<RaritySparkle> particleList, DrawableTooltipLine tooltipLine)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)(textSize.X), (int)(textSize.Y * .78f)));
                Vector2 velocity = Vector2.UnitX * Main.rand.NextFloat(0.25f, 0.35f) * 1.1f;
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, KivotosMethods.RandLerpColor(Color.Lime, Color.LimeGreen).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)(textSize.X), (int)(textSize.Y * 0.78f)));
                velocity = Vector2.UnitX * Main.rand.NextFloat(0.25f, 0.35f);
                scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                RarityShinyOrb rarityShinyOrb3 = new RarityShinyOrb(position, velocity, KivotosMethods.RandLerpColor(Color.Lime, Color.LimeGreen).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb4 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb3);
                particleList.Add(rarityShinyOrb4);
            }
            //最后更新他。
        }

        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            Vector2 textSize = tooltipLine.Font.MeasureString(tooltipLine.Text);
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                Vector2 velocity = Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, KivotosMethods.RandLerpColor(Color.Lime, Color.LimeGreen).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb);
            }
            //最后更新他。
        }

    }
}
