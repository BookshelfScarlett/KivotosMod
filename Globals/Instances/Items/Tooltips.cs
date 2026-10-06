using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Globals.Database.Lists;
using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Instances.Items
{
    public partial class KivotosGlobalItems : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (KivotosLists.StudentWeaponDictionary.TryGetValue(item.type, out string value))
            {
                // Modify tooltips for student weapons
                int firstLine = tooltips.FindIndex(t => t.Name.Contains("Tooltip") && t.Mod == "Terraria");
                int index;
                if (firstLine < 0)
                {
                    // Some items have no vanilla Tooltip* lines. Insert after ItemName instead
                    // of indexing tooltips[-1].
                    int itemNameLine = tooltips.FindIndex(t => t.Name == "ItemName" && t.Mod == "Terraria");
                    index = itemNameLine >= 0 ? itemNameLine + 1 : tooltips.Count;
                }
                else
                {
                    index = firstLine;
                    for (int i = firstLine; i < tooltips.Count; i++)
                    {
                        if (tooltips[i].Name.Contains("Tooltip") && tooltips[i].Mod == "Terraria")
                            index++;
                        else
                            break;
                    }
                }
                string name = Mod.GetLocalizationKey("Database.StudentNames." + value).ToLangValue();
                string ownerPrefix = Mod.GetLocalizationKey("Database.OwnerPrefix").ToLangValue();
                string tooltip = $"={ownerPrefix}·{name}=";
                KivotosMethods.CreateTooltipDirect(tooltips, tooltip, Color.White, "StudentWeapon", index);
            }
        }

        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            if (line.Name == "ItemName" && line.Mod == "Terraria")
            {
                if (KivotosLists.ShinyRarityItemDictionary.TryGetValue(item.type, out var rarityType))
                {
                    RarityDrawHelper.UpdateItemNameParticle(line, rarityType);
                    RarityDrawHelper.UpdateItemNameDraw(line, rarityType);
                }
            }
            if (line.Name == "StudentWeaponName" && line.Mod == Mod.Name)
            {
                if (KivotosLists.ShinyRarityItemDictionary.TryGetValue(item.type, out var value))
                {
                    //RarityDrawHelper.UpdateItemNameParticle(line, value);
                    RarityDrawHelper.UpdateItemNameDraw(line, value);
                }
            }
        }
    }
}
