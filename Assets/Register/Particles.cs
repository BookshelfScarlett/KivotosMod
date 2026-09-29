using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public partial class KivotosTextureAssets : ModSystem
    {
        public string ParticlePath => "KivotosMod/Assets/Texture/Particles/";
        public static Tex2DWithPath Particle_HRShinyOrbSmall { get; set; }
        public static Tex2DWithPath Particle_PixelShinyOrb { get; set; }
        public static Tex2DWithPath Particle_Smoke{ get; set; }
        public static Tex2DWithPath Particle_SmokeAlt{ get; set; }
        public static Tex2DWithPath Particle_HRStar{ get; set; }
        public static Tex2DWithPath Particle_HRStarWhite{ get; set; }
        public static Tex2DWithPath Particle_WhiteCube{ get; set; }
        public void LoadParticles()
        {
            Particle_HRShinyOrbSmall = new Tex2DWithPath(ParticlePath + "HRShinyOrbSmall");
            Particle_PixelShinyOrb = new Tex2DWithPath(ParticlePath + "PixelShinyOrb");
            Particle_Smoke = new Tex2DWithPath(ParticlePath + "Smoke");
            Particle_SmokeAlt = new Tex2DWithPath(ParticlePath + "SmokeAlt");
            Particle_HRStar = new Tex2DWithPath(ParticlePath + "HRStar");
            Particle_HRStarWhite = new Tex2DWithPath(ParticlePath + "HRStarWhite");
            Particle_WhiteCube = new Tex2DWithPath(ParticlePath + "WhiteCube");
        }
        public void UnloadParticles()
        {
            Particle_HRShinyOrbSmall = null;
            Particle_PixelShinyOrb = null;
            Particle_Smoke = null;
            Particle_SmokeAlt = null;
            Particle_HRStar = null;
            Particle_HRStarWhite = null;
            Particle_WhiteCube = null;
        }
    }
}
