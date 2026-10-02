using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.PixelatedRender;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Highlander
{
    public class NozomiWeaponBullet : KivotosPlayerProjs, IPixelatedRenderer
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(12);
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 5;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
            {
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(4), Projectile.velocity / 6f, RandLerpColor(Color.MidnightBlue, Color.LightGreen), 40,
                    1, Main.rand.NextFloat(.9f, 1.1f) * .4f);
            }
            if (Main.rand.NextBool(3))
            {
                Color c = Main.rand.NextBool() ? RandLerpColor(Color.MidnightBlue, Color.DodgerBlue) : RandLerpColor(Color.LightGreen, Color.Green);
                ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(5), 12.4f * Projectile.velocity.ToRandVelocity(Main.rand.NextFloat(ToRadians(5), ToRadians(15)) * Main.rand.NextFloat(.5f, 1.2f)), c,
                    40, 1, Main.rand.NextFloat(.8f, 1.15f) * .23f, glowMult: .4f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            base.OnHitNPC(target, hit, damageDone);
        }
        public BlendState BlendState => BlendState.AlphaBlend;
        public KivotosDrawLayer LayerToRenderTo => KivotosDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            KivotosMethods.EnterShaderAreaPixel(BlendState.Additive);
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = KivotosTextureAssets.Particle_SharpTear;
            Texture2D projTex = tex;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 ori = projTex.Size() / 2f;
            int drawLength = Projectile.oldPos.Length;
            Texture2D glowTex = KivotosTextureAssets.Particle_HRStarWhite.Value;
            SB.EnterShaderArea();
            float glowScale = Projectile.scale * .20f;
            SB.FastDraw(glowTex, drawPos, Color.MidnightBlue, Projectile.rotation, glowTex.Size() / 2f, glowScale, 0);
            SB.FastDraw(glowTex, drawPos, Color.LightGreen, Projectile.rotation, glowTex.Size() / 2f, glowScale * .86f, 0);
            SB.EndShaderArea();
            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(0.26f, .05f, (progress));
                float yMult = Lerp(1f, .35f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale * 1.5f;
                Color c = Color.Lerp(Color.ForestGreen, Color.Lerp(Color.DarkGreen, Color.Blue, .5f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.Lerp(Color.MidnightBlue, Color.White, .43f), Color.Lerp(Color.DarkGreen, Color.RoyalBlue, 0.5f), EasingFunction.EaseInOutQuad(progress));
                int by = (int)Lerp(150, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor((byte)(by - 40)) * opac, oldRot, projTex.Size() / 2f, scale, 0);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(0.5f, 0.5f), c.ToAddColor(0) * opac * 0.9f, oldRot, projTex.Size() / 2f, scale * .96f, 0);
            }
            KivotosMethods.EndShaderAreaPixel();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.IsOutScreen())
                return false;
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            PixelatedRenderManager.BeginDrawProj = true;
            return false;
        }
    }
}
