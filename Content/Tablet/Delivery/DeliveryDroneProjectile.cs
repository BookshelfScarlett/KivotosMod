using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Tablet.Delivery;

public sealed class DeliveryDroneProjectile : ModProjectile
{
    public override string Texture => "KivotosMod/Assets/Tablet/Delivery/DeliveryDrone";

    private const byte ApproachState = 0;
    private const byte BrakeState = 1;
    private const byte DropState = 2;
    private const byte DepartState = 3;

    private const float CruiseSpeed = 8f;
    private const float Inertia = 40f;
    private const float DropDistance = 64f;
    private const float DespawnDistance = 2400f;

    private byte _state;
    private ushort _timer;
    private bool _delivered;

    private int _bodyFrame;
    private int _bodyFrameTimer;
    private int _gripperFrame = 6;
    private int _gripperFrameTimer = 10;
    private bool _visualsInitialized;
    private bool _gripperOpened;
    private Vector2 _heldOffset;

    private static Asset<Texture2D> _spritesheet;

    private int OrderId => (int)Projectile.ai[0];
    private int PackageType => (int)Projectile.ai[1];
    private Player Owner => Projectile.owner >= 0 && Projectile.owner < Main.maxPlayers ? Main.player[Projectile.owner] : null;

    public override void Load()
    {
        if (!Main.dedServ)
            _spritesheet = ModContent.Request<Texture2D>("KivotosMod/Assets/Tablet/Delivery/DeliveryDroneSpritesheet");
    }

    public override void Unload()
    {
        _spritesheet = null;
    }

    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = false;
        Projectile.timeLeft = 600;
        Projectile.netImportant = true;
    }

    public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
    {
        Projectile.Center = Projectile.position;
        if (Owner == null || !Owner.active)
            Projectile.Kill();
    }

    public override void AI()
    {
        Projectile.timeLeft = 3;
        Player player = Owner;

        if (player == null || !player.active)
        {
            EnterDepart();
        }

        switch (_state)
        {
            case ApproachState:
                if (IsOwnerValid(player))
                {
                    MoveToward(player.Center + new Vector2(0f, -24f));
                    if (Vector2.Distance(player.Center, Projectile.Center) < DropDistance)
                        _timer++;
                    else
                        _timer = 0;

                    if (_timer >= 120)
                    {
                        _state = BrakeState;
                        _timer = 0;
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    EnterDepart();
                }
                break;

            case BrakeState:
                Projectile.velocity *= 0.96f;
                if (Projectile.velocity.Length() <= 0.05f)
                {
                    Projectile.velocity = Vector2.Zero;
                    _state = DropState;
                    _timer = 60;
                    Projectile.netUpdate = true;
                }
                break;

            case DropState:
                if (!_delivered && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 dropPosition = Projectile.Center + _heldOffset * Projectile.scale;
                    _delivered = TabletDeliveryOrderSystem.TryDropPackage(OrderId, dropPosition);
                    Projectile.netUpdate = true;
                }

                if (_timer > 0)
                    _timer--;
                else
                    EnterDepart();
                break;

            case DepartState:
                MoveAway();
                if (_timer > 0)
                    _timer--;

                if (player == null || !player.active)
                {
                    if (_timer == 0)
                        Projectile.Kill();
                }
                else if (Vector2.Distance(player.Center, Projectile.Center) >= DespawnDistance || _timer == 0)
                {
                    Projectile.Kill();
                }
                break;
        }

        UpdateVisualState();
        Projectile.rotation = Math.Clamp(Projectile.velocity.X / 25f, -MathHelper.PiOver4, MathHelper.PiOver4);
    }

    private void EnterDepart()
    {
        if (_state == DepartState)
            return;
        _state = DepartState;
        _timer = 600;
        Projectile.netUpdate = true;
    }

    private bool IsOwnerValid(Player player)
    {
        return player != null && player.active && !player.dead && !player.ghost;
    }

    private void MoveToward(Vector2 target)
    {
        Vector2 direction = target - Projectile.Center;
        float distance = direction.Length();
        float speed = CruiseSpeed;
        float inertia = Inertia * 0.5f;

        if (distance < DropDistance * 2f)
        {
            float ratio = distance / (DropDistance * 2f);
            speed *= ratio;
            inertia *= ratio;
        }

        direction = direction.SafeNormalize(Vector2.Zero);
        Projectile.velocity = (Projectile.velocity * inertia + direction * speed) / (inertia + 1f);
    }

    private void MoveAway()
    {
        Projectile.velocity = (Projectile.velocity * Inertia - Vector2.UnitY * CruiseSpeed) / (Inertia + 1f);
    }

    private void UpdateVisualState()
    {
        if (++_bodyFrameTimer >= 2)
        {
            _bodyFrameTimer = 0;
            _bodyFrame = (_bodyFrame + 1) % 4;
        }

        if (Main.dedServ)
            return;

        if (!_visualsInitialized && _state < DropState)
        {
            Main.GetItemDrawFrame(PackageType, out _, out Rectangle frame);
            _heldOffset = new Vector2(0f, frame.Height * 0.5f + Projectile.height * 0.5f + 2f * Projectile.scale);
            _gripperFrame = frame.Width switch
            {
                > 50 => 0,
                > 40 => 1,
                >= 34 => 2,
                >= 22 => 3,
                >= 10 => 4,
                >= 6 => 5,
                _ => 6,
            };
            _visualsInitialized = true;
        }
        else if (_visualsInitialized && _state >= DropState)
        {
            if (!_gripperOpened)
            {
                _gripperFrameTimer = 30;
                _gripperFrame = Math.Max(0, _gripperFrame - 1);
                _gripperOpened = true;
            }
            else if (_gripperFrame < 6 && --_gripperFrameTimer <= 0)
            {
                _gripperFrameTimer = 6;
                _gripperFrame++;
            }
        }
        else if (!_visualsInitialized)
        {
            _gripperFrame = 6;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (_spritesheet == null)
            return false;

        Texture2D sheet = _spritesheet.Value;

        if (_visualsInitialized && _state < DropState && PackageType > ItemID.None)
        {
            Main.GetItemDrawFrame(PackageType, out Texture2D itemTexture, out Rectangle itemFrame);
            Vector2 origin = new(itemFrame.Width * 0.5f, itemFrame.Height * 0.5f);
            Main.EntitySpriteDraw(
                itemTexture,
                Projectile.Center - Main.screenPosition + _heldOffset * Projectile.scale,
                itemFrame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0);
        }

        Main.EntitySpriteDraw(
            sheet,
            Projectile.Center - Main.screenPosition,
            new Rectangle(94, 30 * _gripperFrame, 66, 30),
            lightColor,
            Projectile.rotation,
            new Vector2(33f, 3f),
            Projectile.scale,
            SpriteEffects.None,
            0);

        Main.EntitySpriteDraw(
            sheet,
            Projectile.Center - Main.screenPosition,
            new Rectangle(0, 38 * _bodyFrame, 94, 38),
            lightColor,
            Projectile.rotation,
            new Vector2(47f, 25f),
            Projectile.scale,
            SpriteEffects.None,
            0);

        return false;
    }

    public override bool? CanDamage() => false;

    public override void OnKill(int timeLeft)
    {
        if (Main.netMode != NetmodeID.MultiplayerClient)
            TabletDeliveryOrderSystem.NotifyDroneEnded(OrderId, _delivered);
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(_state);
        writer.Write(_timer);
        writer.Write(_delivered);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        _state = reader.ReadByte();
        _timer = reader.ReadUInt16();
        _delivered = reader.ReadBoolean();
    }
}
