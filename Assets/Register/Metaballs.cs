using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public partial class KivotosTextureAssets : ModSystem
    {
        public string MetaballPath => "KivotosMod/Assets/Texture/Metaballs/";
        public static Tex2DWithPath Metaball_Bloody { get; set; }
        public static Tex2DWithPath Metaball_ShadowNebula { get; set; }
        public void LoadMetaballs()
        {
            Metaball_Bloody = new Tex2DWithPath(MetaballPath + "Bloody");
            Metaball_ShadowNebula = new Tex2DWithPath(MetaballPath + "ShadowNebula");
        }
        public void UnloadMetaballs()
        {
            Metaball_Bloody = null;
            Metaball_ShadowNebula = null;
        }
    }
}
