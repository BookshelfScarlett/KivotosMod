using ReLogic.Content;
using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    /// <summary>
    /// 一个包含纹理和路径的类，用于在模组中管理贴图。
    /// <br>由于tMod的注册方式，这个管理方案可能会使贴图占用两份贴图内存</br>
    /// <br>但是对于tMod环境下，这一点占用其实无伤大雅</br>
    /// </summary>
    public class Tex2DWithPath
    {
        public Asset<Texture2D> Texture { get; }
        public string Path { get; }
        public Tex2DWithPath(Asset<Texture2D> texture, string path)
        {
            Path = path;
            Texture = texture;
        }
        public Tex2DWithPath(string path)
        {
            Path = path;
            Texture = Request<Texture2D>($"{Path}");
        }
        public Texture2D Value => Texture.Value;
    }
    public partial class KivotosTextureAssets : ModSystem
    {
        public override void Load()
        {
            LoadParticles();
            LoadMetaballs();
            LoadNoises();
            LoadTextures();
            LoadTrails();
        }

        public override void Unload()
        {
            UnloadParticles();
            UnloadMetaballs();
            UnloadNoises();
            UnloadTextures();
            UnloadTrails();
        }
        public string TexturePath => "KivotosMod/Assets/Texture/General/";
        public static Tex2DWithPath Texture_RarityGlow { get; private set; }
        public static Tex2DWithPath Texture_BloodStain { get; private set; }
        public static Tex2DWithPath Texture_BloomRing { get; private set; }
        public static Tex2DWithPath Texture_BloomShockwave { get; private set; }
        public static Tex2DWithPath Texture_Fireball { get; private set; }
        public static Tex2DWithPath Texture_FireballPixel { get; private set; }
        public static Tex2DWithPath Texture_Fog { get; private set; }
        public static Tex2DWithPath Texture_Smear { get; private set; }
        public static Tex2DWithPath Texture_SnowCloud { get; private set; }
        public static Tex2DWithPath Texture_Spirite { get; private set; }
        public static Tex2DWithPath Texture_SoftCircleEdge { get; private set; }
        public static Tex2DWithPath Texture_StandardGradient { get; private set; }
        public static Tex2DWithPath Texture_WhiteCircle { get; private set; }
        public static Tex2DWithPath Texture_WhiteCubeBig { get; private set; }
        public static Tex2DWithPath InvisAsset { get; private set; }

        public void LoadTextures()
        {
            Texture_RarityGlow = new Tex2DWithPath(TexturePath + "RarityGlow");
            Texture_BloodStain = new Tex2DWithPath(TexturePath + "BloodStain");
            Texture_BloomRing = new Tex2DWithPath(TexturePath + "BloomRing");
            Texture_BloomShockwave = new Tex2DWithPath(TexturePath + "BloomShockwave");
            Texture_Fireball = new Tex2DWithPath(TexturePath + "Fireball");
            Texture_FireballPixel = new Tex2DWithPath(TexturePath + "FireballPixel");
            Texture_Fog = new Tex2DWithPath(TexturePath + "Fog");
            Texture_Smear = new Tex2DWithPath(TexturePath + "Smear");
            Texture_SnowCloud = new Tex2DWithPath(TexturePath + "SnowCloud");
            Texture_Spirite = new Tex2DWithPath(TexturePath + "Spirite");
            Texture_SoftCircleEdge = new Tex2DWithPath(TexturePath + "SoftCircleEdge");
            Texture_StandardGradient = new Tex2DWithPath(TexturePath + "StandardGradient");
            Texture_WhiteCircle = new Tex2DWithPath(TexturePath + "WhiteCircle");
            Texture_WhiteCubeBig = new Tex2DWithPath(TexturePath + "WhiteCubeBig");
            InvisAsset = new Tex2DWithPath(TexturePath + "InvisAsset");

        }
        public void UnloadTextures()
        {
            Texture_RarityGlow = null;
            Texture_BloodStain = null;
            Texture_BloomRing = null;
            Texture_BloomShockwave = null;
            Texture_Fireball = null;
            Texture_FireballPixel = null;
            Texture_Fog = null;
            Texture_Smear = null;
            Texture_SnowCloud = null;
            Texture_Spirite = null;
            Texture_SoftCircleEdge = null;
            Texture_StandardGradient = null;
            Texture_WhiteCircle = null;
            Texture_WhiteCubeBig = null;
            InvisAsset = null;

        }
    }
}
