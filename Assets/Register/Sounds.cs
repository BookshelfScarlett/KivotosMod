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
        /// <summary>
        /// ScarletShelf 2026-10-2:
        /// 我自己转成.ogg不知道为什么没有声音了
        /// 你们谁能转的话就转一下
        /// </summary>
        public static SoundStyle YuzuShot => new SoundStyle($"{SoundsPath}YuzuShot", 3);
        public static SoundStyle SharpBoom => new SoundStyle($"{SoundsPath}SharpBoom");
        public static SoundStyle SharpBoomHeavy => new SoundStyle($"{SoundsPath}SharpBoomHeavy");
    }
}
