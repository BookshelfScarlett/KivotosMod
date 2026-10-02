namespace KivotosMod.Globals.Database.Paths
{
    /// <summary>
    /// 本类用于存储本模组的本地化路径
    /// </summary>
    public static class LocalizationsDatabase
    {
        public static string ItemLocalization => "Items";
        public static string ProjLocalization => "Projs";
        public static class Items
        {
            public static string StudentWeapons => $"{ItemLocalization}.Ranged.StudentWeapons";
            public static string RangedWeapon => $"{ItemLocalization}.Ranged";
            public static string MeleeWeapon => $"{ItemLocalization}.Melee";
        }
        public static class Projs
        {
            public static string StudentWeapons => $"{ProjLocalization}.Ranged.StudentWeapons";
            public static string MeleeProj => $"{ProjLocalization}.Melee";
            public static string RangedProj => $"{ProjLocalization}.Ranged";
            public static string TypelessProj => $"{ProjLocalization}.Typeless";
        }
    }
}
