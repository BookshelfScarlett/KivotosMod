using KivotosMod.Globals.Instances.Items;
using KivotosMod.Globals.Instances.NPCs;
using KivotosMod.Globals.Instances.Projs;
using KivotosMod.Globals.Players;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace KivotosMod.Globals.Methods
{
    public static partial class KivotosMethods
    {
        public static KivotosPlayer Kivotos(this Player player) => player.GetModPlayer<KivotosPlayer>();
        public static KivotosGlobalProjs Kivotos(this Projectile proj) => proj.GetGlobalProjectile<KivotosGlobalProjs>();
        public static KivotosGlobalItems Kivotos(this Item item) => item.GetGlobalItem<KivotosGlobalItems>();
        public static KivotosGlobalNPCs Kivotos(this NPC npc) => npc.GetGlobalNPC<KivotosGlobalNPCs>();
        public static void SetUpNoUseGraphicItem(this Item item, bool channel = false, bool autoReuse = true)
        {
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = channel;
            item.autoReuse = autoReuse;
        }
    }
}
