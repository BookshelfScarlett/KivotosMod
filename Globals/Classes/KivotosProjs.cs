using KivotosMod.Globals.Database.Paths;
using KivotosMod.Globals.Methods;
using Terraria;
using Terraria.ModLoader;

namespace KivotosMod.Globals.Classes
{
    public abstract class KivotosPlayerProjs : ModProjectile, ILocalizedModType
    {
        protected virtual DamageClass SetDamageClass => DamageClass.Generic;
        public Player Owner => Main.player[Projectile.owner];
        private string GetLocalizationCategory()
        {
            if (SetDamageClass.CountsAsClass<MeleeDamageClass>())
                return (string)LocalizationsDatabase.Projs.MeleeProj;
            else if (SetDamageClass.CountsAsClass<RangedDamageClass>())
                return (string)LocalizationsDatabase.Projs.RangedProj;
            else
                return LocalizationsDatabase.ProjLocalization;
        }
        public override string LocalizationCategory => GetLocalizationCategory();
        public override string Texture => KivotosContent.GetAsset(KivotosContent.Projs, GetType().Name);
        public SpriteBatch SB { get => Main.spriteBatch; }
        public GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public virtual void OnFirstFrame() { }
        public virtual void ProjAI() { }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = SetDamageClass;
        }
        public override void AI()
        {
            if (!Projectile.Kivotos().FirstFrame)
                OnFirstFrame();
            ProjAI();
        }
    }
}
