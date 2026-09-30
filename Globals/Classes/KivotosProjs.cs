using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Classes
{
    public abstract class KivotosPlayerProjs : ModProjectile, ILocalizedModType
    {
        public Player Owner => Main.player[Projectile.owner];
        public override string LocalizationCategory => LocalizationsDatabase.ProjLocalization;
        public override string Texture => KivotosContent.GetAsset(KivotosContent.Projs, GetType().Name);
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public virtual void OnFirstFrame() { }
        public virtual void ProjAI() { }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
        }
        public override void AI()
        {
            if (!Projectile.Kivotos().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
    }
}
