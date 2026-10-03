using KivotosMod.Content.Items.Vanity.Nozomi;
using KivotosMod.Cores.Halos;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Players.Vanitys
{
    public partial class KivotosVanitysPlayer : ModPlayer
    {
        public string VanityName = "None";
        public override void ResetEffects()
        {
            VanityName = "None";
        }
        public override void UpdateDead()
        {
            VanityName = "None";
        }
        public override void PostUpdateEquips()
        {
            if (Player.HasCertainHalo(VanityName))
                return;
            new NozomiHalo(Player.whoAmI).Spawn();

            base.PostUpdateEquips();
        }
    }
}
