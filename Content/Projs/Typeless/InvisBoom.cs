using KivotosMod.Assets.Register;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Typeless
{
    public class InvisBoom : KivotosPlayerProjs
    {
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override string LocalizationCategory => LocalizationsDatabase.Projs.TypelessProj;

        private int ApplyBuffID = -1;
        private int ApplyBuffFrame = 60;
        private int LifeTime = 40;
        private int LocalNPCHitCooldown = 40;
        private int Penetrate = -1;
        private NPC MountedTarget = null;
        private DamageClass DamageType = DamageClass.Generic;
        public void SetUpBoom(int buffID, int buffFrame)
        {
            ApplyBuffID = buffID;
            ApplyBuffFrame = buffFrame;
        }
        public void SetUpBoom(int buffID, int buffFrame, int lifeTime, int npcHitCooldown, int penetrate, DamageClass damageType, NPC mountedTarget = null)
        {
            ApplyBuffID = buffID;
            ApplyBuffFrame = buffFrame;
            LifeTime = lifeTime;
            LocalNPCHitCooldown = npcHitCooldown;
            Penetrate = penetrate;
            DamageType = damageType;
            MountedTarget = mountedTarget;
        }

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.timeLeft = LifeTime;
            Projectile.localNPCHitCooldown = LocalNPCHitCooldown;
            Projectile.penetrate = Penetrate;
            Projectile.DamageType = DamageType;
            Projectile.tileCollide = false;
        }
        public override void ProjAI()
        {
            if (MountedTarget.IsLegal())
                Projectile.Center = MountedTarget.Center;
            base.ProjAI();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ApplyBuffID != -1 && ApplyBuffFrame > 0)
            {
                target.AddBuff(ApplyBuffID, ApplyBuffFrame);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
