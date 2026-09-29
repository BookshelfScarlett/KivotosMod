using KivotosMod.Assets.Register;
using KivotosMod.Content.Raritys.Helper;
using KivotosMod.Globals.Methods;
using ReLogic.Content;
using Terraria;

namespace KivotosMod.Content.Raritys.Sparkles
{

    public class RaritySmoke : RaritySparkle
    {
        public bool UseAlt;
        public bool UseAdd;
        public RaritySmoke(Vector2 position, Vector2 velocity, Color color, int lifetime, float Rot, float opacity, float scale, bool useAlt = false, bool useAdd = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Lifetime = lifetime;
            Scale = scale;
            Rotation = Rot;
            Opacity = opacity;
            UseAlt = useAlt;
            UseAdd = useAdd;
        }

        public override void CustomUpdate()
        {
            Velocity *= 0.9f;
            Opacity = Lerp(Opacity, Lerp(Opacity, 0, 0.3f), 0.12f);
            Position += Velocity;
        }
        public override void CustomDraw(SpriteBatch spriteBatch, Vector2 drawPosition)
        {
            //float brightness = (float)Math.Pow(Lighting.Brightness((int)(Position.X / 16f), (int)(Position.Y / 16f)), 0.15);
            Asset<Texture2D> texture = UseAlt ? KivotosTextureAssets.Particle_SmokeAlt.Texture : KivotosTextureAssets.Particle_Smoke.Texture;

            Rectangle frame = texture.Frame(4, 4, (int)(LifetimeRatio * 16) % 4, (int)(LifetimeRatio * 4));
            Vector2 origin = frame.Size() * 0.5f;
            if (UseAdd)
                spriteBatch.Draw(texture.Value, drawPosition, frame, DrawColor.ToAddColor() * Opacity, Rotation, origin, Scale, 0, 0f);
            else
                spriteBatch.Draw(texture.Value, drawPosition, frame, DrawColor * Opacity, Rotation, origin, Scale, 0, 0f);
        }
    }
}
