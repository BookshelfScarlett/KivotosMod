using KivotosMod.Content.Raritys.DrawMethods;
using KivotosMod.Globals.Database.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace KivotosMod.Content.Raritys.Helper
{
    public static partial class RarityDrawHelper
    {
        public static void UpdateItemNameParticle(DrawableTooltipLine tooltipLine, KivotosRarityType type)
        {
            switch (type)
            {
                case KivotosRarityType.BlackWhite:
                    BlackWhite.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.HoshinoPink:
                    HoshinoPink.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.HinaViolet:
                    HinaViolet.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.MidoriGreen:
                    MidoriGreen.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.YuukaBlue:
                    YuukaBlue.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.MeguOrange:
                    MeguOrange.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                case KivotosRarityType.MutsukiBrown:
                    MutsukiBrown.DrawItemNameParticle(tooltipLine, ref RaritySparklesList);
                    break;
                default:
                    break;
            }
            if (RaritySparklesList.Count > 0)
            {
                UpdateTooltipParticles(tooltipLine, ref RaritySparklesList);
            }
        }


        public static void UpdateItemNameDraw(DrawableTooltipLine tooltipLine, KivotosRarityType type)
        {
            switch (type)
            {
                case KivotosRarityType.BlackWhite:
                    BlackWhite.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.HoshinoPink:
                    HoshinoPink.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.HinaViolet:
                    HinaViolet.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.MidoriGreen:
                    MidoriGreen.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.YuukaBlue:
                    YuukaBlue.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.MeguOrange:
                    MeguOrange.DrawItemName(tooltipLine);
                    break;
                case KivotosRarityType.MutsukiBrown:
                    MutsukiBrown.DrawItemName(tooltipLine);
                    break;
                default:
                    break;
            }
        }

    }
}
