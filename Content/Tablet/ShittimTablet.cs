using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet;

/// <summary>
/// Clean-room implementation of the Shittim Tablet item.
/// It intentionally does not depend on the legacy Kivotos-Student item/UI classes.
/// </summary>
public class ShittimTablet : ModItem
{
    private const int IconFrameCount = 23;

    public override LocalizedText DisplayName => Language.GetOrRegister(
        "Mods.KivotosMod.Items.ShittimTablet.DisplayName",
        () => "什亭之匣");

    public override LocalizedText Tooltip => Language.GetOrRegister(
        "Mods.KivotosMod.Items.ShittimTablet.Tooltip",
        () => "拿在手中时打开平板；切换到其他物品时关闭");

    public override string Texture => "KivotosMod/Assets/Tablet/UI/ShittimTablet";

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 1;
        ItemID.Sets.AnimatesAsSoul[Type] = true;
        Main.RegisterItemAnimation(Type, new TabletInventoryAnimation(Type));
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.rare = ItemRarityID.Cyan;
        Item.value = Item.sellPrice(gold: 1);

        // The held tablet has its own player draw layer.  Suppressing Terraria's normal held-item
        // sprite prevents the animated 23-frame inventory sheet from being drawn as one tall texture.
        Item.noUseGraphic = true;
        Item.holdStyle = ItemHoldStyleID.None;
    }

    public override void HoldStyle(Player player, Rectangle heldItemFrame)
    {
        ApplyHeldPose(player);
    }

    public override void HoldItem(Player player)
    {
        // Keep the pose stable even on frames where vanilla does not re-run HoldStyle.
        ApplyHeldPose(player);
    }

    private static void ApplyHeldPose(Player player)
    {
        float gravity = player.gravDir;
        float direction = player.direction;

        player.compositeFrontArm.enabled = true;
        player.compositeFrontArm.stretch = Player.CompositeArmStretchAmount.Full;
        player.compositeFrontArm.rotation = MathHelper.ToRadians(-65f) * direction * gravity;

        player.compositeBackArm.enabled = true;
        player.compositeBackArm.stretch = Player.CompositeArmStretchAmount.Full;
        player.compositeBackArm.rotation = MathHelper.ToRadians(-50f) * direction * gravity;

        player.itemLocation = player.MountedCenter + new Vector2(12f * direction, -4f * gravity);
    }


    public override void PostUpdate()
    {
        if (Main.itemAnimations[Type] is TabletInventoryAnimation animation && animation.IsLit)
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());
    }

    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Glass, 10)
            .AddIngredient(ItemID.FallenStar, 1)
            .AddTile(TileID.WorkBenches)
            .Register();
    }

    /// <summary>
    /// Independent animation controller for the supplied 23-frame tablet art.
    /// Selected/held: boot frames -> live loop.  Deselected: shutdown frames -> idle.
    /// </summary>
    private sealed class TabletInventoryAnimation : DrawAnimation
    {
        private const int IdleFrame = 0;
        private const int BootStart = 1;
        private const int BootEnd = 8;
        private const int LiveStart = 9;
        private const int LiveEnd = 14;
        private const int ShutdownStart = 15;
        private const int ShutdownEnd = 22;
        private const int StepTicks = 5;

        private readonly int _itemType;
        private int _frame;
        private int _timer;
        private AnimationState _state;

        public TabletInventoryAnimation(int itemType)
        {
            _itemType = itemType;
            _frame = IdleFrame;
            _state = AnimationState.Idle;
        }

        public override void Update()
        {
            Player player = Main.LocalPlayer;
            bool held = player != null
                && player.active
                && !player.dead
                && player.HeldItem != null
                && player.HeldItem.type == _itemType;

            switch (_state)
            {
                case AnimationState.Idle:
                    _frame = IdleFrame;
                    if (held)
                    {
                        _state = AnimationState.Booting;
                        _frame = BootStart;
                        _timer = StepTicks;
                    }
                    break;

                case AnimationState.Booting:
                    if (!held)
                    {
                        _state = AnimationState.ShuttingDown;
                        _frame = ShutdownStart;
                        _timer = StepTicks;
                        break;
                    }
                    if (Tick())
                    {
                        _frame++;
                        if (_frame > BootEnd)
                        {
                            _state = AnimationState.Live;
                            _frame = LiveStart;
                        }
                    }
                    break;

                case AnimationState.Live:
                    if (!held)
                    {
                        _state = AnimationState.ShuttingDown;
                        _frame = ShutdownStart;
                        _timer = StepTicks;
                        break;
                    }
                    if (Tick())
                    {
                        _frame++;
                        if (_frame > LiveEnd)
                            _frame = LiveStart;
                    }
                    break;

                case AnimationState.ShuttingDown:
                    if (held)
                    {
                        _state = AnimationState.Booting;
                        _frame = BootStart;
                        _timer = StepTicks;
                        break;
                    }
                    if (Tick())
                    {
                        _frame++;
                        if (_frame > ShutdownEnd)
                        {
                            _state = AnimationState.Idle;
                            _frame = IdleFrame;
                        }
                    }
                    break;
            }
        }

        private bool Tick()
        {
            if (_timer > 0)
            {
                _timer--;
                return false;
            }

            _timer = StepTicks;
            return true;
        }

        public bool IsLit => _frame != IdleFrame;

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1)
        {
            int frameHeight = texture.Height / IconFrameCount;
            int clamped = Math.Clamp(_frame, 0, IconFrameCount - 1);
            return new Rectangle(0, clamped * frameHeight, texture.Width, frameHeight);
        }

        private enum AnimationState
        {
            Idle,
            Booting,
            Live,
            ShuttingDown,
        }
    }
}

[Autoload(Side = ModSide.Client)]
public sealed class ShittimTabletHeldLayer : PlayerDrawLayer
{
    private static Asset<Texture2D> _texture;

    public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
    {
        Player player = drawInfo.drawPlayer;
        return drawInfo.shadow <= 0f
            && !player.dead
            && !player.ghost
            && player.HeldItem != null
            && player.HeldItem.type == ModContent.ItemType<ShittimTablet>();
    }

    // Draw the tablet immediately before Terraria's arm-over-item pass so the front hand can naturally overlap it.
    public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.ArmOverItem);

    protected override void Draw(ref PlayerDrawSet drawInfo)
    {
        _texture ??= ModContent.Request<Texture2D>(
            "KivotosMod/Assets/Tablet/UI/ShittimTabletHold",
            AssetRequestMode.ImmediateLoad);

        Player player = drawInfo.drawPlayer;
        Vector2 worldPosition = player.itemLocation.Floor() + new Vector2(player.direction, player.gfxOffY).Floor();

        // Walking/jumping and sitting use slightly different vanilla body anchors.  Nudge the
        // tablet with the body instead of leaving it hovering a few pixels away from the hands.
        int bodyFrame = player.bodyFrame.Y / Math.Max(1, player.bodyFrame.Height);
        if (bodyFrame is 7 or 8 or 9 or 14 or 15 or 16)
            worldPosition.Y -= 2f * player.gravDir;
        if (player.sitting.isSitting)
            worldPosition.Y -= 4f * player.gravDir;

        Vector2 screenPosition = worldPosition - Main.screenPosition;
        Color light = Lighting.GetColor(worldPosition.ToTileCoordinates());
        Texture2D texture = _texture.Value;

        DrawData data = new(
            texture,
            screenPosition,
            texture.Bounds,
            light,
            0f,
            texture.Size() * 0.5f,
            1f,
            drawInfo.playerEffect,
            0f);

        drawInfo.DrawDataCache.Add(data);
    }

    public override void Unload()
    {
        _texture = null;
    }
}
