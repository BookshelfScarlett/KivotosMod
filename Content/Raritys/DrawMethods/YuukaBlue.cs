using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Content.Raritys.Sparkles;
using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Raritys.DrawMethods
{

    public static class YuukaBlue
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.DeepSkyBlue, Color.SkyBlue.ToAddColor(), Color.DeepSkyBlue, 1.1f);
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
                RarityShinyOrb rarityShinyOrb = new RarityShinyOrb(position, velocity, KivotosMethods.RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue).ToAddColor(), lifetime, scale);
                RarityShinyOrb rarityShinyOrb2 = new RarityShinyOrb(position, velocity, Color.White.ToAddColor(), lifetime, scale * 0.5f);
                particleList.Add(rarityShinyOrb2);
                particleList.Add(rarityShinyOrb);
                position = Main.rand.NextVector2FromRectangle(new(-(int)(textSize.X * 0.5f), -(int)(textSize.Y * 0.5f), (int)textSize.X, (int)(textSize.Y * 0.35f)));
                position.Y += textSize.Y * 0.4f;
                velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.35f);
                RarityCube rarityCube = new RarityCube(position, velocity, KivotosMethods.RandLerpColor(Color.SkyBlue, Color.DeepSkyBlue).ToAddColor(), lifetime, scale);
                particleList.Add(rarityCube);
            }
            //最后更新他。
        }
    }
}
