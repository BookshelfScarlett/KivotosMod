using System.ComponentModel;
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
        
        [BackgroundColor(211, 211, 211, 192)]
        [DefaultValue(true)]
        public bool SpecialRarity { get; set; }

        [BackgroundColor(211, 211, 211, 192)]
        [Range(50, 30000)]
        [Increment(1)]
        [DefaultValue(10000)]
        public int MaxParticleCounts { get; set; }
        [BackgroundColor(211, 211, 211, 192)]
        [Range(0, 10f)]
        [DefaultValue(1f)]
        public float ScreenShakeStrength { get; set; }

        [BackgroundColor(211, 211, 211, 192)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float ScreenDarkStrength { get; set; }
    }
}
