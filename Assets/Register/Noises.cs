using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public partial class KivotosTextureAssets : ModSystem
    {
        public string NoisePath => "KivotosMod/Assets/Texture/Noises/";

        public static Tex2DWithPath Noise_Aura { get; set; }
        public static Tex2DWithPath Noise_EmptyAura { get; set; }
        public static Tex2DWithPath Noise_HeavyAura { get; set; }
        public static Tex2DWithPath Noise_Misc { get; set; }
        public static Tex2DWithPath Noise_Misc2 { get; set; }
        public static Tex2DWithPath Noise_Smoke { get; set; }
        public static Tex2DWithPath Noise_WaterFlow { get; set; }

        public void LoadNoises()
        {
            Noise_Aura = new Tex2DWithPath(NoisePath + "Aura");
            Noise_EmptyAura = new Tex2DWithPath(NoisePath + "EmptyAura");
            Noise_HeavyAura = new Tex2DWithPath(NoisePath + "HeavyAura");
            Noise_Misc = new Tex2DWithPath(NoisePath + "Misc");
            Noise_Misc2 = new Tex2DWithPath(NoisePath + "Misc2");
            Noise_Smoke = new Tex2DWithPath(NoisePath + "Smoke");
            Noise_WaterFlow = new Tex2DWithPath(NoisePath + "WaterFlow");
        }

        public void UnloadNoises()
        {
            Noise_Aura = null;
            Noise_EmptyAura = null;
            Noise_HeavyAura = null;
            Noise_Misc = null;
            Noise_Misc2 = null;
            Noise_Smoke = null;
            Noise_WaterFlow = null;
        }
    }
}
