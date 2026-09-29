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
        public string TexturePath => "KivotosMod/Assets/Texture/General/";
        public static Tex2DWithPath Texture_RarityGlow { get; private set; }
        public static Tex2DWithPath InvisAsset { get; private set; }
        public override void Load()
        {
            Texture_RarityGlow = new Tex2DWithPath(TexturePath + "RarityGlow");
            InvisAsset = new Tex2DWithPath(TexturePath + "InvisAsset");
            LoadParticles();
        }
        public override void Unload()
        {
            Texture_RarityGlow = null;
            InvisAsset = null;
            UnloadParticles();
        }
    }
}
