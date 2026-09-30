using KivotosMod.Globals.Database.Enums;
using System.Collections.Generic;

namespace KivotosMod.Globals.Database.Lists
{
    public partial class KivotosLists
    {
        /// <summary>
        /// 主要用于存储并引用特殊稀有度绘制
        /// </summary>
        public static Dictionary<int, KivotosRarityType> ShinyRarityItemDictionary = [];
        public static Dictionary<int, string> StudentWeaponDictionary = [];
    }
}
