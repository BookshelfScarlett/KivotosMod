using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Local tablet companion portrait selection. This is intentionally separate from
/// Formation/player-appearance selection: switching the HOME portrait must never
/// alter the player's vanity/character state.
/// </summary>
public sealed class TabletCompanionPlayer : ModPlayer
{
    public TabletStudentId Companion { get; private set; } = TabletStudentId.Arona;

    public void SetCompanion(TabletStudentId id)
    {
        Companion = id;
    }

    public void Cycle(int direction)
    {
        int count = TabletStudentCatalog.CompanionStudents.Count;
        if (count <= 0)
            return;

        int current = TabletStudentCatalog.CompanionIndexOf(Companion);
        int next = (current + (direction < 0 ? -1 : 1) + count) % count;
        Companion = TabletStudentCatalog.CompanionStudents[next].Id;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(Companion)] = (byte)Companion;
    }

    public override void LoadData(TagCompound tag)
    {
        byte raw = tag.ContainsKey(nameof(Companion)) ? tag.GetByte(nameof(Companion)) : (byte)TabletStudentId.Arona;
        TabletStudentId decoded = raw <= (byte)TabletStudentId.Plana ? (TabletStudentId)raw : TabletStudentId.Arona;
        Companion = TabletStudentCatalog.CompanionIndexOf(decoded) == 0 && decoded != TabletStudentId.Arona
            ? TabletStudentId.Arona
            : decoded;
    }
}
