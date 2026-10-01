using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.Trail;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Gehenna
{
    public class HinaWeaponBullet : KivotosPlayerProjs
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(18);
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 2;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(6))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(4), Projectile.velocity / 3f + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1f, 2f),
                    Color.Lerp(Color.DarkViolet, Color.Violet, Main.rand.NextFloat()), Main.rand.Next(30, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, .2f);
            }
            if (Main.rand.NextBool(3))
            {
                ECSParticle.GlowSquare(Projectile.Center.ToRandCirclePos(4), Projectile.velocity / 3f + Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1, 2),
                    Color.Violet.RandLerpColor(Color.DarkViolet), Main.rand.Next(30, 50), 1, Main.rand.NextFloat(TwoPi), Main.rand.NextFloat(.9f, 1.1f) * .6f, 0, Main.rand.NextFloat(-.01f, .01f), .5f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.IsOutScreen())
                return false;
            if (!Projectile.Kivotos().FirstFrame)
                return false;
            Texture2D projTex = Projectile.GetTexture();
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 ori = projTex.Size() / 2f;
            int drawLength = Projectile.oldPos.Length;
            SB.EnterShaderArea(BlendState.NonPremultiplied);
            DrawTrails(KivotosTextureAssets.Trail_BloomDualLine.Texture, Color.DarkViolet, 1f, 1f, 0f);
            SB.EnterShaderArea(BlendState.Additive);
            DrawTrails(KivotosTextureAssets.Trail_BloomDualLine.Texture, Color.Violet, 1f, 1f, 0);
            DrawTrails(KivotosTextureAssets.Trail_BloomDualLine.Texture, Color.Purple, 1f, 1f, 0f);
            int length = Projectile.oldPos.Length;
            SB.EndShaderArea();
            for (int i = 0; i < 8; i++)
                SB.FastDraw(projTex, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + (TwoPi / 8f * i).ToRotationVector2() * 1.5f, Color.Violet.ToAddColor(), Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);
            SB.FastDraw(projTex, drawPos.ToRandCirclePos(1.5f), Color.White, Projectile.rotation, projTex.Size() / 2f, Projectile.scale, 0);

            for (int i = length - 1; i >= 0; i--)
            {
                //if (i <=1)
                //continue;
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i];
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(1f, .65f, (progress));
                float yMult = Lerp(1f, .35f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.DarkViolet, Color.Violet, .5f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Violet, Color.DarkViolet, 0.5f), EasingFunction.EaseInOutQuad(progress));
                int by = (int)Lerp(223, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), pixelColor.ToAddColor((byte)by) * opac, oldRot, projTex.Size() / 2f, scale * .99f, 0);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(2.5f, 2.5f), c.ToAddColor((byte)by) * opac * 0.815f, oldRot, projTex.Size() / 2f, scale * .93f, 0);
            }


            return false;
        }
        public void DrawTrails(Asset<Texture2D> useTex, Color drawColor, float multipleSize = 1f, float alphaValue = 1f, float offsetHeight = 1f)
        {
            float laserLength = 50;
            KivotosShaderAssets.StandardFlowShader.Parameters["LaserTextureSize"].SetValue(useTex.Size());
            KivotosShaderAssets.StandardFlowShader.Parameters["targetSize"].SetValue(new Vector2(laserLength, useTex.Height()));
            KivotosShaderAssets.StandardFlowShader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * offsetHeight);
            KivotosShaderAssets.StandardFlowShader.Parameters["uColor"].SetValue(drawColor.ToVector4() * alphaValue);
            KivotosShaderAssets.StandardFlowShader.Parameters["uFadeoutLength"].SetValue(0.91f);
            KivotosShaderAssets.StandardFlowShader.Parameters["uFadeinLength"].SetValue(0.05f);
            KivotosShaderAssets.StandardFlowShader.CurrentTechnique.Passes[0].Apply();
            if (Projectile.oldPos.Length < 3)
                return;
            //做掉可能存在的零向量
            DrawSetting drawSetting = new DrawSetting(useTex.Value, true);
            List<TrailDrawDate> trailDrawDates = [];
            int posCount = Projectile.oldPos.Length;
            for (int j = 0; j < posCount - 1; j++)
            {
                if (Projectile.oldPos[j].Equals(Vector2.Zero))
                    continue;
                float rot = (Projectile.oldPos[j + 1] - Projectile.oldPos[j]).ToRotation();
                float ratio = j / (posCount - 1);
                Vector2 posOffset = Main.rand.NextVector2Circular(1.5f, 1.5f);
                trailDrawDates.Add(new(Projectile.oldPos[j] + Projectile.Size / 2 + posOffset, drawColor, new Vector2(0, 9 * multipleSize * Projectile.scale), rot));
            }
            TrailRender.DrawTrail([.. trailDrawDates], drawSetting);
        }
    }
}
