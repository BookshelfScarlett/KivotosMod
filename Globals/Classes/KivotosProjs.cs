using KivotosMod.Globals.Database.Paths;
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
    }
}
