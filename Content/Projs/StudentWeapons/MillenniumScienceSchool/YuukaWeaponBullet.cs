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

namespace KivotosMod.Content.Projs.StudentWeapons.MillenniumScienceSchool
{
    public class YuukaWeaponBullet : KivotosPlayerProjs, IPixelatedRenderer
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
            if (Main.rand.NextBool(4))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 8f * Main.rand.NextFloat(1f, 2f),
                    Color.Lerp(Color.LightSkyBlue, Color.RoyalBlue, Main.rand.NextFloat()), Main.rand.Next(30, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .45f, .2f);
            }
            if (Main.rand.NextBool())
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(10), Projectile.velocity / 8f * Main.rand.NextFloat(.4f, 1f) * 2f,
                    RandLerpColor(Color.RoyalBlue, Color.LightSkyBlue), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.54f, 1f) * 0.85f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(10), RandRotTwoPi.ToRotationVector2() * Main.rand.NextFloat(.4f, 1f) * 10f,
                    RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.54f, 1f) * 0.85f);
            }
            for (int i = 0; i < 3; i++)
            {
                ECSParticle.DigitalNumber(Projectile.Center.ToRandCirclePos(10), RandRotTwoPi.ToRotationVector2() * Main.rand.NextFloat(.4f, 1f) * 10f, RandLerpColor(Color.SkyBlue, Color.LightSkyBlue),
                    Main.rand.Next(30, 51), 1, 0, Main.rand.NextFloat(.9f, 1.05f) * .54f, 0.3f, Main.rand.Next(3, 5));
            }
            float glowScale = 0.14f;
            ECSParticle.CrossGlow(Projectile.Center, Vector2.Zero, Color.RoyalBlue, 20, 1, 0, glowScale);
            ECSParticle.CrossGlow(Projectile.Center, Vector2.Zero, Color.DeepSkyBlue, 20, 1, 0, glowScale * .98f);
            ECSParticle.CrossGlow(Projectile.Center, Vector2.Zero, Color.White, 20, 1, 0, glowScale * .96f);
            for (int i = 0; i < 15; i++)
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(10), -Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(.04f, 1f) * 10f,
                    RandLerpColor(Color.SkyBlue, Color.LightSkyBlue), 40, 1, RandRotTwoPi, Main.rand.NextFloat(.94f, 1f) * 1f);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
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
            Texture2D glowTex = KivotosTextureAssets.Particle_OpticalLineGlow.Value;
            SB.EnterShaderArea();
            float glowScale = Projectile.scale * .25f;
            SB.FastDraw(glowTex, drawPos, Color.RoyalBlue, Projectile.rotation, glowTex.Size() / 2f, glowScale, 0);
            SB.FastDraw(glowTex, drawPos, Color.LightSkyBlue, Projectile.rotation, glowTex.Size() / 2f, glowScale * .6f, 0);
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
                Color c = Color.Lerp(Color.RoyalBlue, Color.Lerp(Color.SkyBlue, Color.White, .5f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.LightSkyBlue, Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, 0.15f), EasingFunction.EaseInOutQuad(progress));
                int by = (int)Lerp(150, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor((byte)(by - 40)) * opac, oldRot, projTex.Size() / 2f, scale * .99f, 0);
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
