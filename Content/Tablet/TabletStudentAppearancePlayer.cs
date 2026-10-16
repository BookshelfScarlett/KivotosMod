using KivotosMod.Content.Items.Vanity.Hina;
using KivotosMod.Content.Items.Vanity.Noa;
using KivotosMod.Content.Items.Vanity.Yuuka;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Player-only Formation selection state.  No student NPC is registered or spawned.
/// The selected student survives closing the tablet and leaving the Formation page.
/// Yuuka, Hina and Noa provide a player-format costume. Other selections
/// keep the player's ordinary visual equipment while remaining selected in the tablet UI.
/// </summary>
public sealed class TabletStudentAppearancePlayer : ModPlayer
{
    public TabletStudentId SelectedStudent { get; private set; } = TabletStudentId.Yuuka;
    public bool HasSelection { get; private set; }

    public bool IsYuukaAppearance => HasSelection && SelectedStudent == TabletStudentId.Yuuka;
    public bool IsHinaAppearance => HasSelection && SelectedStudent == TabletStudentId.Hina;
    public bool IsNoaAppearance => HasSelection && SelectedStudent == TabletStudentId.Noa;

    // Elaina reference: use the standard ModPlayer rendering lifecycle. Keeping the
    // appearance active from BOTH FrameEffects and DrawEffects avoids renderer-specific
    // detours and helps with auto-pause, inventory display and player previews.
    // Do not install global LegacyPlayerRenderer hooks for costume selection.

    // ElainaModplayer's actual PreKill IL sets genGore=false and then returns to
    // the vanilla death routine. Its DrawEffects controls head/body/leg velocities
    // and death pose; it does not modify player.position or clear fragment positions.
    // Do not hook KillMe or touch headPosition/bodyPosition/legPosition here.
    private void HoldDeathVisuals(Player player)
    {
        if (!UsesStudentOutfit() || !player.dead)
            return;

        // Stop the detached visual body parts falling away from the death location.
        // This changes rendering state, NOT Player.position/velocity or world physics.
        player.headVelocity = Vector2.Zero;
        player.bodyVelocity = Vector2.Zero;
        player.legVelocity = Vector2.Zero;
    }

    public override void UpdateDead()
    {
        // Alive updates stop at death. Preserve the visual slots during the death
        // countdown, so the appearance does not turn back into the vanilla character.
        ApplyStudentVisualSlots(Player);
        HoldDeathVisuals(Player);
    }

    public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g,
        ref float b, ref float a, ref bool fullBright)
    {
        Player target = drawInfo.drawPlayer;
        if (target == null)
            return;

        // This is the missing half of the Elaina pattern: restore the current
        // student's head/body/legs whenever Terraria builds a draw pass, even if
        // FrameEffects was not updated because the world is auto-paused.
        // Assign VISUAL equip slots only, never armor/vanity inventory contents.
        ApplyStudentVisualSlots(target);

        if (!target.dead || !UsesStudentOutfit())
            return;

        HoldDeathVisuals(target);

        // Match Elaina's death frame selection: final head/body animation frame and
        // second leg frame. Player-format sheets have 20 rows of 56 pixels.
        // Avoid modifying frame.X, since Kivotos' custom sheets are only 40 px wide.
        if (target.headFrame.Height > 0)
            target.headFrame.Y = 19 * target.headFrame.Height;
        if (target.bodyFrame.Height > 0)
            target.bodyFrame.Y = 19 * target.bodyFrame.Height;
        if (target.legFrame.Height > 0)
            target.legFrame.Y = target.legFrame.Height;
    }

    private bool UsesStudentOutfit() => IsYuukaAppearance || IsHinaAppearance || IsNoaAppearance
        || YuukaVanitySet.FullSetVisible(Player) || HinaVanitySet.FullSetVisible(Player)
        || NoaVanitySet.FullSetVisible(Player);

    public void Select(TabletStudentId student)
    {
        SelectedStudent = student;
        HasSelection = true;
    }

    public void ClearSelection()
    {
        HasSelection = false;
    }

    internal static bool IsFormationStudent(TabletStudentId student) => student is
        TabletStudentId.Yuuka or TabletStudentId.Hina or TabletStudentId.Noa
        or TabletStudentId.Shiroko or TabletStudentId.Hoshino;

    internal void ReceiveSelection(bool hasSelection, TabletStudentId student)
    {
        HasSelection = hasSelection;
        SelectedStudent = student;
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        TabletNet.SendAppearance(Player.whoAmI, HasSelection, SelectedStudent, toWho, fromWho);
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        TabletStudentAppearancePlayer copy = (TabletStudentAppearancePlayer)targetCopy;
        copy.HasSelection = HasSelection;
        copy.SelectedStudent = SelectedStudent;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        TabletStudentAppearancePlayer previous = (TabletStudentAppearancePlayer)clientPlayer;
        if (HasSelection != previous.HasSelection || SelectedStudent != previous.SelectedStudent)
            SyncPlayer(-1, -1, false);
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
        TabletStudentId decoded = raw <= (byte)TabletStudentId.Noa ? (TabletStudentId)raw : TabletStudentId.Yuuka;

        // Keep old save values stable (Arona=3, Sora=4, Plana=5) and reject
        // companion-only values in Formation. Hina is the new value 6.
        if (!IsFormationStudent(decoded))
        {
            SelectedStudent = TabletStudentId.Yuuka;
            HasSelection = false;
        }
        else
        {
            SelectedStudent = decoded;
        }
    }

    /// <summary>
    /// Set visual slots without modifying the player's armor or vanity inventory.
    /// Call during FrameEffects, UpdateAutopause and DrawEffects. This mirrors the
    /// direct visual slot assignments used by ElainaModplayer.
    /// </summary>
    private void ApplyStudentVisualSlots(Player target)
    {
        if (IsYuukaAppearance)
        {
            target.head = ModContent.GetInstance<YuukaHead>().Item.headSlot;
            target.body = ModContent.GetInstance<YuukaBody>().Item.bodySlot;
            target.legs = ModContent.GetInstance<YuukaBoots>().Item.legSlot;
        }
        else if (IsHinaAppearance)
        {
            target.head = ModContent.GetInstance<HinaHead>().Item.headSlot;
            target.body = ModContent.GetInstance<HinaBody>().Item.bodySlot;
            target.legs = ModContent.GetInstance<HinaBoots>().Item.legSlot;
        }
        else if (IsNoaAppearance)
        {
            target.head = ModContent.GetInstance<NoaHead>().Item.headSlot;
            target.body = ModContent.GetInstance<NoaBody>().Item.bodySlot;
            target.legs = ModContent.GetInstance<NoaBoots>().Item.legSlot;
        }
    }

    // The vanilla Player.legFrame is animation state, not just draw state. Modifying it
    // in FrameEffects makes the NEXT vanilla walk tick start from the mirrored frame.
    // The temporary override below is scoped to drawing and always restored afterwards.
    private Rectangle _originalLegFrame;
    private bool _legFrameOverriddenForDraw;

    private void RestoreLegFrameAfterDraw()
    {
        if (!_legFrameOverriddenForDraw)
            return;

        Player.legFrame = _originalLegFrame;
        _legFrameOverriddenForDraw = false;
    }

    // Keep existing Elaina-style appearance handling for pause and ordinary gameplay.
    public override void UpdateAutopause()
    {
        RestoreLegFrameAfterDraw(); // Recovery if a draw pass was interrupted.
        ApplyStudentVisualSlots(Player);
    }

    public override void FrameEffects()
    {
        RestoreLegFrameAfterDraw();
        ApplyStudentVisualSlots(Player);
    }

    public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
    {
        // Called AFTER normal animation was calculated, BEFORE the player draw layers.
        // Never accumulate edits to the real walking animation or change gameplay movement.
        RestoreLegFrameAfterDraw();
        Player target = drawInfo.drawPlayer;
        if (target != Player || target.dead || target.ghost || !UsesStudentOutfit()
            || target.mount.Active || System.Math.Abs(target.velocity.X) < 0.10f)
            return;

        // When aiming left and moving right (or vice versa), play ONLY the visible
        // stride backwards. Keep air, sitting, idle, mount, and non-stride frames intact.
        if (target.direction * target.velocity.X >= 0f)
            return;

        int frameHeight = target.legFrame.Height;
        if (frameHeight <= 0)
            return;

        int frame = target.legFrame.Y / frameHeight;
        if (frame < 6 || frame > 16)
            return;

        _originalLegFrame = target.legFrame;
        _legFrameOverriddenForDraw = true;
        target.legFrame.Y = (22 - frame) * frameHeight;
    }

    public override void TransformDrawData(ref PlayerDrawSet drawInfo)
    {
        // Every leg/shoe layer has now copied its source frame into DrawDataCache.
        // Restore the real frame BEFORE Terraria or another mod reads it next time.
        RestoreLegFrameAfterDraw();
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp,
        ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
    {
        // Elaina reference: suppress death gore without interfering with vanilla
        // death positioning, respawn timer, item drops or network state.
        // Do not affect unselected players, or student entries that have no outfit yet.
        if (UsesStudentOutfit())
            genGore = false;

        // Let Terraria process the death normally (items, respawn, sounds, statistics, etc.).
        return true;
    }

}
