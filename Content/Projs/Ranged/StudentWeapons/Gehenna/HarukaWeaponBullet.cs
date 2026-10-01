using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.PixelatedRender;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class HarukaWeaponBullet : ModProjectile, ILocalizedModType, IPixelatedRenderer
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(8);
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(5))
            {
                ECSParticle.LightntingGlow(Projectile.Center.ToRandCirclePosEdge(8), Projectile.velocity / 8f, RandLerpColor(Color.DarkViolet, Color.Black), 40, 0.36f, 0.34f, 4, blendstate: BlendState.AlphaBlend);
            }
            if (Main.rand.NextBool(8))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 6f,
                                    Color.Lerp(Color.Black, Color.DarkViolet, Main.rand.NextFloat()), Main.rand.Next(30, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, .2f, BlendState.AlphaBlend);
            }

            if (Main.rand.NextBool(6))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6);
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 6f,
                    RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .20f, Main.rand.NextBool());
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), RandRotTwoPi.ToRotationVector2() * Main.rand.NextFloat(.1f, 10f),
                    Color.Lerp(Color.Black, Color.DarkViolet, Main.rand.NextFloat()), Main.rand.Next(30, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, .2f, BlendState.AlphaBlend);
            }
            for (int i = 0; i < 15; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(6), RandRotTwoPi.ToRotationVector2() * Main.rand.NextFloat(.1f, 10f),
                    RandLerpColor(Color.DarkViolet, Color.Black), 40, RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .20f, Main.rand.NextBool());
            }
        }

        public BlendState BlendState => BlendState.AlphaBlend;
        public KivotosDrawLayer LayerToRenderTo => KivotosDrawLayer.BeforeDusts;
        public SpriteBatch SB { get => Main.spriteBatch; }
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            KivotosMethods.EnterShaderAreaPixel(BlendState.AlphaBlend);
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = KivotosTextureAssets.Particle_SharpTear;
            Texture2D projTex = tex;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 ori = projTex.Size() / 2f;
            int drawLength = Projectile.oldPos.Length;
            Texture2D glowTex = KivotosTextureAssets.Particle_OpticalLineGlow.Value;
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
                Color c = Color.Lerp(Color.DarkViolet, Color.Lerp(Color.Black, Color.DarkViolet, .55f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.Black, Color.Lerp(Color.Purple, Color.Violet, 0.15f), EasingFunction.EaseInOutExpo(progress));
                int by = (int)Lerp(150, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor(0) * opac * 1.2f, oldRot, projTex.Size() / 2f, scale * 1.1f, 0);
                //这里重复多画一次。
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor((byte)by) * opac * .6f, oldRot, projTex.Size() / 2f, scale, 0);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(0.5f, 0.5f), pixelColor.ToAddColor(255) * opac, oldRot, projTex.Size() / 2f, scale, 0);
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
