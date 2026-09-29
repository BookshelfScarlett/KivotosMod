using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KivotosMod.Globals.Database.Paths
{
    public static class KivotosContent
    {
        public const string Items = "KivotosMod/Assets/Texture/Items/";
        public const string Projs = "KivotosMod/Assets/Texture/Projs/";
        public static string StudentWeapons => $"{Items}Weapons/StudentWeapons/";
        public static string GetAsset(string prefix, string fileName)
        {
            return $"{prefix}{fileName}";
        }
    }
}
