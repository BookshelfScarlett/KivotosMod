using KivotosMod.Assets.Register;
using KivotosMod.Content.Projs.Typeless;
using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Cores.PixelatedRender;
using KivotosMod.Cores.ScreenEffect;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Enums;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Graphics;
using KivotosMod.Globals.Methods;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.Ranged.StudentWeapons.Millennium
{
    public class YuzuWeaponBullet : KivotosPlayerProjs, IPixelatedRenderer
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
        public override string Texture => KivotosTextureAssets.InvisAsset.Path;
        public override void SetStaticDefaults()
        {
            Projectile.ToTrailSetting(23);
        }
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.MaxUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
        }
        public float OverallScale = 0;
        public ref float Timer => ref Projectile.localAI[0];
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 32;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }
        public override void AI()
        {
            float timeToMaxSize = 15f * Projectile.MaxUpdates;
            float aniProgress = Utils.GetLerpValue(0, timeToMaxSize, Timer, true);
            OverallScale = Lerp(0f, 1f, EasingFunction.EaseOutCubic(aniProgress));
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            //复制原版双足翼龙怒焰的AI
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 10f)
            {
                Projectile.velocity.Y += 0.1f;
            }
            if (Projectile.ai[0] >= 20f)
            {
                Projectile.velocity.Y += 0.1f;
            }
            if (Projectile.ai[0] > 20f)
            {
                Projectile.ai[0] = 20f;
            }
            Projectile.velocity.X *= 0.99f;
            if (Projectile.velocity.Y > 32f)
            {
                Projectile.velocity.Y = 32f;
            }

            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            if (Projectile.IsOutScreen())
                return;
            ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(8), Projectile.velocity, RandLerpColor(Color.Goldenrod, Color.OrangeRed), Main.rand.Next(30, 51),
                RandRotTwoPi, 1, Main.rand.NextFloat(.8f, 1.15f) * .51f, false, BlendState.Additive);
            if (Main.rand.NextBool(3))
            {
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Projectile.velocity.ToRandVelocity(ToRadians(10), 2, 9),
                    RandLerpColor(Color.OrangeRed, Color.DarkOrange), 40, 1, Main.rand.NextFloat(.9f, 1.1f) * .73f, .2f);
            }
            if(Main.rand.NextBool(6))
            {
                ECSParticle.TurbulenceShinyOrb(Projectile.Center.ToRandCirclePos(10), Main.rand.NextFloat(.5f, 1.2f) * 3f, RandLerpColor(Color.Orange, Color.DarkOrange), Main.rand.Next(30, 51),
                    1, Main.rand.NextFloat(.85f, 1.15f)*.20f, RandRotTwoPi, .4f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
        }
        public override void OnKill(int timeLeft)
        {
            if (Projectile.IsFinalHit())
                return;
            SpawnExplosion();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SpawnExplosion();
            base.OnHitNPC(target, hit, damageDone);
        }
        public void SpawnExplosion()
        {
            Projectile.SpawnInvisBoom(new KivotosMethods.InvisBoomOptions 
            { Resize = 400,DamageClass = DamageClass.Ranged, BuffID = BuffID.OnFire, BuffTime = 60, LifeTime = 60, HitCooldown = 5 });
                        Vector2 safeDir = Projectile.rotation.ToRotationVector2();
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 4; j++)
                    ECSParticle.StarShape(Projectile.Center.ToRandCirclePos(3f), safeDir.RotatedBy(PiOver2 * j + PiOver4) * Main.rand.NextFloat(.1f, 19f), RandLerpColor(Color.Orange, Color.WhiteSmoke), 30, 1f, 1.091f);
            }
            for (int i = 0; i < 80; i++)
            {
                if (i % 2 == 0)
                    ECSParticle.HRShinyOrb(Projectile.Center.ToRandCirclePos(3f), RandVelTwoPi(1f, 24f), RandLerpColor(Color.Orange, Color.DarkOrange), 30, 1f, Main.rand.NextFloat(.7f, 1.3f) * .1f, 0.45f);
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(2f, 21f), RandLerpColor(Color.Orange, Color.OrangeRed), 45, 1f, Main.rand.NextFloat(.75f, 1.3f) * 1.1f, 0.13f);
            }
            for (int i = 0; i < 70; i++)
            {
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(7f, 28), RandLerpColor(Color.DarkOrange, Color.Orange), 40, RandRotTwoPi, .21f, 0.87f * Main.rand.NextFloat(0.8f, 1.1f), false,BlendState.AlphaBlend);
                ECSParticle.SmokeParticle(Projectile.Center.ToRandCirclePos(5f), RandVelTwoPi(4f, 28f), RandLerpColor(Color.Orange, Color.OrangeRed), 20, RandRotTwoPi, .41f, 1.09f * Main.rand.NextFloat(0.8f, 1.1f), true, BlendState.Additive);
            }
            //float squareSplitScale = .80f;
            ScreenShakeSystem.AddScreenShakes(Projectile.Center, 16f, 10, RandRotTwoPi);
            SoundEngine.PlaySound(KivotosSoundsAssets.SharpBoomHeavy, Projectile.Center);

        }
        public BlendState BlendState => BlendState.AlphaBlend;
        public KivotosDrawLayer LayerToRenderTo => KivotosDrawLayer.BeforeDusts;
        public void RenderPixelated(SpriteBatch spriteBatch)
        {
            KivotosMethods.EnterShaderAreaPixel(BlendState.Additive);
            Texture2D FireBall = KivotosTextureAssets.Texture_Fireball.Value;
            Texture2D FireBallPixel = KivotosTextureAssets.Texture_FireballPixel.Value;
            Texture2D Glow = KivotosTextureAssets.Particle_HRShinyOrbSmall.Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rot = Projectile.velocity.ToRotation() + PiOver2;
            //总控射弹的大小
            //超级复杂的数学计算，这里是反复热重载试的
            Vector2 totalScale = new Vector2(OverallScale, 1f) * Projectile.scale * 0.85f * 1.43f;

            Color betweenGold = Color.Lerp(Color.Gold, Color.OrangeRed, 0.6f);

            float overallAlpha = 1;
            SB.EnterShaderArea();
            SB.FastDraw(Glow, drawPos, Color.DarkGoldenrod* overallAlpha * 0.82f, rot, Glow.Size() / 2f, totalScale * .33f, SpriteEffects.None);
            SB.EndShaderArea();
            Color outerCol = Color.Orange * 0.4f;
            SB.FastDraw(FireBall, drawPos, outerCol.ToAddColor() * overallAlpha, rot, FireBall.Size() / 2f, totalScale, SpriteEffects.None);
            int length = Projectile.oldPos.Length - 5;
            for (int i = 0; i < length; i++)
            {
                Vector2 pos = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                //oldPos[0]存储的是最新的位置，oldPos[1]是上一个位置，依次类推，所以这里的progress是从1到0的变化
                float progress = 1 - (float)i / length;
                //根据progress计算出每个位置的大小和颜色，越靠近尾部的点越小
                //根据progress计算出每个位置的颜色，越靠近尾部的点越淡，这里暂时只有progress
                float colVal = progress;
                //根据progress计算出每个位置的颜色，越靠近尾部的点越偏紫
                Color col = Color.Lerp(Color.DarkRed* 3f, betweenGold, EasingFunction.EaseInOutQuad(progress)) * progress * 0.7f;
                //底图的大小，超级复杂的数学计算，这里是反复热重载试的
                Vector2 size2 = (1f - (progress * 0.15f)) * totalScale;
                //底图的位置，这里会有一个反复出现的随机偏移，越靠近尾部的点偏移越小。用来表现炮弹的震动动态
                Vector2 lowerLayerFireballPos = pos + Main.rand.NextVector2Circular(10f, 10f) * (1f - progress);
                Color lowerLayerFireballColor = col.ToAddColor() * 0.85f * overallAlpha * colVal;
                float oldRot = Projectile.oldRot[i] + PiOver2;
                SB.FastDraw(FireBallPixel, lowerLayerFireballPos, lowerLayerFireballColor,
                        oldRot, FireBallPixel.Size() / 2f, size2, SpriteEffects.None);

                Vector2 size = (1f - (progress * 0.9f)) * totalScale * new Vector2(.25f, 1.15f);
                Vector2 upperLayerFireballPos = pos;
                Color upperLayerFireballColor = col.ToAddColor() * 1.25f * overallAlpha * colVal;
                SB.FastDraw(FireBall, upperLayerFireballPos, upperLayerFireballColor,oldRot, FireBall.Size() / 2f, size* 1.5f, SpriteEffects.None);
            }
            Vector2 v2scale = new Vector2(1f, 0.8f);
            Vector2 upperLayerMainFireballPos = drawPos+Main.rand.NextVector2Circular(5,5);
            SB.FastDraw(FireBall, upperLayerMainFireballPos, betweenGold.ToAddColor() * overallAlpha * 0.75f, rot, FireBall.Size() / 2f, totalScale * v2scale, SpriteEffects.None);
            SB.FastDraw(FireBall, drawPos, Color.White.ToAddColor() * overallAlpha, rot, FireBall.Size() / 2f, v2scale * totalScale * 0.6f, SpriteEffects.None);


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
