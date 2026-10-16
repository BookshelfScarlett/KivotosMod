using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet;

public sealed partial class TabletUiSystem
{
    // UILobby.prefab's Top / Left / Bottom groups, adapted to the tablet's 4:3 glass.
    // Sprites are the original NGUI atlas crops, with their aspect ratio preserved.
    private static readonly string[] LobbyIcons = { "Cafe", "SchaleBilding", "Student", "Edit", "Social", "Craft", "Shop", "Gacha" };
    private static readonly string[] LobbyLabels = { "咖啡厅", "日程", "学生", "编队", "社交", "制造", "商店", "招募" };
    private static readonly string[] LobbyShortcutIcons = { "Common_Icon_Notice", "Common_Icon_MoMoTalk", "School_Icon_Schedule", "Common_Icon_Gembag" };
    private static readonly string[] LobbyShortcutLabels = { "公告", "MomoTalk", "任务", "青辉石购买" };
    private static readonly string[] LobbyBanners = { "Lobby_Gacha_Banner_210204_1", "Event_Banner_841_Jp", "Event_Banner_842_Jp" };
    private static readonly Rectangle LobbyBannerRect = new(458, 118, 77, 49);
    private static readonly Rectangle LobbyPickupRect = new(43, 294, 118, 59);
    private static readonly Rectangle LobbyCharacterRect = new(464, 40, 24, 25);
    private static readonly Rectangle LobbyHideRect = new(503, 77, 33, 25);
    private static readonly Rectangle LobbyMailRect = new(489, 40, 24, 25);
    private static readonly Rectangle LobbyCombatRect = new(453, 329, 82, 85);
    private static readonly Rectangle LobbyRotateRect = new(465, 77, 34, 25);
    private static readonly Rectangle LobbyPortraitRect = new(128, 82, 328, 334);
    private static readonly Rectangle LobbyPreviousRect = new(34, 225, 19, 32);
    private static readonly Rectangle LobbyNextRect = new(524, 225, 20, 32);
    private static readonly Rectangle LobbyNoticeRect = new(133, 158, 344, 134);
    private static readonly Rectangle LobbyClockRect = new(396, 390, 49, 24);
    private static readonly Dictionary<string, Asset<Texture2D>> LobbyTextures = new(StringComparer.Ordinal);
    private static TabletSpineScene _lobbyArona;
    private static bool _lobbyLoadFailed;
    private static bool _lobbyHidden;
    private static bool _lobbyCharactersOpen;
    private static bool _lobbyMemoryScene;
    private static string _lobbyNotice;
    private static float _lobbyBannerTime;
    private static readonly Color LobbyInk = new(21, 55, 87);
    private static readonly Color LobbyWhite = Color.White * (235f / 255f);
    // UILobby's serialized inclineAngle is 10 degrees for the top goods and profile.
    private const float LobbyIncline = 10f;
    private const float LobbyAronaScale = 0.30f;
    private static readonly Vector2 LobbyAronaAnchor = new(287, 480);

    private static Rectangle LobbyNavRect(int index) => new(39 + index * 44, 363, 44, 51);
    private static Rectangle LobbyShortcutRect(int index) => new(43 + index % 2 * 52, 108 + index / 2 * 58, 49, 53);

    private static Texture2D LobbyTexture(string name)
    {
        if (!LobbyTextures.TryGetValue(name, out Asset<Texture2D> asset))
            LobbyTextures[name] = asset = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Home/UI/" + name, AssetRequestMode.ImmediateLoad);
        return asset.Value;
    }

    private static void UpdateLobby(float seconds)
    {
        if (_lobbyArona == null && !_lobbyLoadFailed)
        {
            try
            {
                Mod mod = _instance.Mod;
                _lobbyArona = new TabletSpineScene(mod, "Arona", "arona_spr", "Idle_01", blink: true);
            }
            catch (Exception exception)
            {
                _lobbyLoadFailed = true;
                _instance.Mod.Logger.Error("Unable to load Blue Archive lobby Spine assets.", exception);
            }
        }
        _lobbyArona?.Update(seconds);
        _lobbyBannerTime = (_lobbyBannerTime + Math.Clamp(seconds, 0, 0.1f)) % 8f;
    }

    private static void UnloadLobby()
    {
        TabletSpineScene arona = _lobbyArona;
        _lobbyArona = null;
        Main.QueueMainThreadAction(() => arona?.Dispose());
        LobbyTextures.Clear();
        _lobbyLoadFailed = _lobbyHidden = _lobbyCharactersOpen = _lobbyMemoryScene = false;
        _lobbyNotice = null;
        _lobbyBannerTime = 0;
    }

    private static void DrawLobbyBackground(SpriteBatch sb, Layout layout) => DrawClassroomBackground(sb, layout);

    private static void DrawLobbyHome(SpriteBatch sb, Layout layout, Vector2 mouse)
    {
        TabletStudentInfo student = TabletStudentCatalog.Get(Main.LocalPlayer.GetModPlayer<TabletCompanionPlayer>().Companion);
        if (student.Id == TabletStudentId.Arona && !_lobbyMemoryScene)
        {
            if (!TabletDynamicPortraitRenderer.DrawArona38(sb, layout.ToScreen, layout.Scale, new Vector2(287, 82), 620f, DisplayRect, mouse))
            {
                if (_lobbyArona != null)
                    _lobbyArona.Draw(sb, layout.ToScreen(LobbyAronaAnchor), LobbyAronaScale * layout.Scale, layout.ToScreen(DisplayRect));
                else DrawTabletPortrait(sb, student, LobbyPortraitRect, layout, mouse, 1.10f, HomePortraitRootOffset, DisplayRect);
            }
        }
        else if (student.Id != TabletStudentId.Arona || _lobbyArona == null)
            DrawTabletPortrait(sb, student, LobbyPortraitRect, layout, mouse, 1.10f, HomePortraitRootOffset, DisplayRect);

        if (!_lobbyHidden)
        {
            LobbyPanel(sb, new Rectangle(39, 40, 125, 46), layout, LobbyInk, LobbyIncline);
            LobbyText(sb, "HP", new Vector2(56, 43), new Color(255, 222, 45), 9f, layout);
            LobbyText(sb, Main.LocalPlayer.statLife.ToString(), new Vector2(57, 56), Color.White, 17f, layout, maxWidth: 34f);
            string name = Main.LocalPlayer.name;
            if (name.Length > 10) name = name[..9] + "…";
            LobbyText(sb, name, new Vector2(115, 45), Color.White, 12f, layout, maxWidth: 75f);
            DrawSolid(sb, new Rectangle(77, 63, 77, 2), new Color(85, 97, 112), layout);
            DrawSolid(sb, new Rectangle(77, 63, (int)(77 * Math.Clamp(Main.LocalPlayer.statLife / (float)Math.Max(1, Main.LocalPlayer.statLifeMax2), 0, 1)), 2), new Color(81, 211, 240), layout);
            LobbyText(sb, $"{Main.LocalPlayer.statLife}/{Main.LocalPlayer.statLifeMax2}", new Vector2(116, 69), new Color(103, 218, 246), 8f, layout);

            long credits = TabletWallet.GetTotalCredits(Main.LocalPlayer);
            LobbyCurrency(sb, "Stamina", $"{Main.LocalPlayer.statMana}/{Main.LocalPlayer.statManaMax2}", new Rectangle(181, 40, 77, 25), layout, true);
            LobbyCurrency(sb, "Gold", credits.ToString("N0"), new Rectangle(265, 40, 102, 25), layout);
            LobbyCurrency(sb, "Diamond", (credits / TabletWallet.CreditsPerPyroxene).ToString("N0"), new Rectangle(374, 40, 82, 25), layout, true);
            LobbyPanel(sb, new Rectangle(464, 40, 74, 25), layout, LobbyWhite, LobbyIncline);
            LobbyIcon(sb, "Common_Icon_SpecialLobby", new Rectangle(468, 46, 17, 15), layout, LobbyInk);
            LobbyIcon(sb, "Common_Icon_Mail", new Rectangle(493, 46, 17, 15), layout, LobbyInk);
            LobbyIcon(sb, "Common_Icon_Menu_Small", new Rectangle(519, 46, 15, 15), layout, LobbyInk);
            for (int i = 0; i < LobbyShortcutIcons.Length; i++)
            {
                Rectangle rect = LobbyShortcutRect(i);
                LobbyIconButton(sb, LobbyShortcutIcons[i], rect, LobbyShortcutLabels[i], mouse, layout, true);
            }
            int banner = (int)(_lobbyBannerTime / 4f);
            DrawTextureFit(sb, LobbyTexture(LobbyBanners[1 + banner % 2]), LobbyBannerRect, layout);
            DrawTextureFit(sb, LobbyTexture(LobbyBanners[0]), LobbyPickupRect, layout);
            LobbyText(sb, "活动", new Vector2(LobbyBannerRect.Center.X, 108), Color.White, 9f, layout);
            LobbyPanel(sb, new Rectangle(480, 170, 32, 7), layout, LobbyWhite);
            for (int i = 0; i < 2; i++)
                DrawSolid(sb, new Rectangle(491 + i * 8, 172, 2, 2), i == banner ? new Color(72, 187, 236) : LobbyInk, layout);
            LobbyPanel(sb, new Rectangle(39, 392, 407, 23), layout, LobbyWhite, LobbyIncline);
            for (int i = 0; i < LobbyIcons.Length; i++)
                LobbyIconButton(sb, "Common_Icon_" + LobbyIcons[i], LobbyNavRect(i), LobbyLabels[i], mouse, layout, false);
            DrawLobbyClock(sb, layout);
            DrawTextureFit(sb, LobbyTexture("Lobby_Icon_Work"), new Rectangle(451, 329, 87, 66), layout);
            LobbyPanel(sb, new Rectangle(453, 387, 82, 27), layout, LobbyInk, LobbyIncline);
            LobbyText(sb, "工作", new Vector2(494, 393), Color.White, 13f, layout);
            LobbyPanel(sb, LobbyRotateRect, layout, new Color(205, 214, 221) * (220f / 255f), LobbyIncline);
            LobbyIcon(sb, "Common_Icon_Change_Count", new Rectangle(470, 81, 24, 17), layout, new Color(131, 147, 159));
            LobbyIcon(sb, "Cafe_Icon_Arrow", LobbyPreviousRect, layout, Color.White);
            LobbyIcon(sb, "Cafe_Icon_Arrow", LobbyNextRect, layout, Color.White, SpriteEffects.FlipHorizontally);
        }
        LobbyPanel(sb, LobbyHideRect, layout, LobbyWhite, LobbyIncline);
        LobbyIcon(sb, "Common_UIHide_Icon2", new Rectangle(510, 81, 19, 17), layout, LobbyInk);
        if (_lobbyCharactersOpen && !_lobbyHidden) DrawLobbyCharacters(sb, layout, mouse);
        if (_lobbyNotice != null) DrawLobbyNotice(sb, layout);
    }

    private static void DrawLobbyClock(SpriteBatch sb, Layout layout)
    {
        bool realTime = Main.LocalPlayer.GetModPlayer<TabletUiPreferencesPlayer>().UseRealTimeClock;
        string time = realTime
            ? DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture)
            : FormatLobbyGameTime(Main.time, Main.dayTime);
        Rectangle decoration = realTime
            ? new Rectangle(403, 391, 36, 7)
            : new Rectangle(398, 392, 24, 5);
        DrawTextureFit(sb, LobbyTexture("UIWorkAronaSceneBG_DecoColor"), decoration, layout);
        if (!realTime)
            LobbyText(sb, Main.dayTime ? "白天" : "夜晚", new Vector2(434, 391), LobbyInk, 7f, layout, maxWidth: 20);
        LobbyText(sb, time, new Vector2(LobbyClockRect.Center.X, 400), new Color(45, 70, 100), 11f, layout, maxWidth: LobbyClockRect.Width - 3);
    }

    internal static string FormatLobbyGameTime(double elapsed, bool dayTime)
    {
        // Main.time starts at dawn (04:30) or dusk (19:30), and resets each phase.
        int minute = ((int)(Math.Max(0, elapsed) / 60) + (dayTime ? 270 : 1170)) % 1440;
        return string.Format(CultureInfo.InvariantCulture, "{0:00}:{1:00}", minute / 60, minute % 60);
    }

    private static void LobbyCurrency(SpriteBatch sb, string icon, string value, Rectangle rect, Layout layout, bool plus = false)
    {
        LobbyPanel(sb, rect, layout, LobbyWhite, LobbyIncline);
        DrawTextureFit(sb, LobbyTexture("Common_Icon_" + icon), new Rectangle(rect.X + 3, rect.Y + 3, 17, 18), layout);
        int textWidth = rect.Width - 26 - (plus ? 12 : 0);
        LobbyText(sb, value, new Vector2(rect.X + 23 + textWidth / 2f, rect.Y + 7), LobbyInk, 10f, layout, maxWidth: textWidth);
        if (plus) LobbyIcon(sb, "Common_Plus_Btn", new Rectangle(rect.Right - 13, rect.Y + 8, 8, 8), layout, new Color(39, 175, 225));
    }

    private static void LobbyIconButton(SpriteBatch sb, string icon, Rectangle rect, string label, Vector2 mouse, Layout layout, bool panel)
    {
        if (rect.Contains(mouse.ToPoint())) DrawSolid(sb, rect, new Color(88, 207, 255) * (60f / 255f), layout);
        DrawTextureFit(sb, LobbyTexture(icon), new Rectangle(rect.X + 6, rect.Y + 1, rect.Width - 12, panel ? 34 : 39), layout);
        Vector2 labelPosition = new(rect.Center.X, rect.Y + (panel ? 37 : 40));
        if (panel)
        {
            foreach (Vector2 offset in new[] { new Vector2(-0.6f, 0), new Vector2(0.6f, 0), new Vector2(0, -0.6f), new Vector2(0, 0.6f) })
                LobbyText(sb, label, labelPosition + offset, LobbyInk, 9f, layout, maxWidth: rect.Width);
            LobbyText(sb, label, labelPosition, Color.White, 9f, layout, maxWidth: rect.Width);
        }
        else LobbyText(sb, label, labelPosition, LobbyInk, 10f, layout, maxWidth: rect.Width - 2);
    }

    private static void LobbyIcon(SpriteBatch sb, string name, Rectangle rect, Layout layout, Color tint, SpriteEffects effects = SpriteEffects.None)
    {
        Texture2D tex = LobbyTexture(name);
        float fit = Math.Min(rect.Width / (float)tex.Width, rect.Height / (float)tex.Height);
        Vector2 center = layout.ToScreen(new Vector2(rect.Center.X, rect.Center.Y));
        sb.Draw(tex, center, null, tint, 0, new Vector2(tex.Width / 2f, tex.Height / 2f), fit * layout.Scale, effects, 0);
    }

    private static void LobbyPanel(SpriteBatch sb, Rectangle rect, Layout layout, Color? tint = null, float incline = 0)
    {
        Texture2D tex = LobbyTexture("Common_Bg_Raius5px_Shadow");
        Color color = tint ?? Color.White;
        bool skewed = incline != 0;
        if (skewed)
        {
            float tangent = MathF.Tan(MathHelper.ToRadians(incline));
            Matrix skew = Matrix.Identity;
            skew.M21 = -tangent;
            skew.M41 = tangent * layout.ToScreen(new Vector2(0, rect.Center.Y)).Y;
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                DepthStencilState.None, Main.Rasterizer, null, skew * Main.UIScaleMatrix);
        }
        // Preserve the authored rounded corners and shadow instead of stretching them.
        int[] sx = { 0, 20, tex.Width - 20, tex.Width };
        int[] sy = { 0, 15, tex.Height - 30, tex.Height };
        int[] dx = { rect.X, rect.X + 5, rect.Right - 5, rect.Right };
        int[] dy = { rect.Y, rect.Y + 4, rect.Bottom - 8, rect.Bottom };
        // Small indicators use proportionally smaller destination borders.
        if (rect.Height < 16) { dy[1] = rect.Y + 2; dy[2] = rect.Bottom - 3; }
        try
        {
            for (int y = 0; y < 3; y++) for (int x = 0; x < 3; x++)
                sb.Draw(tex, layout.ToScreen(new Rectangle(dx[x], dy[y], dx[x + 1] - dx[x], dy[y + 1] - dy[y])),
                    new Rectangle(sx[x], sy[y], sx[x + 1] - sx[x], sy[y + 1] - sy[y]), color);
        }
        finally
        {
            if (skewed)
            {
                sb.End();
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                    DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            }
        }
    }

    private static void LobbyText(SpriteBatch sb, string text, Vector2 centerTop, Color color, float pixelHeight, Layout layout, bool bold = true, float maxWidth = 0)
    {
        var font = (bold ? _uiFontBold : _uiFont)?.Value ?? FontAssets.MouseText.Value;
        Vector2 size = font.MeasureString(text);
        float scale = pixelHeight / Math.Max(1f, font.MeasureString("M").Y);
        if (maxWidth > 0 && size.X * scale > maxWidth) scale = maxWidth / Math.Max(1f, size.X);
        sb.DrawString(font, text, layout.ToScreen(centerTop), color, 0, new Vector2(size.X / 2, 0),
            scale * layout.Scale, SpriteEffects.None, 0);
    }

    private static void DrawLobbyCharacters(SpriteBatch sb, Layout layout, Vector2 mouse)
    {
        LobbyPanel(sb, new Rectangle(318, 109, 216, 155), layout);
        LobbyText(sb, "大厅角色", new Vector2(426, 116), LobbyInk, 12f, layout);
        var students = TabletStudentCatalog.CompanionStudents;
        for (int i = 0; i < students.Count; i++)
        {
            Rectangle rect = new(325 + i % 3 * 68, 140 + i / 3 * 57, 65, 53);
            if (rect.Contains(mouse.ToPoint())) DrawSolid(sb, rect, new Color(81, 183, 227) * (90f / 255f), layout);
            DrawTextureFit(sb, ModContent.Request<Texture2D>(students[i].ThumbnailPath).Value, new Rectangle(rect.X + 12, rect.Y, 40, 35), layout);
            LobbyText(sb, students[i].GivenName, new Vector2(rect.Center.X, rect.Y + 36), LobbyInk, 9f, layout);
        }
    }

    private static void DrawLobbyNotice(SpriteBatch sb, Layout layout)
    {
        DrawSolid(sb, DisplayRect, Color.Black * 0.28f, layout);
        LobbyPanel(sb, LobbyNoticeRect, layout);
        LobbyText(sb, _lobbyNotice, new Vector2(305, 180), LobbyInk, 16f, layout);
        LobbyText(sb, "此入口已还原，功能尚未接入。", new Vector2(305, 215), LobbyInk, 11f, layout);
        LobbyText(sb, "点击返回大厅", new Vector2(305, 253), new Color(29, 155, 210), 11f, layout);
    }

    private static void HandleLobbyClick(Player player, Point point)
    {
        if (_lobbyNotice != null) { _lobbyNotice = null; SoundEngine.PlaySound(SoundID.MenuClose); return; }
        if (LobbyHideRect.Contains(point)) { _lobbyHidden = !_lobbyHidden; _lobbyCharactersOpen = false; return; }
        if (!_lobbyHidden && LobbyCharacterRect.Contains(point)) { _lobbyCharactersOpen = !_lobbyCharactersOpen; return; }
        if (_lobbyCharactersOpen && !_lobbyHidden)
        {
            var students = TabletStudentCatalog.CompanionStudents;
            for (int i = 0; i < students.Count; i++)
                if (new Rectangle(325 + i % 3 * 68, 140 + i / 3 * 57, 65, 53).Contains(point))
                { player.GetModPlayer<TabletCompanionPlayer>().SetCompanion(students[i].Id); _lobbyCharactersOpen = false; ResetLobbyMemoryScene(); SoundEngine.PlaySound(SoundID.MenuTick); return; }
            _lobbyCharactersOpen = false;
            return;
        }
        if (!_lobbyHidden)
        {
            if (LobbyPreviousRect.Contains(point) || LobbyNextRect.Contains(point))
            {
                CycleLobbyCompanion(player, LobbyPreviousRect.Contains(point) ? -1 : 1);
                return;
            }
            if (LobbyRotateRect.Contains(point))
            {
                // Temporarily disabled: our own Arona is the active portrait.
                return;
            }
            if (LobbyBannerRect.Contains(point)) { _lobbyNotice = "活动"; return; }
            if (LobbyPickupRect.Contains(point)) { _lobbyNotice = "招募"; return; }
            for (int i = 0; i < LobbyIcons.Length; i++)
            {
                if (!LobbyNavRect(i).Contains(point)) continue;
                if (i == 6)
                {
                    _shopScrollRow = 0; _selectedCategory = TabletShopCategory.All; _showingCart = false;
                    RequestPage(TabletPage.Shop);
                }
                else if (i == 3 || i == 2)
                {
                    TabletStudentAppearancePlayer appearance = player.GetModPlayer<TabletStudentAppearancePlayer>();
                    _selectedStudentIndex = appearance.HasSelection ? TabletStudentCatalog.FormationIndexOf(appearance.SelectedStudent) : 0;
                    RequestPage(TabletPage.Formation);
                }
                else _lobbyNotice = LobbyLabels[i];
                SoundEngine.PlaySound(SoundID.MenuOpen);
                return;
            }
            for (int i = 0; i < LobbyShortcutIcons.Length; i++)
                if (LobbyShortcutRect(i).Contains(point)) { _lobbyNotice = LobbyShortcutLabels[i]; return; }
            if (LobbyMailRect.Contains(point)) { _lobbyNotice = "邮件"; return; }
            if (LobbyCombatRect.Contains(point)) { _lobbyNotice = "工作"; return; }
        }
        if (LobbyPortraitRect.Contains(point) && player.GetModPlayer<TabletCompanionPlayer>().Companion == TabletStudentId.Arona)
        {
            if (!TabletDynamicPortraitRenderer.BlinkArona38()) _lobbyArona?.Touch();
        }
    }

    private static void CycleLobbyCompanion(Player player, int direction)
    {
        var students = TabletStudentCatalog.CompanionStudents;
        if (students.Count == 0) return;
        TabletCompanionPlayer companion = player.GetModPlayer<TabletCompanionPlayer>();
        int current = 0;
        for (int i = 0; i < students.Count; i++)
            if (students[i].Id == companion.Companion) { current = i; break; }
        companion.SetCompanion(students[(current + direction + students.Count) % students.Count].Id);
        ResetLobbyMemoryScene();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private static void ResetLobbyMemoryScene()
    {
        // The original Arona scene stays disabled with the legacy layered background.
        _lobbyMemoryScene = false;
    }
}
