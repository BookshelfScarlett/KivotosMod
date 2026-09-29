using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Content.Raritys.Sparkles;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Raritys.DrawMethods
{
    public static class BlackWhite
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            //最后更新他。
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.Ivory, Color.Black, Color.Ivory, 1);
        }
        public static void DrawItemNameParticle(DrawableTooltipLine tooltipLine, ref List<RaritySparkle> particleList)
        {
            //在这里手动创建新的粒子，然后我们再将其添加进需要的表单内
            if (Main.rand.NextBool(10))
            {
                float scale = Main.rand.NextFloat(0.30f * 0.5f, 0.30f) * 1.2f;
                int lifetime = 160;
                Vector2 position = RarityDrawHelper.GetParticlePosition(tooltipLine);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(0.25f, 0.55f) * (1 * -0.75f);
                RarityShinyOrb rarityShinyOrb = new(position, velocity, Color.Lerp(Color.Black, Color.White, Main.rand.NextFloat()), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
        }
    }
}
