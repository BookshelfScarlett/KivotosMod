using KivotosMod.Assets.Register;
using KivotosMod.Cores.ParticlesECS;
using System;
using Terraria;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class StarShapeECS : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void Update(ref ECSParticleData data)
        {
            bool customRotation = data.aifloat1 != 0;
            data.Scale *= .95f;
            data.Opacity = Lerp(data.Opacity, 0f, (float)Math.Pow(data.LifetimeRatio, 3D));
            //data.DrawColor *= Lerp(1f, 0f, );
            data.Velocity *= .95f;
            data.Rotation = customRotation ? data.Rotation : data.Velocity.ToRotation();
        }
        public override void Draw(ref ECSParticleData data)
        {
            bool drawGlow = data.aifloat0 > 0;
            Vector2 scale = new Vector2(.5f, 1.6f) * data.Scale;
            Texture2D tex = KivotosTextureAssets.Particle_SharpTear;
            Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, data.DrawColor * data.Opacity, data.Rotation + PiOver2, tex.Size() / 2f, scale, 0, 0);
            if (drawGlow)
                Main.spriteBatch.Draw(tex, data.Position - Main.screenPosition, null, Color.White * data.Opacity, data.Rotation + PiOver2, tex.Size() / 2f, scale * data.aifloat0, 0, 0);
        }
    }
}
