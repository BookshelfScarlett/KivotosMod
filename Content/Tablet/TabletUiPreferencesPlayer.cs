using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Small persistent client-facing preference bucket for the clean-room tablet UI.
/// Kept separate from companion/Formation state so changing the tablet presentation
/// never changes the selected student or player appearance.
/// </summary>
public sealed class TabletUiPreferencesPlayer : ModPlayer
{
    private static readonly float[] ScalePresets =
    {
        0.60f,
        0.70f,
        0.80f,
        0.90f,
        1.00f,
        1.10f,
        1.20f,
        1.30f,
        1.40f,
    };

    private const int DefaultScaleIndex = 4;
    private int _scaleIndex = DefaultScaleIndex;

    public float TabletScale => ScalePresets[Math.Clamp(_scaleIndex, 0, ScalePresets.Length - 1)];
    public bool CanDecreaseScale => _scaleIndex > 0;
    public bool CanIncreaseScale => _scaleIndex < ScalePresets.Length - 1;
    public bool UseRealTimeClock { get; private set; } = true;

    // Stored in UI coordinates, not world pixels: movable shortcut survives inventory
    // reopening and character reloads. Negative values mean the default placement.
    private float _inventoryIconX = -1f;
    private float _inventoryIconY = -1f;

    // Open tablet translation relative to the centered default position, in UI pixels.
    // This is separate from the inventory shortcut position.
    private float _tabletOffsetX;
    private float _tabletOffsetY;

    public Vector2 GetTabletOffset(float viewportWidth, float viewportHeight, float tabletWidth, float tabletHeight)
    {
        float maxX = Math.Max(0f, (viewportWidth - tabletWidth) * 0.5f);
        float maxY = Math.Max(0f, (viewportHeight - tabletHeight) * 0.5f);
        return new Vector2(
            Math.Clamp(_tabletOffsetX, -maxX, maxX),
            Math.Clamp(_tabletOffsetY, -maxY, maxY));
    }

    public void MoveTabletOffset(Vector2 delta, float viewportWidth, float viewportHeight, float tabletWidth, float tabletHeight)
    {
        Vector2 current = GetTabletOffset(viewportWidth, viewportHeight, tabletWidth, tabletHeight);
        float maxX = Math.Max(0f, (viewportWidth - tabletWidth) * 0.5f);
        float maxY = Math.Max(0f, (viewportHeight - tabletHeight) * 0.5f);
        _tabletOffsetX = Math.Clamp(current.X + delta.X, -maxX, maxX);
        _tabletOffsetY = Math.Clamp(current.Y + delta.Y, -maxY, maxY);
    }

    public Vector2 GetInventoryIconPosition(int viewportWidth, int viewportHeight)
    {
        float x = _inventoryIconX >= 0f ? _inventoryIconX : 735f;
        float y = _inventoryIconY >= 0f ? _inventoryIconY : 52f;
        return new Vector2(
            Math.Clamp(x, 4f, Math.Max(4f, viewportWidth - 42f)),
            Math.Clamp(y, 4f, Math.Max(4f, viewportHeight - 40f)));
    }

    public void SetInventoryIconPosition(Vector2 position, int viewportWidth, int viewportHeight)
    {
        _inventoryIconX = Math.Clamp(position.X, 4f, Math.Max(4f, viewportWidth - 42f));
        _inventoryIconY = Math.Clamp(position.Y, 4f, Math.Max(4f, viewportHeight - 40f));
    }

    public void SetClockSource(bool realTime)
    {
        UseRealTimeClock = realTime;
    }

    public void ChangeScale(int direction)
    {
        if (direction == 0)
            return;

        _scaleIndex = Math.Clamp(
            _scaleIndex + (direction < 0 ? -1 : 1),
            0,
            ScalePresets.Length - 1);
    }

    public void ResetScale()
    {
        _scaleIndex = DefaultScaleIndex;
    }

    public override void SaveData(TagCompound tag)
    {
        tag[nameof(_scaleIndex)] = _scaleIndex;
        tag[nameof(UseRealTimeClock)] = UseRealTimeClock;
        tag[nameof(_tabletOffsetX)] = _tabletOffsetX;
        tag[nameof(_tabletOffsetY)] = _tabletOffsetY;
        if (_inventoryIconX >= 0f && _inventoryIconY >= 0f)
        {
            tag[nameof(_inventoryIconX)] = _inventoryIconX;
            tag[nameof(_inventoryIconY)] = _inventoryIconY;
        }
    }

    public override void LoadData(TagCompound tag)
    {
        _scaleIndex = tag.ContainsKey(nameof(_scaleIndex))
            ? Math.Clamp(tag.GetInt(nameof(_scaleIndex)), 0, ScalePresets.Length - 1)
            : DefaultScaleIndex;
        UseRealTimeClock = !tag.ContainsKey(nameof(UseRealTimeClock))
            || tag.GetBool(nameof(UseRealTimeClock));
        _tabletOffsetX = tag.ContainsKey(nameof(_tabletOffsetX)) ? tag.GetFloat(nameof(_tabletOffsetX)) : 0f;
        _tabletOffsetY = tag.ContainsKey(nameof(_tabletOffsetY)) ? tag.GetFloat(nameof(_tabletOffsetY)) : 0f;
        _inventoryIconX = tag.ContainsKey(nameof(_inventoryIconX)) ? tag.GetFloat(nameof(_inventoryIconX)) : -1f;
        _inventoryIconY = tag.ContainsKey(nameof(_inventoryIconY)) ? tag.GetFloat(nameof(_inventoryIconY)) : -1f;
    }
}
