using KivotosMod.Assets.Register;
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

namespace KivotosMod.Content.Projs.StudentWeapons.MillenniumScienceSchool
{
    public class MidoriWeaponBullet : KivotosPlayerProjs, IPixelatedRenderer
    {
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Projectile.ToTrailSetting(26);
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
            base.ProjAI();
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
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
        public SpriteBatch SB { get => Main.spriteBatch; }
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
            KivotosMethods.EnterShaderAreaPixel(BlendState.Additive);
            float glowScale = Projectile.scale * .25f;
            SB.FastDraw(glowTex, drawPos, Color.LimeGreen, Projectile.rotation, glowTex.Size() / 2f, glowScale, 0);
            SB.FastDraw(glowTex, drawPos, Color.White, Projectile.rotation, glowTex.Size() / 2f, glowScale * .6f, 0);

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
                Color c = Color.Lerp(Color.DarkGreen, Color.Lerp(Color.Green, Color.Lime, .55f), EasingFunction.EaseInOutQuad(progress));
                float opac = Lerp(1f, .79f, EasingFunction.EaseInOutExpo(progress));
                Color pixelColor = Color.Lerp(Color.Green, Color.Lerp(Color.Lime, Color.LightGreen, 0.15f), EasingFunction.EaseInOutExpo(progress));
                int by = (int)Lerp(150, 0, progress);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor(0) * opac * 1.2f, oldRot, projTex.Size() / 2f, scale * 1.1f, 0);
                //这里重复多画一次。
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(1.5f, 1.5f), c.ToAddColor((byte)by) * opac * .6f, oldRot, projTex.Size() / 2f, scale, 0);
                SB.FastDraw(projTex, oldPos + Main.rand.NextVector2Circular(0.5f, 0.5f), pixelColor.ToAddColor(255) * opac, oldRot, projTex.Size() / 2f, scale, 0);
            }
            SB.EnterShaderArea(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.DarkGreen, 10);
            SB.EnterShaderArea();
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.DarkGreen, 10);
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.Green, 8);
            TrailFunc(KivotosTextureAssets.Trail_ManaStreak.Value, Color.White, 6);
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
            shader.Parameters["uFadeinLength"].SetValue(0.052f);
            shader.CurrentTechnique.Passes[0].Apply();

            DrawSetting sets = new(tex);
            List<TrailDrawDate> date = [];
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                Vector2 listPos = Projectile.oldPos[i] + Projectile.Size / 2 - Projectile.SafeDir() * 10f + Projectile.SafeDir().RotatedBy(PiOver2) * heightPosOffset;
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
