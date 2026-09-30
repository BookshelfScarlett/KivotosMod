using Terraria.Audio;

namespace KivotosMod.Assets.Register
{
    public static class KivotosSoundsAssets
    {
        public static string SoundsPath => "KivotosMod/Assets/Sounds/";
        public static SoundStyle Shotgun_Mastiff => new SoundStyle($"{SoundsPath}Mastiff", 3);
        public static SoundStyle Shotgun_HarukaShotgun => new SoundStyle($"{SoundsPath}HarukaShotgun");
        public static SoundStyle Pistol => new SoundStyle($"{SoundsPath}Pistol");
        public static SoundStyle FlameThrower_Release => new SoundStyle($"{SoundsPath}FireRelease");
    }
}
