using System.Collections.Generic;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Clean-room, UI-only student metadata used by the tablet Formation page.
/// It deliberately contains no NPC type, AI, spawning, combat controller, or team deployment.
/// Portrait/thumbnail art is loaded as UI content only.
/// </summary>
public enum TabletStudentId : byte
{
    Yuuka,
    Hoshino,
    Shiroko,
    Arona,
    Sora,
    Plana,
}

public sealed class TabletStudentInfo
{
    public TabletStudentId Id { get; }
    public string DisplayName { get; }
    public string GivenName { get; }
    public string FactionName { get; }
    public int LegacyFactionFrame { get; }
    public int AttackTypeFrame { get; }
    public int RoleFrame { get; }
    public string ThumbnailPath { get; }
    public bool HasSimpleLayeredPortrait { get; }
    public string PortraitBasePath { get; }
    public string PortraitFacePath { get; }
    public string PortraitHaloPath { get; }
    public int BaseFrameCount { get; }
    public int FaceFrameCount { get; }
    public Microsoft.Xna.Framework.Vector2 BaseOffset { get; }
    public Microsoft.Xna.Framework.Vector2 BaseOrigin { get; }
    public Microsoft.Xna.Framework.Vector2 HeadOffset { get; }
    public Microsoft.Xna.Framework.Vector2 FaceOrigin { get; }
    public Microsoft.Xna.Framework.Vector2 HaloOffset { get; }

    public TabletStudentInfo(
        TabletStudentId id,
        string displayName,
        string givenName,
        string factionName,
        int legacyFactionFrame,
        int attackTypeFrame,
        int roleFrame,
        string thumbnailPath,
        bool hasSimpleLayeredPortrait = false,
        string portraitBasePath = "",
        string portraitFacePath = "",
        string portraitHaloPath = "",
        int baseFrameCount = 1,
        int faceFrameCount = 1,
        Microsoft.Xna.Framework.Vector2? baseOffset = null,
        Microsoft.Xna.Framework.Vector2? baseOrigin = null,
        Microsoft.Xna.Framework.Vector2? headOffset = null,
        Microsoft.Xna.Framework.Vector2? faceOrigin = null,
        Microsoft.Xna.Framework.Vector2? haloOffset = null)
    {
        Id = id;
        DisplayName = displayName;
        GivenName = givenName;
        FactionName = factionName;
        LegacyFactionFrame = legacyFactionFrame;
        AttackTypeFrame = attackTypeFrame;
        RoleFrame = roleFrame;
        ThumbnailPath = thumbnailPath;
        HasSimpleLayeredPortrait = hasSimpleLayeredPortrait;
        PortraitBasePath = portraitBasePath;
        PortraitFacePath = portraitFacePath;
        PortraitHaloPath = portraitHaloPath;
        BaseFrameCount = baseFrameCount;
        FaceFrameCount = faceFrameCount;
        BaseOffset = baseOffset ?? Microsoft.Xna.Framework.Vector2.Zero;
        BaseOrigin = baseOrigin ?? Microsoft.Xna.Framework.Vector2.Zero;
        HeadOffset = headOffset ?? Microsoft.Xna.Framework.Vector2.Zero;
        FaceOrigin = faceOrigin ?? Microsoft.Xna.Framework.Vector2.Zero;
        HaloOffset = haloOffset ?? Microsoft.Xna.Framework.Vector2.Zero;
    }
}

public static class TabletStudentCatalog
{
    public static readonly IReadOnlyList<TabletStudentInfo> Students = new TabletStudentInfo[]
    {
        // Attack frames: 0 Generic, 1 Explosive, 2 Piercing, 3 Mystic, 4 Sonic, 5 Siege.
        // Role frames:   0 None, 1 Tank, 2 Dealer, 3 Healer, 4 Support, 5 Tactical.
        // Legacy faction frame numbers are used only to select the supplied tile-background art.
        new(
            TabletStudentId.Yuuka, "Yuuka Hayase", "Yuuka", "Millennium", 7, 1, 1,
            "KivotosMod/Assets/Tablet/Students/Yuuka/Thumbnail",
            true,
            "KivotosMod/Assets/Tablet/Students/Yuuka/Portrait/Base",
            "KivotosMod/Assets/Tablet/Students/Yuuka/Portrait/Face",
            "KivotosMod/Assets/Tablet/Students/Yuuka/Portrait/Halo",
            2, 9,
            new Microsoft.Xna.Framework.Vector2(0f, -263f),
            new Microsoft.Xna.Framework.Vector2(122f, 203f),
            new Microsoft.Xna.Framework.Vector2(0f, -99f),
            new Microsoft.Xna.Framework.Vector2(34f, 54f),
            new Microsoft.Xna.Framework.Vector2(-16f, -99f)),

        new(
            TabletStudentId.Hoshino, "Hoshino Takanashi", "Hoshino", "Abydos", 5, 1, 1,
            "KivotosMod/Assets/Tablet/Students/Hoshino/Thumbnail",
            true,
            "KivotosMod/Assets/Tablet/Students/Hoshino/Portrait/Base",
            "KivotosMod/Assets/Tablet/Students/Hoshino/Portrait/Face",
            "KivotosMod/Assets/Tablet/Students/Hoshino/Portrait/Halo",
            2, 9,
            new Microsoft.Xna.Framework.Vector2(0f, -274f),
            new Microsoft.Xna.Framework.Vector2(128f, 178f),
            new Microsoft.Xna.Framework.Vector2(0f, -64f),
            new Microsoft.Xna.Framework.Vector2(38f, 50f),
            new Microsoft.Xna.Framework.Vector2(-5f, -103f)),

        new(
            TabletStudentId.Shiroko, "Shiroko Sunaookami", "Shiroko", "Abydos", 5, 1, 2,
            "KivotosMod/Assets/Tablet/Students/Shiroko/Thumbnail",
            true,
            "KivotosMod/Assets/Tablet/Students/Shiroko/Portrait/Base",
            "KivotosMod/Assets/Tablet/Students/Shiroko/Portrait/Face",
            "KivotosMod/Assets/Tablet/Students/Shiroko/Portrait/Halo",
            2, 9,
            new Microsoft.Xna.Framework.Vector2(0f, -274f),
            new Microsoft.Xna.Framework.Vector2(102f, 204f),
            new Microsoft.Xna.Framework.Vector2(0f, -86f),
            new Microsoft.Xna.Framework.Vector2(34f, 52f),
            new Microsoft.Xna.Framework.Vector2(0f, -97f)),

        new(TabletStudentId.Arona, "Arona", "Arona", "Schale", 3, 0, 0,
            "KivotosMod/Assets/Tablet/Students/Arona/Thumbnail"),
        new(TabletStudentId.Sora, "Sora", "Sora", "Angel 24", 4, 0, 0,
            "KivotosMod/Assets/Tablet/Students/Sora/Thumbnail"),
        new(TabletStudentId.Plana, "Plana", "Plana", "Schale", 3, 0, 0,
            "KivotosMod/Assets/Tablet/Students/Plana/Thumbnail"),
    };

    // Unit Formation is a player-appearance picker, not the HOME companion roster.
    // Keep only the three FIX6-era playable students in the grid. Companion/portrait
    // entries are a separate concern and are not presented as player outfits.
    public static readonly IReadOnlyList<TabletStudentInfo> FormationStudents = new TabletStudentInfo[]
    {
        Get(TabletStudentId.Yuuka),
        Get(TabletStudentId.Shiroko),
        Get(TabletStudentId.Hoshino),
    };

    // HOME only cycles entries that currently have a proper full portrait renderer.
    // Plana is kept in the metadata catalog for future work but has only a thumbnail in
    // the supplied assets, so she is deliberately skipped instead of showing a tiny card.
    public static readonly IReadOnlyList<TabletStudentInfo> CompanionStudents = new TabletStudentInfo[]
    {
        Get(TabletStudentId.Arona),
        Get(TabletStudentId.Sora),
        Get(TabletStudentId.Yuuka),
        Get(TabletStudentId.Shiroko),
        Get(TabletStudentId.Hoshino),
    };

    public static int CompanionIndexOf(TabletStudentId id)
    {
        for (int i = 0; i < CompanionStudents.Count; i++)
            if (CompanionStudents[i].Id == id)
                return i;
        return 0;
    }

    public static int FormationIndexOf(TabletStudentId id)
    {
        for (int i = 0; i < FormationStudents.Count; i++)
            if (FormationStudents[i].Id == id)
                return i;
        return 0;
    }

    public static TabletStudentInfo Get(TabletStudentId id)
    {
        foreach (TabletStudentInfo student in Students)
            if (student.Id == id)
                return student;

        return Students[0];
    }

    public static int IndexOf(TabletStudentId id)
    {
        for (int i = 0; i < Students.Count; i++)
            if (Students[i].Id == id)
                return i;
        return 0;
    }
}
