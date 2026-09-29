using Terraria.Localization;
using Terraria.ModLoader.Config;

namespace KivotosMod.Globals.Configs
{
    public class KivotosClientConfig : ModConfig
    {
        public static KivotosClientConfig Instance;
        public override void OnLoaded()
        {
            Instance = this;
        }
        public override ConfigScope Mode => ConfigScope.ClientSide;
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message) => false;
    }
}
