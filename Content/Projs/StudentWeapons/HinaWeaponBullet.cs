using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Classes;
using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Content.Projs.StudentWeapons
{
    internal class HinaWeaponBullet : KivotosPlayerProjs
    {
        public override string LocalizationCategory => LocalizationsDatabase.Projs.StudentWeapons;
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
                ECSParticle.ShinyCrossStarECS(Projectile.Center.ToRandCirclePos(6), Main.rand.NextFloat(TwoPi).ToRotationVector2() * Main.rand.NextFloat(1f, 2f),
                    Color.Lerp(Color.Pink, Color.LightPink, Main.rand.NextFloat()), Main.rand.Next(30, 45), 1, Main.rand.NextFloat(.9f, 1.1f) * .5f, .2f);
            }
            if (Main.rand.NextBool(3))
            {
                Vector2 pos = Projectile.Center.ToRandCirclePos(6);
                ECSParticle.LightntingGlow(pos, Projectile.velocity / 4f, Color.HotPink, 30, 1, 0.44f, 3);
                ECSParticle.LightntingGlow(pos, Projectile.velocity / 4f, Color.White, 30, 1, 0.40f, 3);
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
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.IsOutScreen())
                return false;
            ////这里是强行使用ex98拼凑出来的子弹效果
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Rectangle frame = tex.Frame();
            Vector2 ori = tex.Size() / 2;
            Main.spriteBatch.EnterShaderArea();
            //绘制残影
            float oriScale = .9f;
            Vector2 scale = new(0.56f, 1.6f);
            int length = (int)(Projectile.oldPos.Length);
            for (int i = 0; i < length; i++)
            {
                scale *= 0.965f;
                float rads = (float)i / length;
                Color edgeColor = Color.Lerp(Color.LightPink, Color.HotPink, (1 - rads)).ToAddColor(255) * Clamp(Projectile.velocity.Length(), 0f, 1f);
                Vector2 lerpPos = Vector2.Lerp(Projectile.oldPos[i], Projectile.oldPos[0], 0.20f);
                float rot = Lerp(Projectile.oldRot[i], Projectile.oldRot[0], 1f) + PiOver2;
                Main.spriteBatch.Draw(tex, lerpPos + Projectile.Size / 2f - Main.screenPosition, null, edgeColor, rot, ori, oriScale * scale * Projectile.scale, 0, 0);
            }
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Main.spriteBatch.Draw(tex, pos, null, Color.HotPink, Projectile.rotation + PiOver2, ori, oriScale, 0, 0);
            Main.spriteBatch.EndShaderArea();
            return false;
        }

    }
}
