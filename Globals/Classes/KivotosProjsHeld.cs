using KivotosMod.Globals.Methods;
using Terraria;

namespace KivotosMod.Globals.Classes
{
    public abstract class KivotosProjsHeld : KivotosPlayerProjs
    {
        protected virtual int OriginalItemID => -1;
        protected virtual int AttackSpeed => Owner.ApplyWeaponAttackSpeed(Owner.HeldItem, Owner.HeldItem.useTime * Projectile.MaxUpdates, 5 * Projectile.MaxUpdates);
        protected virtual int ExtraUpdates => 0;
        public bool OwnerIsLegal => Owner.IsHolding(OriginalItemID) && !Owner.CCed && !Owner.noItems&&!Owner.dead;
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.SetUpHeldProj(ExtraUpdates);
            Projectile.DamageType = SetDamageClass;
        }
        public override void AI()
        {
            //玩家手持的武器不对会随时弄死这个东西
            UpdatePlayerState(out bool isKilled);
            if (isKilled)
                return;
            if(!Projectile.Kivotos().FirstFrame)
                OnFirstFrame();
            ProjAI();

        }
        protected virtual void UpdatePlayerState(out bool isKilled)
        {
            isKilled = false;
            if (OwnerIsLegal)
                Projectile.timeLeft = 2;
            else
                isKilled = true;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? CanDamage()
        {
            return null;
        }
    }
}
