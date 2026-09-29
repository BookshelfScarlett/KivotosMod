namespace KivotosMod.Globals.Database.Paths
{
    public static class LocalizationsDatabase
    {
        public static string ItemLocalization => "Items";
        public static string ProjLocalization => "Projs";
        public static class Items
        {
            public static string StudentWeapons => $"{ItemLocalization}.StudentWeapons";
        }
        public static class Projs
        {
            public static string StudentWeapons => $"{ProjLocalization}.StudentWeapons";
        }
    }
}
