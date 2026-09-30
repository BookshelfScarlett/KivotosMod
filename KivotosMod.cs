global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using static Microsoft.Xna.Framework.MathHelper;
global using static Terraria.ModLoader.ModContent;
using Terraria.ModLoader;


namespace KivotosMod
{
    // Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
    public class KivotosMod : Mod
    {
        public static KivotosMod Instance;
        public override void Load()
        {
            Instance = this;
        }
        public override void Unload()
        {
            Instance = null;
        }
    }
}
