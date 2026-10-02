using KivotosMod.Assets.Register;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged
{
    public class GeneralAssaultRifleBullet : KivotosPlayerProjs
    {
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override string LocalizationCategory => LocalizationsDatabase.Projs.RangedProj;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.MaxUpdates = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 2;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public float OverallScale = 0;
        public ref float AniTimer => ref Projectile.localAI[0];
        public override void ProjAI()
        {
            float maxAniTime = 2 * Projectile.MaxUpdates;
            AniTimer++;
            float progress = Utils.GetLerpValue(0, maxAniTime, AniTimer,true);
            OverallScale = Lerp(OverallScale, Lerp(OverallScale, 1f, .3f), progress);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool())
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center.ToRandCirclePos(10), DustID.GoldCoin);
                d.velocity = Projectile.velocity / 4f;
                d.noGravity = true;
            }
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsFinalHit())
                return;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            if (Projectile.IsOutScreen())
                return false;
            Texture2D tex = KivotosTextureAssets.Particle_SharpTear;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.rotation + PiOver2;
            Vector2 scale = new Vector2(OverallScale*.4f, 1.5f) * OverallScale;
            int length = Projectile.oldPos.Length-2;
            //逆向遍历
            for(int i =length-1;i>=0;i--)
            {
                float progress = (float)i / length;
                Vector2 oldPos = Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                Vector2 scaleMult = scale * (1- progress);
                float opac = Lerp(.54f, 1f, EasingFunction.EaseInOutExpo(progress));
                Color c = Color.Lerp(Color.Goldenrod, Color.DarkGoldenrod, progress) * opac;
                SB.FastDraw(tex, oldPos.ToRandCirclePos(1.5f), c.ToAddColor(), oldRot, tex.Size() / 2f, scaleMult*1.1f, 0);
                Color pixelC = Color.Lerp(Color.LightGoldenrodYellow, Color.White, progress) * opac;
                SB.FastDraw(tex, oldPos, pixelC.ToAddColor(100), oldRot, tex.Size() / 2f, scaleMult, 0);

            }
            return false;
        }
    }
}
