using KivotosMod.Content.Items.Vanity.Yuuka;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Player-only Formation selection state.  No student NPC is registered or spawned.
/// The selected student survives closing the tablet and leaving the Formation page.
/// Only Yuuka currently has a complete player-format costume, so other selections keep
/// the player's ordinary visual equipment while remaining selected in the tablet UI.
/// </summary>
public sealed class TabletStudentAppearancePlayer : ModPlayer
{
    public TabletStudentId SelectedStudent { get; private set; } = TabletStudentId.Yuuka;
    public bool HasSelection { get; private set; }

    public bool IsYuukaAppearance => HasSelection && SelectedStudent == TabletStudentId.Yuuka;

    public void Select(TabletStudentId student)
    {
        SelectedStudent = student;
        HasSelection = true;
    }

    public void ClearSelection()
    {
        HasSelection = false;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(HasSelection)] = HasSelection;
        tag[nameof(SelectedStudent)] = (byte)SelectedStudent;
    }

    public override void LoadData(TagCompound tag)
    {
        HasSelection = tag.GetBool(nameof(HasSelection));
        byte raw = tag.ContainsKey(nameof(SelectedStudent)) ? tag.GetByte(nameof(SelectedStudent)) : (byte)TabletStudentId.Yuuka;
        TabletStudentId decoded = raw <= (byte)TabletStudentId.Plana ? (TabletStudentId)raw : TabletStudentId.Yuuka;

        // V11 briefly exposed tablet companions as Formation candidates. Do not preserve
        // those invalid avatar selections in V12; Unit Formation only owns the three
        // player-format student entries.
        if (decoded != TabletStudentId.Yuuka && decoded != TabletStudentId.Shiroko && decoded != TabletStudentId.Hoshino)
        {
            SelectedStudent = TabletStudentId.Yuuka;
            HasSelection = false;
        }
        else
        {
            SelectedStudent = decoded;
        }
    }

    public override void FrameEffects()
    {
        if (IsYuukaAppearance)
        {
            // Visual slot overrides only; nothing is inserted into armor/vanity inventory slots.
            Player.head = ModContent.GetInstance<YuukaHead>().Item.headSlot;
            Player.body = ModContent.GetInstance<YuukaBody>().Item.bodySlot;
            Player.legs = ModContent.GetInstance<YuukaBoots>().Item.legSlot;
        }

        // The backward-walk correction applies both to Formation-selected Yuuka and to a player
        // who manually equips the complete Yuuka vanity set.
        if (IsYuukaAppearance || YuukaVanitySet.FullSetVisible(Player))
            ApplyBackwardWalkLegFrames();
    }

    private void ApplyBackwardWalkLegFrames()
    {
        // Aiming a gun can keep player.direction pointed at the cursor while velocity goes the
        // opposite way. Vanilla continues advancing the forward walk sequence in that case,
        // which makes Yuuka look like she is moon-walking. Mirror only the walk-cycle phase.
        if (System.Math.Abs(Player.velocity.X) < 0.10f)
            return;

        bool movingOppositeFacing = (Player.velocity.X > 0f && Player.direction < 0)
                                  || (Player.velocity.X < 0f && Player.direction > 0);
        if (!movingOppositeFacing)
            return;

        // The standard 20-frame Terraria player leg sheet uses frames 6..16 for the walking
        // stride.  Reversing that phase keeps the same sprite direction while making the feet
        // step backward.  Frames outside the walk stride (idle/jump/sit/etc.) are untouched.
        int frameHeight = Player.legFrame.Height > 0 ? Player.legFrame.Height : 56;
        int frame = Player.legFrame.Y / frameHeight;
        if (frame < 6 || frame > 16)
            return;

        int reversedFrame = 22 - frame; // 6<->16, 7<->15, ... 11 stays centered.
        Player.legFrame.Y = reversedFrame * frameHeight;
    }
}
