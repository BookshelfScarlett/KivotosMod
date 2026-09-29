using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Content.Raritys.Sparkles;
using KivotosMod.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Raritys.DrawMethods
{
    public static class HoshinoPink 
    {
        public static void DrawItemName(DrawableTooltipLine line)
        {
            RarityDrawHelper.DrawCustomTooltipLine(line, Color.LightPink, Color.Violet, Color.White, 1);
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
                RarityShinyOrb rarityShinyOrb = new(position, velocity, KivotosMethods.RandLerpColor(Color.HotPink, Color.LightPink), lifetime, scale);
                particleList.Add(rarityShinyOrb);
            }
        }
    }
}
