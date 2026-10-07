using System;
using Terraria.ModLoader;
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
    }

    public override void LoadData(TagCompound tag)
    {
        _scaleIndex = tag.ContainsKey(nameof(_scaleIndex))
            ? Math.Clamp(tag.GetInt(nameof(_scaleIndex)), 0, ScalePresets.Length - 1)
            : DefaultScaleIndex;
    }
}
