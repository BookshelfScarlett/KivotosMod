using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace KivotosMod.Content.Tablet;

[Autoload(Side = ModSide.Client)]
public sealed class TabletUiSystem : ModSystem
{
    private enum TabletPage
    {
        Home,
        Shop,
        Formation,
    }

    private const int TabletWidth = 610;
    private const int TabletHeight = 450;

    // Presentation timing is kept separate from the shop logic.  The tablet is a physical
    // object first: selecting it raises it from below the screen, deselecting it reverses
    // the same motion.  45 ticks is the legacy-feel default (about 0.75 s at 60 Hz).
    private const int OpenCloseTicks = 45;
    private const int PageFadeTicks = 30;
    private const int PageHoldTicks = 30;
    private const int PageTransitionTicks = PageFadeTicks * 2 + PageHoldTicks;

    private static readonly Rectangle DisplayRect = new(32, 32, 514, 386);

    private static readonly Rectangle HomeShop = new(386, 94, 148, 72);
    private static readonly Rectangle HomeFormation = new(386, 170, 148, 72);
    private static readonly Rectangle HomeTravel = new(310, 246, 72, 72);
    private static readonly Rectangle HomeMessage = new(386, 246, 72, 72);
    private static readonly Rectangle HomeMission = new(462, 246, 72, 72);
    private static readonly Rectangle HomeWardrobe = new(310, 322, 72, 72);
    private static readonly Rectangle HomeUpgrade = new(386, 322, 72, 72);
    private static readonly Rectangle HomeArchive = new(462, 322, 72, 72);

    private static readonly Rectangle ReturnRect = new(50, 48, 36, 34);
    private static readonly Rectangle HomeButtonRect = new(556, 207, 36, 36);
    private static readonly Rectangle SettingsButtonRect = new(514, 40, 32, 20);

    // Tablet-local settings overlay.  "Resolution" here is the physical tablet render scale:
    // the authored surface remains 610x450, while the player can render it from 60% to 140%.
    private static readonly Rectangle SettingsPanelRect = new(156, 126, 298, 198);
    private static readonly Rectangle SettingsBackRect = new(166, 132, 32, 28);
    private static readonly Rectangle SettingsScaleDownRect = new(184, 220, 46, 34);
    private static readonly Rectangle SettingsScaleUpRect = new(380, 220, 46, 34);
    private static readonly Rectangle SettingsResetRect = new(244, 274, 122, 28);

    private static readonly Rectangle ShopPanelRect = new(302, 76, 240, 308);
    private static readonly Rectangle ShopCreditRect = new(302, 388, 100, 26);
    private static readonly Rectangle ShopPyroxeneRect = new(406, 388, 100, 26);
    private static readonly Rectangle ShopCartRect = new(510, 388, 32, 26);
    // Scrollbars follow the original authored shop geometry, but are implemented from scratch.
    private static readonly Rectangle ShopScrollTrackRect = new(530, 80, 8, 300);

    // Sora is the shop clerk on the authored SHOP screen.  Keep her independent tablet
    // portrait rig in the unused left half, behind the category buttons and product panel.
    // The clip stops hair/halo from bleeding into the top bar or category strip.
    private static readonly Rectangle ShopSoraPortraitRect = new(42, 74, 220, 340);
    private static readonly Rectangle ShopSoraClipRect = new(36, 72, 232, 342);
    private const float ShopSoraPortraitScale = 0.90f;
    private const float ShopSoraPortraitRootOffset = 116f;

    // Angel 24 foreground composition.  The counter/front layer occupies the same
    // 514x386 display-space as the shop background.  The small clerk bubble uses the
    // legacy visual placement (display + 8,298), reimplemented in this UI system.
    private static readonly Rectangle ShopSoraSpeechBubbleRect = new(40, 330, 240, 80);

    private static readonly Rectangle CheckoutOverlayRect = new(302, 76, 240, 308);
    private static readonly Rectangle CheckoutPanelRect = new(316, 144, 212, 194);
    private static readonly Rectangle CheckoutCancelRect = new(324, 304, 94, 26);
    private static readonly Rectangle CheckoutConfirmRect = new(426, 304, 94, 26);

    // FIX6-era Unit Formation selection geometry, expressed in the new tablet coordinate
    // system. The team/loadout/queue stage stays disabled: Formation opens directly here.
    // The 32 px display origin is already folded into these rectangles.
    private static readonly Rectangle FormationSortRect = new(40, 72, 188, 24);
    private static readonly Rectangle FormationSwitchRect = new(232, 72, 24, 24);
    private static readonly Rectangle FormationFilterRect = new(260, 72, 24, 24);
    private static readonly Rectangle FormationSelectionBackgroundRect = new(40, 100, 244, 282);
    private static readonly Rectangle FormationClearRect = new(40, 386, 24, 24);
    private static readonly Rectangle FormationSelectRect = new(68, 386, 216, 24);

    private static readonly Rectangle FormationStudentButtonRect = new(514, 72, 24, 24);
    private static readonly Rectangle FormationOutfitButtonRect = new(514, 100, 24, 24);
    private static readonly Rectangle FormationNameplateRect = new(298, 378, 240, 32);
    private static readonly Rectangle FormationInfoRect = new(374, 346, 88, 28);
    private static readonly Rectangle FormationPortraitRect = new(298, 72, 240, 306);
    // The legacy selected slot zoomed to 2x and then drew its portrait at 1x with the
    // portrait root far below the visible panel.  Keeping that root reproduces the
    // characteristic upper-body crop from the old Unit Formation screen.
    private const float FormationPortraitRootOffset = 194f;

    // Main-menu companion portrait anchor.  The old tablet drew a 512px portrait render-target
    // from a root below the visible display, producing a large upper-body composition instead of
    // fitting a whole character inside the left pane.  We reproduce that presentation without
    // importing the old UI/portrait code.
    private static readonly Rectangle HomePortraitRect = new(36, 70, 252, 348);
    private const float HomePortraitScale = 1.00f;
    // HOME portraits are intentionally anchored lower than Formation.  The previous
    // 34px offset left the head/halo above the visible display after clipping, so the
    // character read as a cropped torso.  116px keeps the authored large portrait scale
    // while framing the face and upper body inside the HOME screen.
    private const float HomePortraitRootOffset = 116f;
    private static readonly Rectangle HomeStudentLeftRect = new(40, 226, 18, 30);
    private static readonly Rectangle HomeStudentRightRect = new(262, 226, 18, 30);

    public static bool Visible { get; private set; }

    private static TabletUiSystem _instance;
    private UserInterface _tabletInterface;
    private TabletSurfaceState _tabletState;

    private static TabletPage _page = TabletPage.Home;
    private static int _shopScrollRow;
    private static TabletShopCategory _selectedCategory = TabletShopCategory.All;
    private static int _cartScrollIndex;
    private static bool _draggingScrollbar;
    private static bool _showingCart;
    private static bool _checkoutOpen;
    private static bool _checkoutPending;
    private static bool _settingsOpen;
    private static bool _leftWasDown;
    private static bool _escapeWasDown;
    private static int _selectedStudentIndex;

    // 0 = fully put away, OpenCloseTicks = fully presented.  Keeping the timer when the
    // player changes slots lets a quick re-select reverse the motion instead of snapping.
    private static int _holdTimer;

    // Page transitions deliberately live above page contents but below touch/power effects.
    private static int _pageTransitionElapsed = -1;
    private static TabletPage _pageTransitionTarget = TabletPage.Home;

    // Touch feedback uses the authored 27x4 sheets.  This is independent from buttons so
    // every tap/drag on the display gets the same glass-screen response.
    private static readonly List<TouchBurst> TouchBursts = new();
    private static readonly List<TouchSpark> TouchSparks = new();
    private static readonly List<Vector2> TouchTrail = new();
    private static Vector2? _lastTouchDragPoint;
    private static float _touchDragRemainder;

    private static readonly Dictionary<int, int> ProductQuantities = new();

    private static float OpenProgress => MathHelper.Clamp(_holdTimer / (float)OpenCloseTicks, 0f, 1f);
    private static bool FullyOpened => _holdTimer >= OpenCloseTicks;
    private static bool PageTransitionActive => _pageTransitionElapsed >= 0;

    private static Asset<Texture2D> _frame;
    private static Asset<Texture2D> _homeButton;
    private static Asset<Texture2D> _mainBackground;
    private static Asset<Texture2D> _shopBackground;
    private static Asset<Texture2D> _topBar;
    private static Asset<Texture2D> _returnButton;
    private static Asset<Texture2D> _mainShop;
    private static Asset<Texture2D> _shopSoraForeground;
    private static Asset<Texture2D> _shopSoraSpeechBubble;
    private static Asset<Texture2D> _mainFormation;
    private static Asset<Texture2D> _mainMission;
    private static Asset<Texture2D> _mainTravel;
    private static Asset<Texture2D> _mainArchive;
    private static Asset<Texture2D> _mainWardrobe;
    private static Asset<Texture2D> _mainUpgrade;
    private static Asset<Texture2D> _mainMessage;
    private static Asset<Texture2D> _shopPanel;
    private static Asset<Texture2D> _productCard;
    private static Asset<Texture2D> _purchaseButton;
    private static Asset<Texture2D> _productPlus;
    private static Asset<Texture2D> _productMinus;
    private static Asset<Texture2D> _currencyPanel;
    private static Asset<Texture2D> _cartButton;
    private static Asset<Texture2D> _cartEntry;
    private static Asset<Texture2D> _cartPlus;
    private static Asset<Texture2D> _cartMinus;
    private static Asset<Texture2D> _cartRemove;
    private static Asset<Texture2D> _checkoutBackground;
    private static Asset<Texture2D> _checkoutPanel;
    private static Asset<Texture2D> _checkoutCancel;
    private static Asset<Texture2D> _checkoutConfirm;
    private static Asset<Texture2D> _checkoutCheckbox;
    private static Asset<Texture2D> _creditIcon;
    private static Asset<Texture2D> _pyroxeneIcon;
    private static Asset<Texture2D> _settingsButton;
    private static Asset<Texture2D> _studentLeft;
    private static Asset<Texture2D> _studentRight;
    private static Asset<Texture2D> _touchEffect;
    private static Asset<Texture2D> _touchParticle;
    private static Asset<Texture2D> _mainNotification;

    private static Asset<Texture2D> _formationBackground;
    private static Asset<Texture2D> _formationSelectionBackground;
    private static Asset<Texture2D> _formationTile;
    private static Asset<Texture2D> _formationTileBackgrounds;
    private static Asset<Texture2D> _formationAttackTypes;
    private static Asset<Texture2D> _formationRoles;
    private static Asset<Texture2D> _formationNameplateBackground;
    private static Asset<Texture2D> _formationInfoBackground;
    private static Asset<Texture2D> _formationSelectButton;
    private static Asset<Texture2D> _formationClearButton;
    private static Asset<Texture2D> _formationSortButton;
    private static Asset<Texture2D> _formationSwitchButton;
    private static Asset<Texture2D> _formationFilterButton;
    private static Asset<Texture2D> _formationStudentButton;
    private static Asset<Texture2D> _formationOutfitButton;
    private static Asset<Texture2D> _formationBond;
    private static readonly Asset<Texture2D>[] _studentThumbnails = new Asset<Texture2D>[6];
    private static readonly Dictionary<TabletStudentId, Asset<Texture2D>> _studentPortraitBase = new();
    private static readonly Dictionary<TabletStudentId, Asset<Texture2D>> _studentPortraitFace = new();
    private static readonly Dictionary<TabletStudentId, Asset<Texture2D>> _studentPortraitHalo = new();
    private static Asset<DynamicSpriteFont> _uiFont;
    private static Asset<DynamicSpriteFont> _uiFontBold;
    private static Asset<Texture2D> _digitsSmall;
    private static readonly Asset<Texture2D>[] _classroomLayers = new Asset<Texture2D>[13];
    private static readonly Asset<Texture2D>[] _categoryIcons = new Asset<Texture2D>[10];

    public override void Load()
    {
        _instance = this;
        _tabletInterface = new UserInterface();
        _tabletState = new TabletSurfaceState();
        _tabletState.Activate();

        const AssetRequestMode mode = AssetRequestMode.ImmediateLoad;
        _frame = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/TabletFrameOpen", mode);
        _homeButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/TabletHomeButton", mode);
        _mainBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainBackground", mode);
        _shopBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ShopBackground", mode);
        _shopSoraForeground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ShopSoraForeground", mode);
        _shopSoraSpeechBubble = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ShopSoraSpeechBubble", mode);
        _topBar = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/TopBar", mode);
        _returnButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ReturnButton", mode);
        _mainShop = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainShop", mode);
        _mainFormation = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainFormation", mode);
        _mainMission = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainMission", mode);
        _mainTravel = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainTravel", mode);
        _mainArchive = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainArchive", mode);
        _mainWardrobe = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainWardrobe", mode);
        _mainUpgrade = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainUpgrade", mode);
        _mainMessage = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainMessage", mode);
        _shopPanel = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ShopPanel", mode);
        _productCard = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ProductCard", mode);
        _purchaseButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/PurchaseButton", mode);
        _productPlus = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ProductPlus", mode);
        _productMinus = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/ProductMinus", mode);
        _currencyPanel = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CurrencyPanel", mode);
        _cartButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CartButton", mode);
        _cartEntry = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CartEntryBackground", mode);
        _cartPlus = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CartPlus", mode);
        _cartMinus = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CartMinus", mode);
        _cartRemove = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CartRemove", mode);
        _checkoutBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CheckoutBackground", mode);
        _checkoutPanel = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CheckoutPanel", mode);
        _checkoutCancel = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CheckoutCancel", mode);
        _checkoutConfirm = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CheckoutConfirm", mode);
        _checkoutCheckbox = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/CheckoutCheckbox", mode);
        _creditIcon = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Currency/UnionCredit", mode);
        _pyroxeneIcon = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Currency/Pyroxene", mode);
        _settingsButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/SettingsButton", mode);
        _studentLeft = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/StudentLeft", mode);
        _studentRight = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/StudentRight", mode);
        _touchEffect = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/TouchEffect", mode);
        _touchParticle = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/TouchParticle", mode);
        _mainNotification = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/UI/MainNotification", mode);

        _formationBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_Background", mode);
        _formationSelectionBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_SelectionBackground", mode);
        _formationTile = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/UIStudentTile_Tile", mode);
        _formationTileBackgrounds = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/UIStudentTile_Backgrounds", mode);
        _formationAttackTypes = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/UIStudentTile_AttackTypes", mode);
        _formationRoles = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/UIStudentTile_Roles", mode);
        _formationNameplateBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_NameplateBackground", mode);
        _formationInfoBackground = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_InfoBackground", mode);
        _formationSelectButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_SelectButton", mode);
        _formationClearButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_ClearButton", mode);
        _formationSortButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_SortButton", mode);
        _formationSwitchButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_SwitchButton", mode);
        _formationFilterButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_FilterButton", mode);
        _formationStudentButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_StudentButton", mode);
        _formationOutfitButton = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_OutfitButton", mode);
        _formationBond = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Formation/FormationLayer_Bond", mode);

        for (int i = 0; i < TabletStudentCatalog.Students.Count; i++)
        {
            TabletStudentInfo student = TabletStudentCatalog.Students[i];
            _studentThumbnails[i] = ModContent.Request<Texture2D>(student.ThumbnailPath, mode);
            if (!student.HasSimpleLayeredPortrait)
                continue;

            _studentPortraitBase[student.Id] = ModContent.Request<Texture2D>(student.PortraitBasePath, mode);
            _studentPortraitFace[student.Id] = ModContent.Request<Texture2D>(student.PortraitFacePath, mode);
            _studentPortraitHalo[student.Id] = ModContent.Request<Texture2D>(student.PortraitHaloPath, mode);
        }

        _uiFont = ModContent.Request<DynamicSpriteFont>("KivotosMod/Assets/Tablet/Fonts/NotoSans", mode);
        _uiFontBold = ModContent.Request<DynamicSpriteFont>("KivotosMod/Assets/Tablet/Fonts/NotoSansBold", mode);
        _digitsSmall = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Fonts/UIDigitsSmall", mode);

        string[] classroomNames =
        {
            "Sky", "Sea", "Ground", "Wall", "Lockers", "Board", "Light",
            "ChairBack", "TableBack", "ChairMid", "TableMid", "ChairFront", "TableFront"
        };
        for (int i = 0; i < classroomNames.Length; i++)
            _classroomLayers[i] = ModContent.Request<Texture2D>($"KivotosMod/Assets/Tablet/UI/Classroom/{classroomNames[i]}", mode);

        string[] categoryNames =
        {
            // Keep the exact category order used by the legacy Kivotos-Student shop.
            "All", "Weapons", "Tools", "Armor", "Equipment",
            "Consumables", "Ammunition", "Tiles", "Materials", "Other"
        };
        for (int i = 0; i < categoryNames.Length; i++)
            _categoryIcons[i] = ModContent.Request<Texture2D>($"KivotosMod/Assets/Tablet/UI/Category{categoryNames[i]}", mode);
    }

    public override void Unload()
    {
        Visible = false;
        _holdTimer = 0;
        _settingsOpen = false;
        _pageTransitionElapsed = -1;
        TouchBursts.Clear();
        TouchSparks.Clear();
        TouchTrail.Clear();
        _lastTouchDragPoint = null;
        _touchDragRemainder = 0f;
        _tabletInterface?.SetState(null);
        _tabletInterface = null;
        _tabletState = null;
        _instance = null;
        TabletCart.Clear();
        ProductQuantities.Clear();
        _frame = _homeButton = _mainBackground = _shopBackground = _topBar = _returnButton = null;
        _shopSoraForeground = _shopSoraSpeechBubble = null;
        _mainShop = _mainFormation = _mainMission = _mainTravel = _mainArchive = null;
        _mainWardrobe = _mainUpgrade = _mainMessage = null;
        _shopPanel = _productCard = _purchaseButton = _productPlus = _productMinus = null;
        _currencyPanel = _cartButton = _cartEntry = _cartPlus = _cartMinus = _cartRemove = null;
        _checkoutBackground = _checkoutPanel = _checkoutCancel = _checkoutConfirm = _checkoutCheckbox = null;
        _creditIcon = _pyroxeneIcon = _settingsButton = _studentLeft = _studentRight = null;
        _touchEffect = _touchParticle = _mainNotification = null;
        _formationBackground = _formationSelectionBackground = _formationTile = _formationTileBackgrounds = null;
        _formationAttackTypes = _formationRoles = _formationNameplateBackground = _formationInfoBackground = null;
        _formationSelectButton = _formationClearButton = null;
        _formationSortButton = _formationSwitchButton = _formationFilterButton = null;
        _formationStudentButton = _formationOutfitButton = _formationBond = null;
        TabletDynamicPortraitRenderer.Unload();
        for (int i = 0; i < _studentThumbnails.Length; i++)
            _studentThumbnails[i] = null;
        _studentPortraitBase.Clear();
        _studentPortraitFace.Clear();
        _studentPortraitHalo.Clear();
        _uiFont = _uiFontBold = null;
        _digitsSmall = null;
        for (int i = 0; i < _classroomLayers.Length; i++)
            _classroomLayers[i] = null;
        for (int i = 0; i < _categoryIcons.Length; i++)
            _categoryIcons[i] = null;
    }

    public static void Toggle()
    {
        if (Visible)
            ForceClose();
        else
            Open();
    }

    public static void Open()
    {
        if (Visible)
            return;

        Visible = true;
        _holdTimer = 0;
        _instance?._tabletInterface?.SetState(_instance._tabletState);
        _draggingScrollbar = false;
        _checkoutPending = false;
        _settingsOpen = false;
        _leftWasDown = Main.mouseLeft;
        _escapeWasDown = false;
        _pageTransitionElapsed = -1;
        TouchBursts.Clear();
        TouchSparks.Clear();
        TouchTrail.Clear();
        _lastTouchDragPoint = null;
        _touchDragRemainder = 0f;

        // Vanilla inventory owns the inventory key/state.  Never force it closed here.
        // If the tablet is moved from the backpack into the hotbar while the inventory is open,
        // UpdateUI keeps the tablet suspended until the inventory is closed, then opens normally.
    }

    // Hard close is reserved for leaving the world/menu.  Normal hotbar deselection is handled
    // by UpdateUI and animates _holdTimer back to zero before the UIState is detached.
    public static void Close() => ForceClose();

    private static void ForceClose()
    {
        _holdTimer = 0;
        FinishClose();
    }

    private static void FinishClose()
    {
        Visible = false;
        _instance?._tabletInterface?.SetState(null);
        _draggingScrollbar = false;
        _checkoutOpen = false;
        _checkoutPending = false;
        _settingsOpen = false;
        _pageTransitionElapsed = -1;
        TouchBursts.Clear();
        TouchSparks.Clear();
        TouchTrail.Clear();
        _lastTouchDragPoint = null;
        _touchDragRemainder = 0f;
    }

    public static void HandleCheckoutResult(bool success, string message)
    {
        _checkoutPending = false;
        if (success)
        {
            TabletCart.Clear();
            _checkoutOpen = false;
            _showingCart = false;
            _cartScrollIndex = 0;
        }

        if (!string.IsNullOrWhiteSpace(message) && Main.netMode != NetmodeID.Server)
            Main.NewText(message, success ? new Color(120, 220, 255) : new Color(255, 120, 120));
    }

    public override void UpdateUI(GameTime gameTime)
    {
        if (Main.gameMenu)
        {
            if (Visible)
                ForceClose();
            return;
        }

        Player player = Main.LocalPlayer;

        // The vanilla backpack/inventory takes priority over the tablet UI.  This is important
        // for a tablet stored outside the hotbar: the player must be able to open the backpack,
        // drag it into a hotbar slot, rearrange items, or move it away again without our UI
        // consuming mouse input or immediately closing Main.playerInventory.
        //
        // If the tablet was already open, opening the backpack temporarily puts it away.  When
        // the backpack closes and the tablet is still the selected held item, the normal
        // take-out animation starts again from frame zero.
        if (Main.playerInventory)
        {
            if (Visible)
                ForceClose();

            // Keep edge-trigger state in sync while vanilla owns the mouse/escape key so the
            // first click after closing the inventory cannot leak through into a tablet button.
            _leftWasDown = Main.mouseLeft;
            _escapeWasDown = Main.keyState.IsKeyDown(Keys.Escape);
            return;
        }

        int tabletType = ModContent.ItemType<ShittimTablet>();
        bool holdingTablet = player != null
            && player.active
            && !player.dead
            && player.HeldItem != null
            && player.HeldItem.type == tabletType;

        if (holdingTablet && !Visible)
            Open();

        if (!Visible)
            return;

        // The held item drives a reversible presentation timer.  Switching slots does not
        // destroy the UI instantly; it retracts first, exactly like putting the device away.
        _holdTimer = Math.Clamp(_holdTimer + (holdingTablet ? 1 : -1), 0, OpenCloseTicks);

        if (!holdingTablet && _holdTimer <= 0)
        {
            FinishClose();
            return;
        }

        UpdatePageTransition();

        // Let tModLoader establish the exact UI coordinate space first. The tablet then reads
        // its full-screen UIState dimensions for both layout and hit-testing.
        _tabletInterface?.Update(gameTime);

        Layout layout = GetLayout();
        Vector2 mouseUi = GetUiMousePosition();
        Vector2 mouseTablet = layout.ToTablet(mouseUi);
        Point p = mouseTablet.ToPoint();
        bool insideTablet = new Rectangle(0, 0, TabletWidth, TabletHeight).Contains(p);
        bool inputEnabled = holdingTablet && FullyOpened && !PageTransitionActive;

        if (insideTablet && holdingTablet)
        {
            player.mouseInterface = true;
            PlayerInput.LockVanillaMouseScroll("KivotosMod:Tablet");
        }

        bool leftDown = Main.mouseLeft;
        bool clicked = leftDown && !_leftWasDown;
        bool released = !leftDown && _leftWasDown;
        _leftWasDown = leftDown;

        UpdateTouchEffects(
            inputEnabled && leftDown && DisplayRect.Contains(p),
            inputEnabled && clicked && DisplayRect.Contains(p),
            mouseTablet);

        bool escapeDown = Main.keyState.IsKeyDown(Keys.Escape);
        if (inputEnabled && escapeDown && !_escapeWasDown)
        {
            if (_settingsOpen)
            {
                _settingsOpen = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else
            {
                HandleBack();
            }
        }
        _escapeWasDown = escapeDown;

        if (!inputEnabled)
        {
            _draggingScrollbar = false;
            return;
        }

        if (_page == TabletPage.Shop && insideTablet && !_checkoutOpen && !_settingsOpen)
        {
            int wheel = PlayerInput.ScrollWheelDeltaForUI;
            if (wheel != 0)
            {
                ScrollRows(wheel > 0 ? -1 : 1);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }

        // Dragging the scrollbar is handled continuously, not only on click edges.
        if (_page == TabletPage.Shop && !_checkoutOpen && !_settingsOpen && _draggingScrollbar)
        {
            if (leftDown)
            {
                SetScrollFromMouse(p.Y);
                player.mouseInterface = true;
            }
            else
            {
                _draggingScrollbar = false;
            }
        }

        if (released)
            _draggingScrollbar = false;

        if (!clicked || !insideTablet)
            return;

        // The gear is available from every tablet page.  Settings is modal: while it is open,
        // clicks cannot leak through to HOME/Shop/Formation controls underneath it.
        if (SettingsButtonRect.Contains(p))
        {
            _settingsOpen = !_settingsOpen;
            _draggingScrollbar = false;
            SoundEngine.PlaySound(_settingsOpen ? SoundID.MenuOpen : SoundID.MenuClose);
            return;
        }

        if (_settingsOpen)
        {
            TabletUiPreferencesPlayer prefs = player.GetModPlayer<TabletUiPreferencesPlayer>();

            if (SettingsBackRect.Contains(p))
            {
                _settingsOpen = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
                return;
            }

            if (SettingsScaleDownRect.Contains(p))
            {
                prefs.ChangeScale(-1);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            else if (SettingsScaleUpRect.Contains(p))
            {
                prefs.ChangeScale(1);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            else if (SettingsResetRect.Contains(p))
            {
                prefs.ResetScale();
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            return;
        }

        if (HomeButtonRect.Contains(p))
        {
            if (_page != TabletPage.Home)
            {
                _showingCart = false;
                _checkoutOpen = false;
                RequestPage(TabletPage.Home);
            }
            return;
        }

        if (_page == TabletPage.Home)
        {
            // HOME portrait switching is independent from Formation/player appearance.
            if (HomeStudentLeftRect.Contains(p))
            {
                player.GetModPlayer<TabletCompanionPlayer>().Cycle(-1);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            if (HomeStudentRightRect.Contains(p))
            {
                player.GetModPlayer<TabletCompanionPlayer>().Cycle(1);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            if (HomeShop.Contains(p))
            {
                _shopScrollRow = 0;
                _selectedCategory = TabletShopCategory.All;
                _showingCart = false;
                RequestPage(TabletPage.Shop);
            }
            else if (HomeFormation.Contains(p))
            {
                TabletStudentAppearancePlayer appearance = player.GetModPlayer<TabletStudentAppearancePlayer>();
                _selectedStudentIndex = appearance.HasSelection
                    ? TabletStudentCatalog.FormationIndexOf(appearance.SelectedStudent)
                    : 0;
                RequestPage(TabletPage.Formation);
            }
            return;
        }

        if (ReturnRect.Contains(p))
        {
            HandleBack();
            return;
        }

        if (_page == TabletPage.Formation)
        {
            // The old selection header is present for visual parity. Queue/team mode remains
            // disabled in this branch, so these controls intentionally do not open another page.
            if (FormationSortRect.Contains(p) || FormationSwitchRect.Contains(p) || FormationFilterRect.Contains(p)
                || FormationStudentButtonRect.Contains(p) || FormationOutfitButtonRect.Contains(p))
            {
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            for (int i = 0; i < TabletStudentCatalog.FormationStudents.Count; i++)
            {
                if (!GetFormationTileRect(i).Contains(p))
                    continue;

                _selectedStudentIndex = i;
                player.GetModPlayer<TabletStudentAppearancePlayer>().Select(TabletStudentCatalog.FormationStudents[i].Id);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }

            if (FormationClearRect.Contains(p))
            {
                player.GetModPlayer<TabletStudentAppearancePlayer>().ClearSelection();
                SoundEngine.PlaySound(SoundID.MenuClose);
                return;
            }

            if (FormationSelectRect.Contains(p))
            {
                TabletStudentInfo student = TabletStudentCatalog.FormationStudents[Math.Clamp(_selectedStudentIndex, 0, TabletStudentCatalog.FormationStudents.Count - 1)];
                player.GetModPlayer<TabletStudentAppearancePlayer>().Select(student.Id);
                SoundEngine.PlaySound(SoundID.MenuOpen);
                return;
            }

            return;
        }

        for (int i = 0; i < _categoryIcons.Length; i++)
        {
            Rectangle categoryRect = GetCategoryRect(i);
            if (!categoryRect.Contains(p))
                continue;

            _selectedCategory = (TabletShopCategory)i;
            _showingCart = false;
            _checkoutOpen = false;
            _checkoutPending = false;
            _shopScrollRow = 0;
            SoundEngine.PlaySound(SoundID.MenuTick);
            return;
        }

        if (_checkoutOpen)
        {
            if (CheckoutCancelRect.Contains(p) && !_checkoutPending)
            {
                _checkoutOpen = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
            else if (CheckoutConfirmRect.Contains(p) && !_checkoutPending && !TabletCart.IsEmpty)
            {
                _checkoutPending = true;
                SoundEngine.PlaySound(SoundID.MenuOpen);
                TabletCheckoutService.RequestCheckout(TabletCart.Snapshot());
            }
            return;
        }

        if (ShopCartRect.Contains(p))
        {
            if (!_showingCart)
            {
                _showingCart = true;
                _cartScrollIndex = 0;
            }
            else if (!TabletCart.IsEmpty)
            {
                _checkoutOpen = true;
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
            return;
        }

        if (ShopScrollTrackRect.Contains(p))
        {
            _draggingScrollbar = true;
            SetScrollFromMouse(p.Y);
            SoundEngine.PlaySound(SoundID.MenuTick);
            return;
        }

        if (_showingCart)
            HandleCartClick(p);
        else
            HandleProductClick(p);
    }

    private static void HandleBack()
    {
        if (_checkoutOpen)
        {
            if (!_checkoutPending)
                _checkoutOpen = false;
            return;
        }
        if (_showingCart)
        {
            _showingCart = false;
            return;
        }
        if (_page == TabletPage.Shop || _page == TabletPage.Formation)
        {
            // Formation selection is persistent player state. Returning home must never clear it.
            RequestPage(TabletPage.Home);
            return;
        }

        // On the home screen the physical home/escape action does not put the item away.
        // The tablet closes when the player actually stops holding it.
    }

    private static void RequestPage(TabletPage target)
    {
        if (target == _page || PageTransitionActive)
            return;

        _pageTransitionTarget = target;
        _pageTransitionElapsed = 0;
        SoundEngine.PlaySound(SoundID.MenuOpen);
    }

    private static void UpdatePageTransition()
    {
        if (!PageTransitionActive)
            return;

        _pageTransitionElapsed++;

        int switchTick = PageFadeTicks + PageHoldTicks;
        if (_pageTransitionElapsed == switchTick)
            _page = _pageTransitionTarget;

        if (_pageTransitionElapsed >= PageTransitionTicks)
            _pageTransitionElapsed = -1;
    }

    private static void UpdateTouchEffects(bool draggingOnDisplay, bool pressedOnDisplay, Vector2 mouseTablet)
    {
        for (int i = TouchBursts.Count - 1; i >= 0; i--)
        {
            TouchBursts[i].Update();
            if (TouchBursts[i].Expired)
                TouchBursts.RemoveAt(i);
        }

        for (int i = TouchSparks.Count - 1; i >= 0; i--)
        {
            TouchSparks[i].Update();
            if (TouchSparks[i].Expired)
                TouchSparks.RemoveAt(i);
        }

        if (pressedOnDisplay)
            TouchBursts.Add(new TouchBurst(mouseTablet));

        if (draggingOnDisplay)
        {
            if (_lastTouchDragPoint.HasValue)
            {
                Vector2 from = _lastTouchDragPoint.Value;
                Vector2 delta = mouseTablet - from;
                float distance = delta.Length();
                const float spacing = 24f;
                float travelled = _touchDragRemainder + distance;
                int spawnCount = (int)(travelled / spacing);

                for (int i = 1; i <= spawnCount; i++)
                {
                    float along = distance <= 0.001f
                        ? 1f
                        : MathHelper.Clamp((i * spacing - _touchDragRemainder) / distance, 0f, 1f);
                    TouchSparks.Add(new TouchSpark(Vector2.Lerp(from, mouseTablet, along)));
                }

                _touchDragRemainder = travelled - spawnCount * spacing;
            }
            else
            {
                _touchDragRemainder = 0f;
            }

            _lastTouchDragPoint = mouseTablet;
            if (TouchTrail.Count == 0 || Vector2.DistanceSquared(TouchTrail[^1], mouseTablet) >= 1f)
                TouchTrail.Add(mouseTablet);
            while (TouchTrail.Count > 12)
                TouchTrail.RemoveAt(0);
        }
        else
        {
            _lastTouchDragPoint = null;
            _touchDragRemainder = 0f;
            if (TouchTrail.Count > 0)
                TouchTrail.RemoveAt(0);
        }
    }

    private static void HandleProductClick(Point p)
    {
        IReadOnlyList<TabletShopProduct> products = TabletShopCatalog.GetProducts(_selectedCategory);
        int start = _shopScrollRow * 2;
        for (int i = 0; i < 4; i++)
        {
            int index = start + i;
            if (index >= products.Count)
                break;

            TabletShopProduct product = products[index];
            Rectangle card = GetShopCardRect(i);
            Rectangle minus = new(card.X + 8, card.Y + 82, 14, 16);
            Rectangle plus = new(card.X + 86, card.Y + 82, 14, 16);
            Rectangle add = new(card.X + 8, card.Y + 100, 92, 20);

            int quantity = GetSelectedQuantity(product.ItemType);
            if (minus.Contains(p))
            {
                ProductQuantities[product.ItemType] = Math.Max(1, quantity - 1);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }
            if (plus.Contains(p))
            {
                ProductQuantities[product.ItemType] = Math.Min(9999, quantity + 1);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }
            if (add.Contains(p))
            {
                long after = TabletCart.GetTotalCredits() + (long)product.PriceCredits * quantity;
                if (after > TabletWallet.GetTotalCredits(Main.LocalPlayer))
                {
                    Main.NewText("余额不足，无法加入这么多商品。", new Color(255, 120, 120));
                    return;
                }

                TabletCart.Add(product.ItemType, quantity);
                ProductQuantities[product.ItemType] = 1;
                SoundEngine.PlaySound(SoundID.Grab);
                return;
            }
        }
    }

    private static void HandleCartClick(Point p)
    {
        List<TabletCartLine> lines = TabletCart.Snapshot();
        int start = _cartScrollIndex;
        for (int i = 0; i < 5; i++)
        {
            int index = start + i;
            if (index >= lines.Count)
                break;

            TabletCartLine line = lines[index];
            Rectangle row = GetCartRowRect(i);
            Rectangle remove = new(row.X + 200, row.Y + 6, 14, 16);
            Rectangle minus = new(row.X + 124, row.Y + 28, 14, 16);
            Rectangle plus = new(row.X + 184, row.Y + 28, 14, 16);

            if (remove.Contains(p))
            {
                TabletCart.Remove(line.ItemType);
                NormalizeCartScroll();
                SoundEngine.PlaySound(SoundID.MenuClose);
                return;
            }
            if (minus.Contains(p))
            {
                if (line.Quantity > 1)
                    TabletCart.Adjust(line.ItemType, -1);
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }
            if (plus.Contains(p) && TabletShopCatalog.TryGetByItemType(line.ItemType, out TabletShopProduct product))
            {
                long after = TabletCart.GetTotalCredits() + product.PriceCredits;
                if (after <= TabletWallet.GetTotalCredits(Main.LocalPlayer))
                    TabletCart.Adjust(line.ItemType, 1);
                else
                    Main.NewText("余额不足。", new Color(255, 120, 120));
                SoundEngine.PlaySound(SoundID.MenuTick);
                return;
            }
        }
    }

    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (index < 0)
            index = layers.Count;

        layers.Insert(index, new LegacyGameInterfaceLayer(
            "KivotosMod: Tablet",
            delegate
            {
                // Inventory is a vanilla modal surface.  Even if another system toggles it
                // after UpdateUI ran this frame, never draw the tablet over the backpack.
                if (Visible && !Main.gameMenu && !Main.playerInventory)
                    _tabletInterface?.Draw(Main.spriteBatch, new GameTime());
                return true;
            },
            InterfaceScaleType.UI));
    }

    private static void DrawTabletSurface(SpriteBatch sb)
    {
        if (!Visible || Main.gameMenu)
            return;

        Layout layout = GetLayout();
        Vector2 mouseTablet = layout.ToTablet(GetUiMousePosition());

        if (_page == TabletPage.Shop)
            DrawTexture(sb, _shopBackground.Value, DisplayRect, layout);
        else if (_page == TabletPage.Formation)
            DrawNativeCentered(sb, _formationBackground.Value, DisplayRect, layout, new Rectangle(130, 2, 514, 386));
        else
            DrawClassroomBackground(sb, layout);

        if (_page == TabletPage.Home)
            DrawHome(sb, layout, mouseTablet);
        else if (_page == TabletPage.Shop)
            DrawShop(sb, layout, mouseTablet);
        else
            DrawFormation(sb, layout, mouseTablet);

        DrawTopBar(sb, layout, mouseTablet);

        // Return sits above the top bar so the bar can never cover it.
        if (_page == TabletPage.Shop || _page == TabletPage.Formation)
            DrawReturnButton(sb, layout, mouseTablet);

        if (_settingsOpen)
            DrawSettingsOverlay(sb, layout, mouseTablet);

        DrawPageTransitionOverlay(sb, layout);
        DrawTouchFeedback(sb, layout);
        DrawPowerOverlay(sb, layout);
        DrawFrame(sb, layout, mouseTablet);
    }

    private static void DrawHome(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        // The legacy home screen always had a live companion portrait in the left half.
        // It is drawn before the buttons so the right-side HOME controls remain on top.
        TabletCompanionPlayer companion = Main.LocalPlayer.GetModPlayer<TabletCompanionPlayer>();
        TabletStudentInfo companionStudent = TabletStudentCatalog.Get(companion.Companion);
        DrawTabletPortrait(sb, companionStudent, HomePortraitRect, layout, mouseTablet,
            HomePortraitScale, HomePortraitRootOffset, DisplayRect);

        DrawHomeButton(sb, _mainShop.Value, HomeShop, "Shop", true, mouseTablet, layout);
        DrawHomeButton(sb, _mainFormation.Value, HomeFormation, "Formation", true, mouseTablet, layout);
        DrawHomeButton(sb, _mainTravel.Value, HomeTravel, "Travel", false, mouseTablet, layout);
        DrawHomeButton(sb, _mainMessage.Value, HomeMessage, "Message", false, mouseTablet, layout);
        DrawHomeButton(sb, _mainMission.Value, HomeMission, "Mission", false, mouseTablet, layout);
        DrawHomeButton(sb, _mainWardrobe.Value, HomeWardrobe, "Wardrobe", false, mouseTablet, layout);
        DrawHomeButton(sb, _mainUpgrade.Value, HomeUpgrade, "Upgrade", false, mouseTablet, layout);
        DrawHomeButton(sb, _mainArchive.Value, HomeArchive, "Archive", false, mouseTablet, layout);

        // Notification red dots are intentionally disabled for now.

        DrawStudentSwitchButton(sb, _studentLeft.Value, HomeStudentLeftRect, HomeStudentLeftRect.Contains(mouseTablet.ToPoint()), layout);
        DrawStudentSwitchButton(sb, _studentRight.Value, HomeStudentRightRect, HomeStudentRightRect.Contains(mouseTablet.ToPoint()), layout);
    }

    private static void DrawFormation(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        // Direct FIX6-style Unit Formation picker. The team/loadout/queue screen is
        // intentionally bypassed, but the selection composition itself follows the
        // authored positions and native texture overhangs.
        DrawLegacyThreeFrameButton(sb, _formationSortButton.Value, FormationSortRect,
            FormationSortRect.Contains(mouseTablet.ToPoint()), layout);
        DrawLegacyThreeFrameButton(sb, _formationSwitchButton.Value, FormationSwitchRect,
            FormationSwitchRect.Contains(mouseTablet.ToPoint()), layout);
        DrawLegacyThreeFrameButton(sb, _formationFilterButton.Value, FormationFilterRect,
            FormationFilterRect.Contains(mouseTablet.ToPoint()), layout);
        DrawText(sb, "Name", new Vector2(FormationSortRect.X + 20, FormationSortRect.Y + 3),
            Color.White, 0.38f, layout, bold: true);

        DrawNativeCentered(sb, _formationSelectionBackground.Value, FormationSelectionBackgroundRect, layout);

        for (int i = 0; i < TabletStudentCatalog.FormationStudents.Count; i++)
            DrawFormationStudentTile(sb, i, layout, mouseTablet);

        TabletStudentInfo selected = TabletStudentCatalog.FormationStudents[
            Math.Clamp(_selectedStudentIndex, 0, TabletStudentCatalog.FormationStudents.Count - 1)];

        DrawLegacyThreeFrameButton(sb, _formationStudentButton.Value, FormationStudentButtonRect,
            FormationStudentButtonRect.Contains(mouseTablet.ToPoint()), layout);
        DrawLegacyThreeFrameButton(sb, _formationOutfitButton.Value, FormationOutfitButtonRect,
            FormationOutfitButtonRect.Contains(mouseTablet.ToPoint()), layout);

        DrawTabletPortrait(sb, selected, FormationPortraitRect, layout, mouseTablet,
            1.0f, FormationPortraitRootOffset, DisplayRect);

        DrawNativeCentered(sb, _formationInfoBackground.Value, FormationInfoRect, layout);
        DrawNativeCentered(sb, _formationNameplateBackground.Value, FormationNameplateRect, layout);

        // Legacy info strip: damage type on the left, bond in the middle, role on the right.
        int attackFrameWidth = _formationAttackTypes.Value.Width / 6;
        int roleFrameWidth = _formationRoles.Value.Width / 6;
        DrawTexture(sb, _formationAttackTypes.Value, new Rectangle(FormationInfoRect.X + 4, FormationInfoRect.Y + 6, 20, 20), layout,
            new Rectangle(selected.AttackTypeFrame * attackFrameWidth, 0, attackFrameWidth, _formationAttackTypes.Value.Height));
        // FormationLayer_Bond is a two-frame sheet: frame 0 is a broken-heart placeholder,
        // frame 1 is the actual bond heart.  Drawing the whole 68px sheet squeezed both states
        // together, which is why V12 showed a cluster of "three hearts".
        int bondFrameWidth = _formationBond.Value.Width / 2;
        DrawTexture(sb, _formationBond.Value,
            new Rectangle(FormationInfoRect.X + 34, FormationInfoRect.Y + 2, 32, 24),
            layout,
            new Rectangle(bondFrameWidth, 0, bondFrameWidth, _formationBond.Value.Height));
        DrawTexture(sb, _formationRoles.Value, new Rectangle(FormationInfoRect.X + 64, FormationInfoRect.Y + 6, 20, 20), layout,
            new Rectangle(selected.RoleFrame * roleFrameWidth, 0, roleFrameWidth, _formationRoles.Value.Height));

        DrawCenteredText(sb, selected.DisplayName,
            new Vector2(FormationNameplateRect.Center.X, FormationNameplateRect.Y + 6),
            Color.White, 0.39f, layout);

        bool clearHover = FormationClearRect.Contains(mouseTablet.ToPoint());
        bool selectHover = FormationSelectRect.Contains(mouseTablet.ToPoint());
        DrawFormationButton(sb, _formationClearButton.Value, FormationClearRect, clearHover, layout);
        DrawFormationButton(sb, _formationSelectButton.Value, FormationSelectRect, selectHover, layout);
        DrawCenteredText(sb, "Select", new Vector2(FormationSelectRect.Center.X, FormationSelectRect.Y + 3),
            Color.White, 0.36f, layout);
    }

    private static void DrawFormationStudentTile(SpriteBatch sb, int index, Layout layout, Vector2 mouseTablet)
    {
        Rectangle tile = GetFormationTileRect(index);
        TabletStudentInfo student = TabletStudentCatalog.FormationStudents[index];

        int factionFrame = Math.Max(0, student.LegacyFactionFrame);
        int backgroundFrameWidth = _formationTileBackgrounds.Value.Width / 10;
        int backgroundFrameHeight = _formationTileBackgrounds.Value.Height / 2;
        int backgroundX = factionFrame % 10;
        int backgroundY = factionFrame / 10;
        DrawTexture(sb, _formationTileBackgrounds.Value, new Rectangle(tile.X, tile.Y, 72, 80), layout,
            new Rectangle(backgroundX * backgroundFrameWidth, backgroundY * backgroundFrameHeight,
                backgroundFrameWidth, backgroundFrameHeight));

        int thumbnailIndex = TabletStudentCatalog.IndexOf(student.Id);
        DrawTexture(sb, _studentThumbnails[thumbnailIndex].Value, new Rectangle(tile.X + 2, tile.Y + 2, 68, 76), layout);

        bool hover = tile.Contains(mouseTablet.ToPoint());
        bool selected = _selectedStudentIndex == index;
        int tileFrameWidth = _formationTile.Value.Width / 3;
        int tileFrame = hover ? 2 : (selected ? 1 : 0);
        DrawTexture(sb, _formationTile.Value, new Rectangle(tile.X - 2, tile.Y - 2, 76, 106), layout,
            new Rectangle(tileFrame * tileFrameWidth, 0, tileFrameWidth, _formationTile.Value.Height));

        int attackFrameWidth = _formationAttackTypes.Value.Width / 6;
        int roleFrameWidth = _formationRoles.Value.Width / 6;
        DrawTexture(sb, _formationAttackTypes.Value, new Rectangle(tile.X + 4, tile.Y + 4, 24, 24), layout,
            new Rectangle(student.AttackTypeFrame * attackFrameWidth, 0, attackFrameWidth, _formationAttackTypes.Value.Height));
        DrawTexture(sb, _formationRoles.Value, new Rectangle(tile.X + 4, tile.Y + 4, 24, 24), layout,
            new Rectangle(student.RoleFrame * roleFrameWidth, 0, roleFrameWidth, _formationRoles.Value.Height));

        DrawCenteredText(sb, student.GivenName, new Vector2(tile.Center.X, tile.Y + 82),
            Color.White, 0.31f, layout);
    }

    private static void DrawTabletPortrait(
        SpriteBatch sb,
        TabletStudentInfo student,
        Rectangle bounds,
        Layout layout,
        Vector2 mouseTablet,
        float portraitScale,
        float rootYOffset = 0f,
        Rectangle? clipBounds = null)
    {
        // Arona and Sora use the new independent bone rig: body joints, secondary hair/cloth
        // motion, blinking and mouse-following eyes are solved every frame.
        if (TabletDynamicPortraitRenderer.Draw(
            sb,
            student.Id,
            layout.ToScreen,
            layout.Scale,
            bounds,
            mouseTablet,
            portraitScale,
            rootYOffset,
            clipBounds))
        {
            return;
        }

        // Yuuka/Hoshino/Shiroko keep their compact layered portrait assets. This path is also
        // used by HOME, so switching companions does not depend on any Formation state.
        if (student.HasSimpleLayeredPortrait
            && _studentPortraitBase.TryGetValue(student.Id, out Asset<Texture2D> baseAsset)
            && _studentPortraitFace.TryGetValue(student.Id, out Asset<Texture2D> faceAsset)
            && _studentPortraitHalo.TryGetValue(student.Id, out Asset<Texture2D> haloAsset))
        {
            Vector2 rootTablet = new(bounds.Center.X, bounds.Bottom - 2 + rootYOffset);

            Texture2D baseTexture = baseAsset.Value;
            int baseFrameWidth = baseTexture.Width / Math.Max(1, student.BaseFrameCount);
            Rectangle baseSource = new(0, 0, baseFrameWidth, baseTexture.Height);
            Vector2 basePosition = rootTablet + student.BaseOffset * portraitScale;
            DrawSimplePortraitPart(sb, baseTexture, baseSource, basePosition, student.BaseOrigin,
                portraitScale, layout, clipBounds);

            Texture2D faceTexture = faceAsset.Value;
            int faceFrameWidth = faceTexture.Width / Math.Max(1, student.FaceFrameCount);
            Rectangle faceSource = new(0, 0, faceFrameWidth, faceTexture.Height);
            Vector2 headPosition = rootTablet + (student.BaseOffset + student.HeadOffset) * portraitScale;
            DrawSimplePortraitPart(sb, faceTexture, faceSource, headPosition, student.FaceOrigin,
                portraitScale, layout, clipBounds);

            Texture2D haloTexture = haloAsset.Value;
            Vector2 haloPosition = rootTablet + (student.BaseOffset + student.HeadOffset + student.HaloOffset) * portraitScale;
            DrawSimplePortraitPart(sb, haloTexture, haloTexture.Bounds, haloPosition,
                new Vector2(haloTexture.Width * 0.5f, haloTexture.Height * 0.5f),
                portraitScale, layout, clipBounds);
            return;
        }

        // Plana currently has no complete standalone portrait art in the supplied reference
        // resources. Keep her own thumbnail rather than borrowing another student's rig.
        int index = TabletStudentCatalog.IndexOf(student.Id);
        Rectangle fallback = new(bounds.X + 28, bounds.Y + 26, bounds.Width - 56, bounds.Height - 52);
        DrawTextureFit(sb, _studentThumbnails[index].Value, fallback, layout);
    }

    private static void DrawSimplePortraitPart(
        SpriteBatch sb,
        Texture2D texture,
        Rectangle source,
        Vector2 positionTablet,
        Vector2 origin,
        float portraitScale,
        Layout layout,
        Rectangle? clipBounds)
    {
        if (clipBounds == null)
        {
            sb.Draw(texture, layout.ToScreen(positionTablet), source, Color.White, 0f, origin,
                layout.Scale * portraitScale, SpriteEffects.None, 0f);
            return;
        }

        Rectangle clip = clipBounds.Value;
        float left = positionTablet.X - origin.X * portraitScale;
        float top = positionTablet.Y - origin.Y * portraitScale;
        float width = source.Width * portraitScale;
        float height = source.Height * portraitScale;
        float right = left + width;
        float bottom = top + height;

        float clippedLeft = MathF.Max(left, clip.Left);
        float clippedTop = MathF.Max(top, clip.Top);
        float clippedRight = MathF.Min(right, clip.Right);
        float clippedBottom = MathF.Min(bottom, clip.Bottom);
        if (clippedRight <= clippedLeft || clippedBottom <= clippedTop || width <= 0f || height <= 0f)
            return;

        float u0 = (clippedLeft - left) / width;
        float v0 = (clippedTop - top) / height;
        float u1 = (clippedRight - left) / width;
        float v1 = (clippedBottom - top) / height;

        int sx0 = source.X + Math.Clamp((int)MathF.Floor(source.Width * u0), 0, source.Width - 1);
        int sy0 = source.Y + Math.Clamp((int)MathF.Floor(source.Height * v0), 0, source.Height - 1);
        int sx1 = source.X + Math.Clamp((int)MathF.Ceiling(source.Width * u1), 1, source.Width);
        int sy1 = source.Y + Math.Clamp((int)MathF.Ceiling(source.Height * v1), 1, source.Height);
        if (sx1 <= sx0 || sy1 <= sy0)
            return;

        Rectangle clippedSource = new(sx0, sy0, sx1 - sx0, sy1 - sy0);
        float adjustedLeft = left + (sx0 - source.X) / (float)source.Width * width;
        float adjustedTop = top + (sy0 - source.Y) / (float)source.Height * height;
        float adjustedRight = left + (sx1 - source.X) / (float)source.Width * width;
        float adjustedBottom = top + (sy1 - source.Y) / (float)source.Height * height;

        Rectangle tabletDestination = new(
            (int)MathF.Round(adjustedLeft),
            (int)MathF.Round(adjustedTop),
            Math.Max(1, (int)MathF.Round(adjustedRight - adjustedLeft)),
            Math.Max(1, (int)MathF.Round(adjustedBottom - adjustedTop)));
        sb.Draw(texture, layout.ToScreen(tabletDestination), clippedSource, Color.White);
    }

    private static void DrawStudentSwitchButton(
        SpriteBatch sb,
        Texture2D texture,
        Rectangle bounds,
        bool hover,
        Layout layout)
    {
        int frameWidth = texture.Width / 3;
        int frame = hover ? 1 : 0;
        DrawNativeCentered(sb, texture, bounds, layout,
            new Rectangle(frame * frameWidth, 0, frameWidth, texture.Height));
    }

    private static void DrawLegacyThreeFrameButton(
        SpriteBatch sb,
        Texture2D texture,
        Rectangle bounds,
        bool hover,
        Layout layout)
    {
        int frameWidth = texture.Width / 3;
        int frame = hover ? 1 : 0;
        DrawNativeCentered(sb, texture, bounds, layout,
            new Rectangle(frame * frameWidth, 0, frameWidth, texture.Height));
    }

    private static void DrawFormationButton(SpriteBatch sb, Texture2D texture, Rectangle destination, bool hover, Layout layout)
    {
        // Authored FIX6 buttons include a 4 px overhang around their logical hit boxes.
        // Preserve that native size instead of stretching the texture to the hit rectangle.
        int frames = 3;
        int frameWidth = texture.Width / frames;
        int frame = hover ? 1 : 0;
        DrawNativeCentered(sb, texture, destination, layout,
            new Rectangle(frame * frameWidth, 0, frameWidth, texture.Height));
    }

    private static Rectangle GetFormationTileRect(int index)
    {
        int column = index % 3;
        int row = index / 3;
        return new Rectangle(44 + column * 76, 104 + row * 106, 72, 102);
    }

    private static void DrawShop(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        // The original visual composition has Sora serving as the clerk in the left half of
        // the SHOP page.  This uses our clean-room bone renderer (no NPC/legacy UI classes).
        // Draw her first so the category rail and the shop panel stay legible above her.
        TabletStudentInfo shopClerk = TabletStudentCatalog.Get(TabletStudentId.Sora);
        DrawTabletPortrait(sb, shopClerk, ShopSoraPortraitRect, layout, mouseTablet,
            ShopSoraPortraitScale, ShopSoraPortraitRootOffset, ShopSoraClipRect);

        // Sora stands behind the Angel 24 checkout counter.  This is a true foreground
        // layer, aligned pixel-for-pixel with the shop background, so her lower body is
        // occluded exactly like the authored composition instead of floating in front.
        DrawTexture(sb, _shopSoraForeground.Value, DisplayRect, layout);

        // Keep the small clerk dialogue/counter bubble in front of the counter.  The shop
        // controls are drawn afterwards, matching the intended layer order.
        DrawNativeCentered(sb, _shopSoraSpeechBubble.Value, ShopSoraSpeechBubbleRect, layout);

        for (int i = 0; i < _categoryIcons.Length; i++)
        {
            Rectangle rect = GetCategoryRect(i);
            bool selected = !_showingCart && (int)_selectedCategory == i;
            bool hover = rect.Contains(mouseTablet.ToPoint());
            DrawCategoryIcon(sb, _categoryIcons[i].Value, rect, selected, hover, layout);
        }

        DrawNativeCentered(sb, _shopPanel.Value, ShopPanelRect, layout);
        if (_showingCart)
            DrawCart(sb, layout, mouseTablet);
        else
            DrawProducts(sb, layout, mouseTablet);

        long cartTotal = TabletCart.GetTotalCredits();
        long pyro = cartTotal / TabletWallet.CreditsPerPyroxene;
        DrawNativeCentered(sb, _currencyPanel.Value, ShopCreditRect, layout);
        DrawNativeCentered(sb, _currencyPanel.Value, ShopPyroxeneRect, layout);
        DrawTinyIcon(sb, _creditIcon.Value, new Rectangle(307, 395, 18, 11), layout);
        DrawText(sb, cartTotal.ToString("N0"), new Vector2(329, 394), new Color(76, 99, 149), 0.40f, layout);
        DrawTinyIcon(sb, _pyroxeneIcon.Value, new Rectangle(411, 392, 12, 18), layout);
        DrawText(sb, pyro.ToString("N0"), new Vector2(428, 394), new Color(76, 99, 149), 0.40f, layout);

        bool cartHover = ShopCartRect.Contains(mouseTablet.ToPoint());
        int cartFrameWidth = _cartButton.Value.Width / 3;
        int cartFrame = TabletCart.IsEmpty && _showingCart ? 2 : (cartHover ? 1 : 0);
        DrawNativeCentered(sb, _cartButton.Value, ShopCartRect, layout, new Rectangle(cartFrame * cartFrameWidth, 0, cartFrameWidth, _cartButton.Value.Height));
        if (TabletCart.ItemCount > 0)
            DrawCenteredText(sb, TabletCart.ItemCount.ToString(), new Vector2(526, 376), Color.White, 0.31f, layout);

        if (_checkoutOpen)
            DrawCheckout(sb, layout, mouseTablet);
    }

    private static void DrawProducts(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        IReadOnlyList<TabletShopProduct> products = TabletShopCatalog.GetProducts(_selectedCategory);
        int totalRows = Math.Max(0, (products.Count + 1) / 2);
        int maxRow = Math.Max(0, totalRows - 2);
        _shopScrollRow = Math.Clamp(_shopScrollRow, 0, maxRow);

        int start = _shopScrollRow * 2;
        for (int i = 0; i < 4; i++)
        {
            int index = start + i;
            if (index >= products.Count)
                break;
            DrawProductCard(sb, products[index], GetShopCardRect(i), mouseTablet, layout);
        }

        DrawScrollbar(sb, products.Count, false, layout, mouseTablet);
        if (products.Count == 0)
            DrawCenteredText(sb, "No items in this category", new Vector2(416, 210), new Color(70, 80, 95), 0.50f, layout);
    }

    private static void DrawProductCard(SpriteBatch sb, TabletShopProduct product, Rectangle card, Vector2 mouseTablet, Layout layout)
    {
        DrawNativeCentered(sb, _productCard.Value, card, layout);
        string name = Lang.GetItemNameValue(product.ItemType);
        if (name.Length > 18)
            name = name[..17] + "…";
        DrawCenteredText(sb, name, new Vector2(card.Center.X, card.Y + 5), new Color(48, 68, 93), 0.31f, layout);

        Rectangle itemHoverRect = new(card.X + 4, card.Y + 4, 100, 76);
        DrawItemIcon(sb, product.ItemType, new Rectangle(card.X + 4, card.Y + 30, 100, 48), layout);
        if (itemHoverRect.Contains(mouseTablet.ToPoint()))
            ShowItemTooltip(product.ItemType);

        Rectangle minus = new(card.X + 8, card.Y + 82, 14, 16);
        Rectangle plus = new(card.X + 86, card.Y + 82, 14, 16);
        DrawTwoFrameButton(sb, _productMinus.Value, minus, minus.Contains(mouseTablet.ToPoint()), layout);
        DrawTwoFrameButton(sb, _productPlus.Value, plus, plus.Contains(mouseTablet.ToPoint()), layout);
        DrawDigitsSmall(sb, GetSelectedQuantity(product.ItemType), new Vector2(card.Center.X + 20, card.Y + 82), 1, new Color(76, 99, 149), new Color(191, 197, 220), layout);

        Rectangle add = new(card.X + 8, card.Y + 100, 92, 20);
        bool hover = add.Contains(mouseTablet.ToPoint());
        int purchaseFrameWidth = _purchaseButton.Value.Width / 2;
        DrawNativeCentered(sb, _purchaseButton.Value, add, layout, new Rectangle((hover ? 1 : 0) * purchaseFrameWidth, 0, purchaseFrameWidth, _purchaseButton.Value.Height));
        DrawTinyIcon(sb, _creditIcon.Value, new Rectangle(card.X + 13, card.Y + 106, 12, 7), layout);
        long selectedPrice = (long)product.PriceCredits * GetSelectedQuantity(product.ItemType);
        DrawDigitsSmall(sb, selectedPrice, new Vector2(card.Right - 10, card.Y + 103), 1, Color.White, Color.Transparent, layout);
    }

    private static void DrawCart(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        List<TabletCartLine> lines = TabletCart.Snapshot();
        int maxStart = Math.Max(0, lines.Count - 5);
        _cartScrollIndex = Math.Clamp(_cartScrollIndex, 0, maxStart);

        int start = _cartScrollIndex;
        for (int i = 0; i < 5; i++)
        {
            int index = start + i;
            if (index >= lines.Count)
                break;
            DrawCartRow(sb, lines[index], GetCartRowRect(i), mouseTablet, layout);
        }

        DrawScrollbar(sb, lines.Count, true, layout, mouseTablet);
        if (lines.Count == 0)
            DrawCenteredText(sb, "Shopping cart is empty", new Vector2(416, 210), new Color(70, 80, 95), 0.50f, layout);
        else
            DrawCenteredText(sb, "CART", new Vector2(416, 354), new Color(48, 68, 93), 0.36f, layout);
    }

    private static void DrawCartRow(SpriteBatch sb, TabletCartLine line, Rectangle row, Vector2 mouseTablet, Layout layout)
    {
        int frameWidth = _cartEntry.Value.Width / 2;
        DrawNativeCentered(sb, _cartEntry.Value, row, layout, new Rectangle(0, 0, frameWidth, _cartEntry.Value.Height));

        // The cart should visibly identify what is in each line, just like the shop cards do.
        // Reserve the left 42 px for the real item texture instead of showing a text-only row.
        Rectangle itemHoverRect = new(row.X + 3, row.Y + 3, 116, 44);
        DrawItemIcon(sb, line.ItemType, new Rectangle(row.X + 5, row.Y + 5, 40, 40), layout);
        if (itemHoverRect.Contains(mouseTablet.ToPoint()))
            ShowItemTooltip(line.ItemType);

        string name = Lang.GetItemNameValue(line.ItemType);
        if (name.Length > 22)
            name = name[..21] + "…";
        DrawText(sb, name, new Vector2(row.X + 50, row.Y + 7), new Color(48, 68, 93), 0.30f, layout);

        Rectangle remove = new(row.X + 200, row.Y + 6, 14, 16);
        Rectangle minus = new(row.X + 124, row.Y + 28, 14, 16);
        Rectangle plus = new(row.X + 184, row.Y + 28, 14, 16);
        DrawTwoFrameButton(sb, _cartRemove.Value, remove, remove.Contains(mouseTablet.ToPoint()), layout);
        DrawTwoFrameButton(sb, _cartMinus.Value, minus, minus.Contains(mouseTablet.ToPoint()), layout);
        DrawTwoFrameButton(sb, _cartPlus.Value, plus, plus.Contains(mouseTablet.ToPoint()), layout);

        DrawTinyIcon(sb, _creditIcon.Value, new Rectangle(row.X + 50, row.Y + 30, 14, 9), layout);
        long price = 0;
        if (TabletShopCatalog.TryGetByItemType(line.ItemType, out TabletShopProduct product))
            price = (long)product.PriceCredits * line.Quantity;
        DrawDigitsSmall(sb, price, new Vector2(row.X + 118, row.Y + 27), 1, new Color(76, 99, 149), Color.Transparent, layout);
        DrawDigitsSmall(sb, line.Quantity, new Vector2(row.X + 177, row.Y + 28), 1, new Color(76, 99, 149), new Color(191, 197, 220), layout);
    }

    private static void DrawCheckout(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        // Keep the cart visible behind confirmation.  The old V8 full-size checkout backdrop
        // was an opaque black rectangle, which made the cart look as if it had been disabled.
        // The confirmation card is self-contained, so draw only that card here.
        int panelFrameWidth = _checkoutPanel.Value.Width / 2;
        DrawNativeCentered(sb, _checkoutPanel.Value, CheckoutPanelRect, layout, new Rectangle(panelFrameWidth, 0, panelFrameWidth, _checkoutPanel.Value.Height));

        DrawCenteredText(sb, "CONFIRM ORDER", new Vector2(422, 154), new Color(48, 68, 93), 0.42f, layout, bold: true);
        DrawText(sb, "Order total", new Vector2(328, 186), new Color(48, 68, 93), 0.34f, layout);
        DrawTinyIcon(sb, _creditIcon.Value, new Rectangle(328, 208, 18, 11), layout);
        DrawText(sb, TabletCart.GetTotalCredits().ToString("N0"), new Vector2(352, 204), new Color(76, 99, 149), 0.36f, layout);

        int checkboxFrameWidth = _checkoutCheckbox.Value.Width / 2;
        DrawNativeCentered(sb, _checkoutCheckbox.Value, new Rectangle(328, 250, 18, 18), layout, new Rectangle(checkboxFrameWidth, 0, checkboxFrameWidth, _checkoutCheckbox.Value.Height));
        DrawText(sb, "Delivery Drone", new Vector2(352, 248), new Color(48, 68, 93), 0.36f, layout);
        DrawText(sb, "One package · all cart items", new Vector2(328, 274), new Color(76, 99, 149), 0.30f, layout);

        bool cancelHover = CheckoutCancelRect.Contains(mouseTablet.ToPoint()) && !_checkoutPending;
        bool confirmHover = CheckoutConfirmRect.Contains(mouseTablet.ToPoint()) && !_checkoutPending;
        DrawTwoFrameButton(sb, _checkoutCancel.Value, CheckoutCancelRect, cancelHover, layout);
        DrawTwoFrameButton(sb, _checkoutConfirm.Value, CheckoutConfirmRect, confirmHover, layout);
        DrawCenteredText(sb, "Cancel", new Vector2(371, 309), Color.White, 0.34f, layout);
        DrawCenteredText(sb, _checkoutPending ? "Processing..." : "Confirm", new Vector2(473, 309), new Color(48, 68, 93), 0.34f, layout);
    }

    private static Rectangle GetCategoryRect(int index) => new(274, 80 + index * 28, 24, 24);

    private static Rectangle GetShopCardRect(int slot)
    {
        int col = slot & 1;
        int row = slot >> 1;
        return new Rectangle(306 + col * 110, 80 + row * 128, 108, 126);
    }

    private static Rectangle GetCartRowRect(int slot) => new(306, 80 + slot * 52, 220, 50);

    private static int GetSelectedQuantity(int itemType)
    {
        if (!ProductQuantities.TryGetValue(itemType, out int quantity))
        {
            quantity = 1;
            ProductQuantities[itemType] = 1;
        }
        return quantity;
    }

    private static void ScrollRows(int delta)
    {
        if (_showingCart)
        {
            int maxStart = Math.Max(0, TabletCart.LineCount - 5);
            _cartScrollIndex = Math.Clamp(_cartScrollIndex + delta, 0, maxStart);
        }
        else
        {
            int totalRows = Math.Max(0, (TabletShopCatalog.GetProducts(_selectedCategory).Count + 1) / 2);
            int maxRow = Math.Max(0, totalRows - 2);
            _shopScrollRow = Math.Clamp(_shopScrollRow + delta, 0, maxRow);
        }
    }

    private static void NormalizeCartScroll()
    {
        int maxStart = Math.Max(0, TabletCart.LineCount - 5);
        _cartScrollIndex = Math.Clamp(_cartScrollIndex, 0, maxStart);
    }

    private static void SetScrollFromMouse(int tabletMouseY)
    {
        int count = _showingCart ? TabletCart.LineCount : TabletShopCatalog.GetProducts(_selectedCategory).Count;
        int totalUnits = _showingCart ? count : Math.Max(0, (count + 1) / 2);
        int visibleUnits = _showingCart ? 5 : 2;
        int maxStart = Math.Max(0, totalUnits - visibleUnits);
        if (maxStart <= 0)
        {
            if (_showingCart) _cartScrollIndex = 0; else _shopScrollRow = 0;
            return;
        }

        float t = MathHelper.Clamp((tabletMouseY - ShopScrollTrackRect.Y) / (float)ShopScrollTrackRect.Height, 0f, 1f);
        int value = (int)MathF.Round(t * maxStart);
        if (_showingCart) _cartScrollIndex = value; else _shopScrollRow = value;
    }

    private static void DrawScrollbar(SpriteBatch sb, int itemCount, bool cart, Layout layout, Vector2 mouseTablet)
    {
        int totalUnits = cart ? itemCount : Math.Max(0, (itemCount + 1) / 2);
        int visibleUnits = cart ? 5 : 2;
        int maxStart = Math.Max(0, totalUnits - visibleUnits);

        DrawSolid(sb, ShopScrollTrackRect, new Color(30, 55, 75, 70), layout);

        float ratio = totalUnits <= 0 ? 1f : MathHelper.Clamp(visibleUnits / (float)Math.Max(visibleUnits, totalUnits), 0.12f, 1f);
        int thumbHeight = Math.Max(28, (int)MathF.Round(ShopScrollTrackRect.Height * ratio));
        int start = cart ? _cartScrollIndex : _shopScrollRow;
        float t = maxStart <= 0 ? 0f : start / (float)maxStart;
        int thumbY = ShopScrollTrackRect.Y + (int)MathF.Round((ShopScrollTrackRect.Height - thumbHeight) * t);
        Rectangle thumb = new(ShopScrollTrackRect.X, thumbY, ShopScrollTrackRect.Width, thumbHeight);

        bool hover = ShopScrollTrackRect.Contains(mouseTablet.ToPoint());
        DrawSolid(sb, thumb, hover || _draggingScrollbar ? new Color(48, 68, 93, 230) : new Color(76, 99, 149, 210), layout);
    }

    private static void DrawTopBar(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        DrawNativeCentered(sb, _topBar.Value, new Rectangle(36, 36, 514, 32), layout);
        string pageName = _page switch
        {
            TabletPage.Home => "HOME",
            TabletPage.Formation => "Unit Formation",
            _ => _showingCart ? "SHOP / CART" : "SHOP"
        };
        DrawText(sb, pageName, new Vector2(90, 43), new Color(48, 68, 93), 0.48f, layout, bold: true);

        long total = TabletWallet.GetTotalCredits(Main.LocalPlayer);
        long pyro = total / TabletWallet.CreditsPerPyroxene;
        DrawTinyIcon(sb, _creditIcon.Value, new Rectangle(336, 47, 12, 7), layout);
        DrawDigitsSmall(sb, total, new Vector2(416, 44), 7, new Color(76, 99, 149), new Color(191, 197, 220), layout);
        DrawTinyIcon(sb, _pyroxeneIcon.Value, new Rectangle(428, 43, 11, 15), layout);
        DrawDigitsSmall(sb, pyro, new Vector2(508, 44), 7, new Color(76, 99, 149), new Color(191, 197, 220), layout);
        int settingsFrameWidth = _settingsButton.Value.Width / 3;
        int settingsFrame = (_settingsOpen || SettingsButtonRect.Contains(mouseTablet.ToPoint())) ? 1 : 0;
        DrawNativeCentered(sb, _settingsButton.Value, SettingsButtonRect, layout,
            new Rectangle(settingsFrame * settingsFrameWidth, 0, settingsFrameWidth, _settingsButton.Value.Height));
    }

    private static void DrawSettingsOverlay(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        // Keep the underlying page visible; the settings surface is an in-tablet modal rather
        // than replacing the current page.  This also keeps the gear usable as a close toggle.
        DrawSolid(sb, DisplayRect, Color.Black * 0.42f, layout);

        Rectangle shadow = new(SettingsPanelRect.X + 4, SettingsPanelRect.Y + 5, SettingsPanelRect.Width, SettingsPanelRect.Height);
        DrawSolid(sb, shadow, new Color(12, 24, 43, 160), layout);
        DrawSolid(sb, SettingsPanelRect, new Color(238, 247, 255, 250), layout);

        // Header and divider.
        Rectangle header = new(SettingsPanelRect.X, SettingsPanelRect.Y, SettingsPanelRect.Width, 40);
        DrawSolid(sb, header, new Color(90, 123, 178, 255), layout);
        int returnFrameWidth = _returnButton.Value.Width / 3;
        int returnFrame = SettingsBackRect.Contains(mouseTablet.ToPoint()) ? 1 : 0;
        DrawTexture(sb, _returnButton.Value, SettingsBackRect, layout,
            new Rectangle(returnFrame * returnFrameWidth, 0, returnFrameWidth, _returnButton.Value.Height));

        DrawCenteredText(sb, "TABLET SETTINGS", new Vector2(SettingsPanelRect.Center.X + 10, SettingsPanelRect.Y + 9),
            Color.White, 0.45f, layout);
        DrawText(sb, "Tablet Resolution", new Vector2(SettingsPanelRect.X + 24, SettingsPanelRect.Y + 64),
            new Color(48, 68, 93), 0.40f, layout, bold: true);

        TabletUiPreferencesPlayer prefs = Main.LocalPlayer.GetModPlayer<TabletUiPreferencesPlayer>();
        float requestedScale = prefs.TabletScale;
        int width = (int)MathF.Round(TabletWidth * requestedScale);
        int height = (int)MathF.Round(TabletHeight * requestedScale);
        int percent = (int)MathF.Round(requestedScale * 100f);
        string resolution = $"{width} x {height}   ({percent}%)";
        DrawCenteredText(sb, resolution,
            new Vector2(SettingsPanelRect.Center.X, SettingsPanelRect.Y + 102),
            new Color(48, 68, 93), 0.43f, layout, bold: true);

        DrawSettingsButton(sb, SettingsScaleDownRect, "<",
            SettingsScaleDownRect.Contains(mouseTablet.ToPoint()), prefs.CanDecreaseScale, layout);
        DrawSettingsButton(sb, SettingsScaleUpRect, ">",
            SettingsScaleUpRect.Contains(mouseTablet.ToPoint()), prefs.CanIncreaseScale, layout);
        DrawSettingsButton(sb, SettingsResetRect, "Reset 100%",
            SettingsResetRect.Contains(mouseTablet.ToPoint()), true, layout);

        DrawCenteredText(sb, "Back / ESC / gear: close",
            new Vector2(SettingsPanelRect.Center.X, SettingsPanelRect.Bottom - 20),
            new Color(90, 108, 130), 0.29f, layout, bold: false);
    }

    private static void DrawSettingsButton(
        SpriteBatch sb,
        Rectangle rect,
        string text,
        bool hover,
        bool enabled,
        Layout layout)
    {
        Color fill = !enabled
            ? new Color(172, 183, 198, 210)
            : hover
                ? new Color(76, 120, 190, 255)
                : new Color(91, 113, 158, 255);
        DrawSolid(sb, rect, fill, layout);
        DrawCenteredText(sb, text, new Vector2(rect.Center.X, rect.Y + 6),
            Color.White, rect.Width > 60 ? 0.34f : 0.48f, layout, bold: true);
    }

    private static void DrawFrame(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        DrawTexture(sb, _frame.Value, new Rectangle(0, 0, TabletWidth, TabletHeight), layout);
        Texture2D tex = _homeButton.Value;
        int frameWidth = tex.Width / 2;
        bool hover = HomeButtonRect.Contains(mouseTablet.ToPoint());
        Rectangle source = new(hover ? frameWidth : 0, 0, frameWidth, tex.Height);
        Rectangle screen = layout.ToScreen(HomeButtonRect);
        Vector2 center = new(screen.Center.X, screen.Center.Y);
        Vector2 origin = new(source.Width / 2f, source.Height / 2f);
        sb.Draw(tex, center, source, Color.White, -MathHelper.PiOver2, origin, layout.Scale, SpriteEffects.None, 0f);
    }

    private static void DrawReturnButton(SpriteBatch sb, Layout layout, Vector2 mouseTablet)
    {
        bool hover = ReturnRect.Contains(mouseTablet.ToPoint());
        Texture2D tex = _returnButton.Value;
        int frameWidth = tex.Width / 3;
        DrawNativeCentered(sb, tex, ReturnRect, layout, new Rectangle((hover ? 1 : 0) * frameWidth, 0, frameWidth, tex.Height));
    }

    private static void DrawHomeButton(SpriteBatch sb, Texture2D tex, Rectangle dest, string label, bool enabled, Vector2 mouseTablet, Layout layout)
    {
        int frameWidth = tex.Width / 3;
        bool hover = enabled && dest.Contains(mouseTablet.ToPoint());

        // Frame 2 in these authored button sheets is NOT a disabled background.
        // It is mostly transparent/icon-only artwork. Using it for unavailable features
        // made the home screen look like large empty holes. Keep the complete frame visible
        // even when the feature is temporarily non-interactive.
        int frame = hover ? 1 : 0;
        DrawNativeCentered(sb, tex, dest, layout, new Rectangle(frame * frameWidth, 0, frameWidth, tex.Height));

        Color color = enabled ? new Color(48, 68, 93) : new Color(100, 108, 122);
        DrawCenteredText(sb, label, new Vector2(dest.Center.X, dest.Bottom - 20), color, dest.Width > 100 ? 0.40f : 0.34f, layout);
    }

    private static void DrawCategoryIcon(SpriteBatch sb, Texture2D tex, Rectangle dest, bool selected, bool hover, Layout layout)
    {
        // Legacy category sheets are 3 frames wide:
        // 0 = normal, 1 = maintained/selected, 2 = highlight overlay.
        int frameWidth = tex.Width / 3;
        int baseFrame = selected ? 1 : 0;
        DrawNativeCentered(sb, tex, dest, layout, new Rectangle(baseFrame * frameWidth, 0, frameWidth, tex.Height));

        if (hover)
            DrawNativeCentered(sb, tex, dest, layout, new Rectangle(2 * frameWidth, 0, frameWidth, tex.Height));
    }

    private static void DrawTwoFrameButton(SpriteBatch sb, Texture2D tex, Rectangle dest, bool hover, Layout layout)
    {
        int frameWidth = tex.Width / 2;
        DrawNativeCentered(sb, tex, dest, layout, new Rectangle((hover ? 1 : 0) * frameWidth, 0, frameWidth, tex.Height));
    }

    private static void DrawSimpleButton(SpriteBatch sb, Rectangle rect, string text, Vector2 mouseTablet, Layout layout)
    {
        bool hover = rect.Contains(mouseTablet.ToPoint());
        DrawSolid(sb, rect, hover ? new Color(74, 172, 220, 225) : new Color(37, 87, 120, 210), layout);
        DrawCenteredText(sb, text, new Vector2(rect.Center.X, rect.Y + 2), Color.White, 0.48f, layout);
    }

    private static void DrawNativeCentered(SpriteBatch sb, Texture2D tex, Rectangle authoredBounds, Layout layout, Rectangle? source = null)
    {
        Rectangle src = source ?? new Rectangle(0, 0, tex.Width, tex.Height);
        Vector2 center = layout.ToScreen(new Vector2(authoredBounds.Center.X, authoredBounds.Center.Y));
        Vector2 origin = new(src.Width * 0.5f, src.Height * 0.5f);
        sb.Draw(tex, center, src, Color.White, 0f, origin, layout.Scale, SpriteEffects.None, 0f);
    }

    private static void DrawTexture(SpriteBatch sb, Texture2D tex, Rectangle tabletRect, Layout layout, Rectangle? source = null)
    {
        sb.Draw(tex, layout.ToScreen(tabletRect), source, Color.White);
    }

    private static void DrawTinyIcon(SpriteBatch sb, Texture2D tex, Rectangle tabletRect, Layout layout)
    {
        sb.Draw(tex, layout.ToScreen(tabletRect), null, Color.White);
    }

    private static void DrawTextureFit(SpriteBatch sb, Texture2D tex, Rectangle tabletBounds, Layout layout)
    {
        DrawTextureFit(sb, tex, tex.Bounds, tabletBounds, layout);
    }

    private static void DrawTextureFit(SpriteBatch sb, Texture2D tex, Rectangle source, Rectangle tabletBounds, Layout layout)
    {
        float fit = Math.Min(tabletBounds.Width / (float)Math.Max(1, source.Width), tabletBounds.Height / (float)Math.Max(1, source.Height));
        int width = Math.Max(1, (int)MathF.Round(source.Width * fit));
        int height = Math.Max(1, (int)MathF.Round(source.Height * fit));
        Rectangle rect = new(
            tabletBounds.X + (tabletBounds.Width - width) / 2,
            tabletBounds.Y + (tabletBounds.Height - height) / 2,
            width,
            height);
        DrawTexture(sb, tex, rect, layout, source);
    }

    private static void DrawItemIcon(SpriteBatch sb, int itemType, Rectangle tabletBounds, Layout layout)
    {
        if (itemType <= ItemID.None || itemType >= ItemLoader.ItemCount)
            return;

        // Vanilla item textures are lazily loaded.  Modded guns were already resident, which hid
        // this bug in earlier builds; vanilla bullets/rockets could therefore render as blank cards.
        Main.instance.LoadItem(itemType);
        Texture2D tex = TextureAssets.Item[itemType].Value;

        // Respect Terraria/tModLoader item animations instead of always sampling the entire sheet.
        // This also lets the Shittim Tablet use its own clean-room animated inventory icon.
        Rectangle source = Main.itemAnimations[itemType]?.GetFrame(tex) ?? tex.Bounds;
        DrawTextureFit(sb, tex, source, tabletBounds, layout);
    }

    private static void DrawSolid(SpriteBatch sb, Rectangle tabletRect, Color color, Layout layout)
    {
        sb.Draw(TextureAssets.MagicPixel.Value, layout.ToScreen(tabletRect), color);
    }

    private static void DrawText(SpriteBatch sb, string text, Vector2 tabletPosition, Color color, float scale, Layout layout, bool bold = true)
    {
        DynamicSpriteFont font = (bold ? _uiFontBold : _uiFont)?.Value ?? FontAssets.MouseText.Value;
        Vector2 position = layout.ToScreen(tabletPosition);
        float finalScale = scale * layout.Scale;

        // Keep the clean Noto Sans presentation used by the tablet artwork. No Terraria-style heavy outline.
        sb.DrawString(font, text, position, color, 0f, Vector2.Zero, finalScale, SpriteEffects.None, 0f);
    }

    private static void DrawCenteredText(SpriteBatch sb, string text, Vector2 tabletCenterTop, Color color, float scale, Layout layout, bool bold = true)
    {
        DynamicSpriteFont font = (bold ? _uiFontBold : _uiFont)?.Value ?? FontAssets.MouseText.Value;
        float finalScale = scale * layout.Scale;
        Vector2 size = font.MeasureString(text) * finalScale;
        Vector2 center = layout.ToScreen(tabletCenterTop);
        Vector2 position = new(center.X - size.X / 2f, center.Y);
        sb.DrawString(font, text, position, color, 0f, Vector2.Zero, finalScale, SpriteEffects.None, 0f);
    }

    private static void DrawDigitsSmall(SpriteBatch sb, long value, Vector2 rightEdgeTop, int minDigits, Color color, Color shadowColor, Layout layout)
    {
        Texture2D tex = _digitsSmall?.Value;
        if (tex == null)
            return;

        string digits = Math.Abs(value).ToString();
        if (digits.Length < minDigits)
            digits = digits.PadLeft(minDigits, '0');

        float x = rightEdgeTop.X - 10f;
        for (int i = digits.Length - 1; i >= 0; i--)
        {
            int digit = digits[i] - '0';
            Rectangle src = new(digit * 10, 0, 10, 14);
            Vector2 pos = layout.ToScreen(new Vector2(x, rightEdgeTop.Y));
            if (shadowColor.A > 0)
                sb.Draw(tex, pos + new Vector2(0f, 2f * layout.Scale), src, shadowColor, 0f, Vector2.Zero, layout.Scale, SpriteEffects.None, 0f);
            sb.Draw(tex, pos, src, color, 0f, Vector2.Zero, layout.Scale, SpriteEffects.None, 0f);
            x -= 8f;
        }

        if (value < 0)
        {
            Rectangle src = new(24, 14, 8, 14);
            Vector2 pos = layout.ToScreen(new Vector2(x + 2f, rightEdgeTop.Y));
            if (shadowColor.A > 0)
                sb.Draw(tex, pos + new Vector2(0f, 2f * layout.Scale), src, shadowColor, 0f, Vector2.Zero, layout.Scale, SpriteEffects.None, 0f);
            sb.Draw(tex, pos, src, color, 0f, Vector2.Zero, layout.Scale, SpriteEffects.None, 0f);
        }
    }

    private static void ShowItemTooltip(int itemType)
    {
        if (itemType <= ItemID.None || itemType >= ItemLoader.ItemCount)
            return;

        // Use Terraria's own HoverItem/MouseText pipeline so the shop tooltip contains the same
        // damage, knockback, use speed, rarity, modded tooltip lines and description as inventory hover.
        Item sample = ContentSamples.ItemsByType[itemType].Clone();
        Main.HoverItem = sample;
        Main.hoverItemName = sample.Name;
        Main.instance.MouseText(Main.hoverItemName);
    }

    private static void DrawClassroomBackground(SpriteBatch sb, Layout layout)
    {
        // The original art is 880x466 and authored with an X offset of 250.
        // Crop the 514x386 display window from every layer, preserving the authored parallax composition.
        Rectangle source = new(250, 0, DisplayRect.Width, DisplayRect.Height);
        for (int i = 0; i < _classroomLayers.Length; i++)
        {
            Texture2D tex = _classroomLayers[i].Value;
            sb.Draw(tex, layout.ToScreen(DisplayRect), source, Color.White);
        }
    }

    private static void DrawPageTransitionOverlay(SpriteBatch sb, Layout layout)
    {
        if (!PageTransitionActive)
            return;

        int elapsed = Math.Clamp(_pageTransitionElapsed, 0, PageTransitionTicks);
        float opacity;
        if (elapsed < PageFadeTicks)
            opacity = elapsed / (float)PageFadeTicks;
        else if (elapsed < PageFadeTicks + PageHoldTicks)
            opacity = 1f;
        else
            opacity = 1f - (elapsed - PageFadeTicks - PageHoldTicks) / (float)PageFadeTicks;

        opacity = MathHelper.Clamp(opacity, 0f, 1f);
        if (opacity > 0f)
            DrawSolid(sb, DisplayRect, Color.Black * opacity, layout);
    }

    private static void DrawPowerOverlay(SpriteBatch sb, Layout layout)
    {
        float progress = OpenProgress;
        if (progress >= 0.999f)
            return;

        // Power-on/off is a CRT-like sequence: black display -> thin vertical white line ->
        // horizontal bloom -> white fade.  Because it is driven by OpenProgress, putting the
        // tablet away naturally plays the exact visual sequence in reverse.
        const float stage1End = 15f / 35f;
        const float stage2End = 25f / 35f;

        float blackOpacity = 0f;
        float whiteOpacity = 0f;
        float whiteWidth = 0f;
        float whiteHeight = 0f;

        if (progress < stage1End)
        {
            float t = MathHelper.Clamp(progress / stage1End, 0f, 1f);
            float eased = 1f - (1f - t) * (1f - t);
            blackOpacity = 1f;
            whiteOpacity = eased;
            whiteHeight = eased;
            whiteWidth = 0.025f * t;
        }
        else if (progress < stage2End)
        {
            float t = MathHelper.Clamp((progress - stage1End) / (stage2End - stage1End), 0f, 1f);
            blackOpacity = 1f;
            whiteOpacity = 1f;
            whiteHeight = 1f;
            whiteWidth = 0.025f + 0.975f * t * t;
        }
        else
        {
            float t = MathHelper.Clamp((progress - stage2End) / (1f - stage2End), 0f, 1f);
            whiteOpacity = 1f - t;
            whiteHeight = 1f;
            whiteWidth = 1f;
        }

        if (blackOpacity > 0f)
            DrawSolid(sb, DisplayRect, Color.Black * blackOpacity, layout);

        if (whiteOpacity <= 0f || whiteWidth <= 0f || whiteHeight <= 0f)
            return;

        int width = Math.Max(1, (int)MathF.Round(DisplayRect.Width * whiteWidth));
        int height = Math.Max(1, (int)MathF.Round(DisplayRect.Height * whiteHeight));
        Rectangle whiteRect = new(
            DisplayRect.Center.X - width / 2,
            DisplayRect.Center.Y - height / 2,
            width,
            height);
        DrawSolid(sb, whiteRect, Color.White * whiteOpacity, layout);
    }

    private static void DrawTouchFeedback(SpriteBatch sb, Layout layout)
    {
        if (_touchEffect == null || _touchParticle == null)
            return;

        Texture2D rippleTexture = _touchEffect.Value;
        int rippleFrameWidth = rippleTexture.Width / TouchBurst.FrameCount;
        int rippleFrameHeight = rippleTexture.Height / TouchBurst.VariantCount;

        foreach (TouchBurst burst in TouchBursts)
        {
            int frame = Math.Clamp(burst.Age, 0, TouchBurst.FrameCount - 1);
            Rectangle source = new(
                frame * rippleFrameWidth,
                burst.Variant * rippleFrameHeight,
                rippleFrameWidth,
                rippleFrameHeight);
            sb.Draw(
                rippleTexture,
                layout.ToScreen(burst.Position),
                source,
                Color.White,
                0f,
                new Vector2(rippleFrameWidth, rippleFrameHeight) * 0.5f,
                layout.Scale,
                SpriteEffects.None,
                0f);
        }

        Texture2D particleTexture = _touchParticle.Value;
        int particleFrameWidth = particleTexture.Width / TouchSpark.FrameCount;
        int particleFrameHeight = particleTexture.Height / TouchSpark.VariantCount;

        for (int i = 0; i < TouchTrail.Count; i++)
        {
            float alpha = (i + 1f) / Math.Max(1, TouchTrail.Count);
            Rectangle source = new(0, 0, particleFrameWidth, particleFrameHeight);
            sb.Draw(
                particleTexture,
                layout.ToScreen(TouchTrail[i]),
                source,
                Color.LightBlue * (alpha * 0.65f),
                0f,
                new Vector2(particleFrameWidth, particleFrameHeight) * 0.5f,
                0.65f * layout.Scale,
                SpriteEffects.None,
                0f);
        }

        foreach (TouchSpark spark in TouchSparks)
        {
            int frame = Math.Clamp(spark.Age, 0, TouchSpark.FrameCount - 1);
            Rectangle source = new(
                frame * particleFrameWidth,
                spark.Variant * particleFrameHeight,
                particleFrameWidth,
                particleFrameHeight);
            sb.Draw(
                particleTexture,
                layout.ToScreen(spark.Position),
                source,
                Color.White,
                spark.Rotation,
                new Vector2(particleFrameWidth, particleFrameHeight) * 0.5f,
                layout.Scale,
                SpriteEffects.None,
                0f);
        }
    }

    private static Vector2 GetUiMousePosition()
    {
        // Terraria UI hit-testing uses Main.MouseScreen directly (the same coordinate space used by UIElement.ContainsPoint).
        // Do NOT invert UIScaleMatrix here: doing so transforms the cursor a second time and produces the persistent
        // offset seen at non-default UI scales / centered UI matrices.
        return Main.MouseScreen;
    }

    private static Layout GetLayout()
    {
        // Use the UIState's own dimensions. These are the exact coordinates tModLoader also uses
        // for Main.MouseScreen hit-testing, so the renderer and interactions cannot drift apart.
        CalculatedStyle root = _instance?._tabletState?.GetDimensions() ?? default;
        float uiWidth = root.Width > 1f ? root.Width : Main.screenWidth;
        float uiHeight = root.Height > 1f ? root.Height : Main.screenHeight;
        Vector2 rootPos = root.Width > 1f ? new Vector2(root.X, root.Y) : Vector2.Zero;

        float fit = Math.Min((uiWidth - 24f) / TabletWidth, (uiHeight - 24f) / TabletHeight);
        float requestedScale = 1f;
        Player localPlayer = Main.LocalPlayer;
        if (localPlayer != null && localPlayer.active)
            requestedScale = localPlayer.GetModPlayer<TabletUiPreferencesPlayer>().TabletScale;

        // Respect the player's selected tablet resolution, but never let the physical device
        // spill outside the current UI viewport.  The lower clamp only protects tiny windows.
        float scale = MathHelper.Clamp(Math.Min(requestedScale, fit), 0.55f, 1.40f);
        Vector2 origin = rootPos + new Vector2((uiWidth - TabletWidth * scale) * 0.5f, (uiHeight - TabletHeight * scale) * 0.5f);

        // Raise/lower the entire physical tablet from the bottom of the screen with an OutQuad
        // curve.  At progress 0 its authored center is one UI-screen below the resting position.
        float progress = OpenProgress;
        float eased = 1f - (1f - progress) * (1f - progress);
        origin.Y += (1f - eased) * uiHeight;

        // A tiny inverse-velocity offset keeps the device visually attached to the player's
        // hands while moving instead of looking like a fixed HUD window.
        Player player = Main.LocalPlayer;
        if (player != null && player.active)
            origin -= player.velocity / Math.Max(0.01f, Main.UIScale);

        return new Layout(origin, scale);
    }

    private sealed class TouchBurst
    {
        public const int FrameCount = 27;
        public const int VariantCount = 4;

        public readonly Vector2 Position;
        public readonly int Variant;
        public int Age { get; private set; }
        public bool Expired => Age >= FrameCount;

        public TouchBurst(Vector2 position)
        {
            Position = position;
            Variant = Main.rand.Next(VariantCount);
        }

        public void Update() => Age++;
    }

    private sealed class TouchSpark
    {
        public const int FrameCount = 27;
        public const int VariantCount = 4;

        public Vector2 Position { get; private set; }
        public int Variant { get; }
        public int Age { get; private set; }
        public float Rotation { get; }
        public bool Expired => Age >= FrameCount;

        private Vector2 _velocity;
        private readonly float _falloff;

        public TouchSpark(Vector2 position)
        {
            Position = position;
            Variant = Main.rand.Next(VariantCount);
            _velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
            _falloff = Main.rand.NextFloat(0.8f, 1f);
            Rotation = Main.rand.NextBool() ? 0f : MathHelper.Pi;
        }

        public void Update()
        {
            Position += _velocity;
            _velocity *= _falloff;
            Age++;
        }
    }

    private sealed class TabletSurfaceState : UIState
    {
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            DrawTabletSurface(spriteBatch);
        }
    }

    private readonly struct Layout
    {
        public readonly Vector2 Origin;
        public readonly float Scale;

        public Layout(Vector2 origin, float scale)
        {
            Origin = origin;
            Scale = scale;
        }

        public Vector2 ToTablet(Vector2 uiPosition) => (uiPosition - Origin) / Scale;
        public Vector2 ToScreen(Vector2 tabletPosition) => Origin + tabletPosition * Scale;

        public Rectangle ToScreen(Rectangle tabletRect)
        {
            return new Rectangle(
                (int)MathF.Round(Origin.X + tabletRect.X * Scale),
                (int)MathF.Round(Origin.Y + tabletRect.Y * Scale),
                Math.Max(1, (int)MathF.Round(tabletRect.Width * Scale)),
                Math.Max(1, (int)MathF.Round(tabletRect.Height * Scale)));
        }
    }
}
