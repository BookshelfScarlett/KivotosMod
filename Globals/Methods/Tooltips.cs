using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, string LineName = "KivotosMod", int index = -1)
        {
            string text = textPath.ToLangValue();
            Mod tooltipMod = KivotosMod.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName + "Name", text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static void CreateTooltip(this List<TooltipLine> tooltips, string textPath, Color? color = null, string LineName = "KivotosMod", int index = -1, params object[] args)
        {
            string text = textPath.ToLangValue().ToFormatValue(args);
            Mod tooltipMod = KivotosMod.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName + "Name", text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static void CreateTooltipDirect(this List<TooltipLine> tooltips, string textPath, Color? color = null, string LineName = "KivotosMod", int index = -1)
        {
            string text = textPath;
            Mod tooltipMod = KivotosMod.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName + "Name", text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }

        public static void CreateTooltipDirect(this List<TooltipLine> tooltips, string textValue, Color? color = null, string LineName = "KivotosMod", int index = -1, params object[] args)
        {
            string text = textValue.ToFormatValue(args);
            Mod tooltipMod = KivotosMod.Instance;
            Color overrideColor = color ?? Color.White;
            var newLine = new TooltipLine(tooltipMod, LineName + "Name", text)
            {
                OverrideColor = overrideColor
            };
            int count = tooltips.Count;
            if (count is 0)
                tooltips.Add(newLine);
            else
            {
                if (index != -1)
                    count = index;
                tooltips.Insert(count, newLine);
            }
        }
        public static string ToLangValue(this string textPath) => Language.GetTextValue(textPath);

        public static string ToFormatValue(this string baseTextValue, params object[] args)
        {
            try
            {
                return string.Format(baseTextValue, args);
            }
            catch
            {
                return baseTextValue + "格式化出错";
            }
        }
    }
}
