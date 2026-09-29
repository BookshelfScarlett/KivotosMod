using KivotosMod.Cores.ParticlesECS;
using KivotosMod.Globals.Methods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace KivotosMod.Globals.Graphics.ParticlesECS
{
    public class LightningGlowParticle : ECSParticleBehavior
    {
        public override void OnSpawn(ref ECSParticleData data)
        {
            base.OnSpawn(ref data);
        }
        public override void Update(ref ECSParticleData data)
        {
            data.DrawColor = Color.Lerp(data.DrawColor, Color.Transparent, .105f);
        }
        public override void Draw(ref ECSParticleData data)
        {
            int drawTime = (int)data.aifloat0;
            Vector2 pos = data.Position - Main.screenPosition;
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Vector2 scale = new(.5f, 3.5f);
            for (int i = 0; i < drawTime; i++)
            {
                Vector2 offsetVec = data.Velocity.ToSafeNormalize() * i * 1.4f;
                Main.spriteBatch.Draw(tex, pos + offsetVec, null, data.DrawColor, data.Velocity.ToRotation() + PiOver2, tex.Size() / 2f, data.Scale * scale, 0, 0);
            }
        }
    }
}
