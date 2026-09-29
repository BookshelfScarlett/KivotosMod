using Terraria.Audio;

namespace KivotosMod.Assets.Register
{
    public static class KivotosSoundsAssets
    {
        public static string SoundsPath => "KivotosMod/Assets/Sounds/";
        public static SoundStyle Shotgun_Mastiff => new SoundStyle($"{SoundsPath}Mastiff", 3);
    }
}
