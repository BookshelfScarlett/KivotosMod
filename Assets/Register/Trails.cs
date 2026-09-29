using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public partial class KivotosTextureAssets : ModSystem
    {
        public string TrailPath => "KivotosMod/Assets/Texture/Trails/";
        public static Tex2DWithPath Trail_BloomDualLine { get; private set; }
        public static Tex2DWithPath Trail_ManaStreak { get; set; }
        public static Tex2DWithPath Trail_ManaStreakTiny { get; set; }
        public static Tex2DWithPath Trail_RvSlash { get; set; }
        public static Tex2DWithPath Trail_VShapeWithTail { get; set; }
        public static Tex2DWithPath Trail_FadedStreak { get; set; }
        public static Tex2DWithPath Trail_MegaBeam { get; set; }
        public static Tex2DWithPath Trail_ManaMegaBeam { get; set; }
        public static Tex2DWithPath Trail_Lightning0 { get; set; }
        public static Tex2DWithPath Trail_Lightning1 { get; set; }
        public static Tex2DWithPath Trail_Lightning2 { get; set; }
        public static Tex2DWithPath Trail_Lightning3 { get; set; }
        public static Tex2DWithPath Trail_Lightning4 { get; set; }
        public void LoadTrails()
        {
            Trail_BloomDualLine = new Tex2DWithPath(TrailPath + "BloomDualLine");
            Trail_ManaStreak = new Tex2DWithPath(TrailPath + "ManaStreak");
            Trail_ManaStreakTiny = new Tex2DWithPath(TrailPath + "ManaStreakTiny");
            Trail_RvSlash = new Tex2DWithPath(TrailPath + "RvSlash");
            Trail_VShapeWithTail = new Tex2DWithPath(TrailPath + "VShapeWithTail");
            Trail_FadedStreak = new Tex2DWithPath(TrailPath + "FadedStreak");
            Trail_MegaBeam = new Tex2DWithPath(TrailPath + "MegaBeam");
            Trail_ManaMegaBeam = new Tex2DWithPath(TrailPath + "ManaMegaBeam");
            Trail_Lightning0 = new Tex2DWithPath(TrailPath + "Lightning0");
            Trail_Lightning1 = new Tex2DWithPath(TrailPath + "Lightning1");
            Trail_Lightning2 = new Tex2DWithPath(TrailPath + "Lightning2");
            Trail_Lightning3 = new Tex2DWithPath(TrailPath + "Lightning3");
            Trail_Lightning4 = new Tex2DWithPath(TrailPath + "Lightning4");
        }
        public void UnloadTrails()
        {
            Trail_BloomDualLine = null;
            Trail_ManaStreak = null;
            Trail_ManaStreakTiny = null;
            Trail_RvSlash = null;
            Trail_VShapeWithTail = null;
            Trail_FadedStreak = null;
            Trail_MegaBeam = null;
            Trail_ManaMegaBeam = null;
            Trail_Lightning0 = null;
            Trail_Lightning1 = null;
            Trail_Lightning2 = null;
            Trail_Lightning3 = null;
            Trail_Lightning4 = null;
        }
    }
}
