using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.PixelatedRender;
using KivotosMod.Cores.Trail;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Millennium
{
    /// <summary>
    /// 复制的小绿的子弹，改了个色
    /// </summary>
    public class MomoiWeaponBullet : KivotosPlayerProjs, IPixelatedRenderer
    {
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(16);
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.MaxUpdates = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.width = Projectile.height = 16;
        }
        public override void OnFirstFrame()
        {
            base.OnFirstFrame();
        }
        public override void ProjAI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.IsOutScreen())
                return;
            if (Main.rand.NextBool(4))
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePosEdge(5f),
                    Projectile.velocity.ToRandVelocity(ToRadians(15), 1), RandLerpColor(Color.HotPink, Color.LightPink), 40, 1f, .66f, .2f);
            if (Main.rand.NextBool(4))
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(8), Projectile.velocity / 4f, RandLerpColor(Color.DeepPink, Color.HotPink),
                    40, 0.86f, RandRotTwoPi, Main.rand.NextFloat(.9f, 1.1f) * .2f, 1);
            if (Main.rand.NextBool(4))
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(6), Projectile.velocity / 8, RandLerpColor(Color.HotPink, Color.DeepPink), Main.rand.Next(30, 51),
                    RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .20f, Main.rand.NextBool(), blendstate: BlendState.Additive);
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center, RandVelTwoPi(1, 7), RandLerpColor(Color.Pink, Color.HotPink), Main.rand.Next(30, 51), RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .145f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(60) - Projectile.SafeDir() * 1.2f, Main.rand.NextFloat(.9f, 1.1f) * 1f,
                    RandLerpColor(Color.LightPink, Color.HotPink), Main.rand.Next(30, 41), 1, Main.rand.NextFloat(.8f, 1.12f) * .1f, glowMult: .6f);

            }
            for (int i = 0; i < 6; i++)
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(3), RandVelTwoPi(1, 9), RandLerpColor(Color.Pink, Color.HotPink), 40, 1, RandRotTwoPi, 0.32f, 1);
            Projectile.timeLeft -= 100;
            Projectile.BounceOnTile(oldVelocity);
            return false;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 8; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center, RandVelTwoPi(1, 7), RandLerpColor(Color.HotPink, Color.DeepPink), Main.rand.Next(30, 51), RandRotTwoPi, 1, Main.rand.NextFloat(.9f, 1.1f) * .145f, Main.rand.NextBool(), BlendState.Additive);
            }
            for (int i = 0; i < 16; i++)
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(60) - Projectile.SafeDir() * 1.2f, Main.rand.NextFloat(.9f, 1.1f) * 1f,
                    RandLerpColor(Color.DeepPink, Color.HotPink), Main.rand.Next(30, 41), 1, Main.rand.NextFloat(.8f, 1.12f) * .1f, glowMult: .6f);

            }
            for (int i = 0; i < 6; i++)
                ECSParticle.ShrinkParticle(Projectile.Center.ToRandCirclePos(3), RandVelTwoPi(1, 9), RandLerpColor(Color.HotPink, Color.DeepPink), 40, 1, RandRotTwoPi, 0.32f, 1);

            base.OnHitNPC(target, hit, damageDone);
        }
        public BlendState BlendState => BlendState.AlphaBlend;
        public KivotosDrawLayer LayerToRenderTo => KivotosDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            if (!Projectile.Kivotos().FirstFrame)
                return;

            KivotosMethods.EnterShaderAreaPixel(BlendState.Additive);
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = KivotosTextureAssets.Particle_SharpTear;
            Texture2D projTex = tex;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D glowTex = KivotosTextureAssets.Particle_OpticalLineGlow.Value;
            KivotosMethods.EnterShaderAreaPixel(BlendState.Additive);
            float glowScale = Projectile.scale * .15f;
            SB.FastDraw(glowTex, drawPos, Color.DeepPink, Projectile.rotation, glowTex.Size() / 2f, glowScale, 0);
            SB.FastDraw(glowTex, drawPos, Color.White, Projectile.rotation, glowTex.Size() / 2f, glowScale * .6f, 0);

            int length = Projectile.oldPos.Length;
            for (int i = length - 1; i >= 0; i--)
            {
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], .2f);
                Vector2 oldPos = lerpPos - Main.screenPosition + Projectile.Size / 2f;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                //图竖直
                float progress = i / (float)length;
                float xMult = Lerp(0.7f, .05f, (progress));
                float yMult = Lerp(1f, .2f, progress);
                Vector2 scale = new Vector2(xMult, yMult) * Projectile.scale * 1.2f;
                Color c = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.DeepPink, .75f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.White, Color.Lerp(Color.Pink, Color.DeepPink, 0.85f), EasingFunction.EaseInOutExpo(progress));
                int by = (int)Lerp(50, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor(0) * opac * 1.2f, oldRot, projTex.Size() / 2f, scale * 1.1f, 0);
                //这里重复多画一次。
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor((byte)by) * opac * .6f, oldRot, projTex.Size() / 2f, scale, 0);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(0.5f, 0.5f), pixelColor.ToAddColor(100) * opac, oldRot, projTex.Size() / 2f, scale, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.DeepPink, 10);
            SB.EnterShaderArea();
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.HotPink, 10);
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.White * .5f, 6);
            SB.EndShaderArea();

            KivotosMethods.EndShaderAreaPixel();
        }
        public void TrailFunc(Texture2D tex, Color color, float primitiveHeight = 30, float heightPosOffset = 0f)
        {
            float laserLength = (int)Projectile.localAI[1];
            Effect shader = KivotosShaderAssets.StandardFlowShader;
            shader.Parameters["LaserTextureSize"].SetValue(tex.Size());
            shader.Parameters["targetSize"].SetValue(new Vector2(laserLength, tex.Height));
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * -100);
            shader.Parameters["uColor"].SetValue(color.ToVector4() * 1);
            shader.Parameters["uFadeoutLength"].SetValue(1.13f);
            shader.Parameters["uFadeinLength"].SetValue(0.2f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < (Projectile.oldPos.Length); i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2;
                date.Add(new(listPos, Color.White, new(0, primitiveHeight), Projectile.oldRot[i]));
            }
            TrailRender.DrawTrail(date.ToArray(), sets);
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
