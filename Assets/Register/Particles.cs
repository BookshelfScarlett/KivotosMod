using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public partial class KivotosTextureAssets : ModSystem
    {
        public string ParticlePath => "KivotosMod/Assets/Texture/Particles/";

        public static Tex2DWithPath Particle_HRShinyOrbSmall { get; set; }
        public static Tex2DWithPath Particle_PixelShinyOrb { get; set; }
        public static Tex2DWithPath Particle_BloodDrop { get; private set; }
        public static Tex2DWithPath Particle_Smoke { get; set; }
        public static Tex2DWithPath Particle_SmokeAlt { get; set; }
        public static Tex2DWithPath Particle_HRStar { get; set; }
        public static Tex2DWithPath Particle_HRStarWhite { get; set; }
        public static Tex2DWithPath Particle_WhiteCube { get; set; }
        public static Tex2DWithPath Particle_CrossGlow { get; set; }
        public static Tex2DWithPath Particle_Fire { get; set; }
        public static Tex2DWithPath Particle_GlowSquare { get; set; }
        public static Tex2DWithPath Particle_GlowSquareBig { get; set; }
        public static Tex2DWithPath Particle_GlowSquareThick { get; set; }
        public static Tex2DWithPath Particle_KiraStar { get; set; }
        public static Tex2DWithPath Particle_KiraStarGlow { get; set; }
        public static Tex2DWithPath Particle_Leafs { get; set; }
        public static Tex2DWithPath Particle_Lightning0 { get; set; }
        public static Tex2DWithPath Particle_Lightning1 { get; set; }
        public static Tex2DWithPath Particle_Lightning2 { get; set; }
        public static Tex2DWithPath Particle_OpticalLineGlow { get; set; }
        public static Tex2DWithPath Particle_Petal { get; set; }
        public static Tex2DWithPath Particle_Ring { get; set; }
        public static Tex2DWithPath Particle_RingHard { get; set; }
        public static Tex2DWithPath Particle_RingShiny { get; set; }
        public static Tex2DWithPath Particle_SharpTearClean { get; set; }
        public static Tex2DWithPath Particle_ThunderBolt { get; set; }
        public static Tex2DWithPath Particle_ExpressionAngry { get; set; }
        public static Tex2DWithPath Particle_ExpressionShock { get; set; }
        public static Tex2DWithPath Particle_ExpressionQuestion { get; set; }
        public static Tex2DWithPath Particle_ExpressionQuestionShock { get; set; }
        public static Tex2DWithPath Particle_ExpressionTired { get; set; }
        public static Tex2DWithPath Particle_ExpressionDrop { get; set; }
        public static Tex2DWithPath Particle_BlueNumberOne { get; set; }
        public static Tex2DWithPath Particle_BlueNumberZero { get; set; }
        public static Tex2DWithPath Particle_TinyNumberOne { get; set; }
        public static Tex2DWithPath Particle_TinyNumberZero { get; set; }
        public static Texture2D Particle_SharpTear => TextureAssets.Extra[ExtrasID.SharpTears].Value;

        public void LoadParticles()
        {
            Particle_HRShinyOrbSmall = new Tex2DWithPath(ParticlePath + "HRShinyOrbSmall");
            Particle_PixelShinyOrb = new Tex2DWithPath(ParticlePath + "PixelShinyOrb");
            Particle_BloodDrop = new Tex2DWithPath(ParticlePath + "BloodDrop");
            Particle_Smoke = new Tex2DWithPath(ParticlePath + "Smoke");
            Particle_SmokeAlt = new Tex2DWithPath(ParticlePath + "SmokeAlt");
            Particle_HRStar = new Tex2DWithPath(ParticlePath + "HRStar");
            Particle_HRStarWhite = new Tex2DWithPath(ParticlePath + "HRStarWhite");
            Particle_WhiteCube = new Tex2DWithPath(ParticlePath + "WhiteCube");
            Particle_CrossGlow = new Tex2DWithPath(ParticlePath + "CrossGlow");
            Particle_Fire = new Tex2DWithPath(ParticlePath + "Fire");
            Particle_GlowSquare = new Tex2DWithPath(ParticlePath + "GlowSquare");
            Particle_GlowSquareBig = new Tex2DWithPath(ParticlePath + "GlowSquareBig");
            Particle_GlowSquareThick = new Tex2DWithPath(ParticlePath + "GlowSquareThick");
            Particle_KiraStar = new Tex2DWithPath(ParticlePath + "KiraStar");
            Particle_KiraStarGlow = new Tex2DWithPath(ParticlePath + "KiraStarGlow");
            Particle_Leafs = new Tex2DWithPath(ParticlePath + "Leafs");
            Particle_Lightning0 = new Tex2DWithPath(ParticlePath + "Lightning0");
            Particle_Lightning1 = new Tex2DWithPath(ParticlePath + "Lightning1");
            Particle_Lightning2 = new Tex2DWithPath(ParticlePath + "Lightning2");
            Particle_OpticalLineGlow = new Tex2DWithPath(ParticlePath + "OpticalLineGlow");
            Particle_Petal = new Tex2DWithPath(ParticlePath + "Petal");
            Particle_Ring = new Tex2DWithPath(ParticlePath + "Ring");
            Particle_RingHard = new Tex2DWithPath(ParticlePath + "RingHard");
            Particle_RingShiny = new Tex2DWithPath(ParticlePath + "RingShiny");
            Particle_SharpTearClean = new Tex2DWithPath(ParticlePath + "SharpTearClean");
            Particle_ThunderBolt = new Tex2DWithPath(ParticlePath + "ThunderBolt");
            Particle_ExpressionAngry = new Tex2DWithPath(ParticlePath + "ExpressionAngry");
            Particle_ExpressionDrop = new Tex2DWithPath(ParticlePath + "ExpressionDrop");
            Particle_ExpressionQuestion = new Tex2DWithPath(ParticlePath + "ExpressionQuestion");
            Particle_ExpressionQuestionShock = new Tex2DWithPath(ParticlePath + "ExpressionQuestionShock");
            Particle_ExpressionAngry = new Tex2DWithPath(ParticlePath + "ExpressionAngry");
            Particle_ExpressionTired = new Tex2DWithPath(ParticlePath + "ExpressionTired");
            Particle_BlueNumberZero = new Tex2DWithPath(ParticlePath + "BlueNumberZero");
            Particle_BlueNumberOne = new Tex2DWithPath(ParticlePath + "BlueNumberOne");
            Particle_TinyNumberZero = new Tex2DWithPath(ParticlePath + "TinyNumberZero");
            Particle_TinyNumberOne = new Tex2DWithPath(ParticlePath + "TinyNumberOne");
        }

        public void UnloadParticles()
        {
            Particle_HRShinyOrbSmall = null;
            Particle_PixelShinyOrb = null;
            Particle_BloodDrop = null;
            Particle_Smoke = null;
            Particle_SmokeAlt = null;
            Particle_HRStar = null;
            Particle_HRStarWhite = null;
            Particle_WhiteCube = null;
            Particle_CrossGlow = null;
            Particle_Fire = null;
            Particle_GlowSquare = null;
            Particle_GlowSquareBig = null;
            Particle_GlowSquareThick = null;
            Particle_KiraStar = null;
            Particle_KiraStarGlow = null;
            Particle_Leafs = null;
            Particle_Lightning0 = null;
            Particle_Lightning1 = null;
            Particle_Lightning2 = null;
            Particle_OpticalLineGlow = null;
            Particle_Petal = null;
            Particle_Ring = null;
            Particle_RingHard = null;
            Particle_RingShiny = null;
            Particle_SharpTearClean = null;
            Particle_ThunderBolt = null;
            Particle_ExpressionTired = null;
            Particle_ExpressionQuestionShock = null;
            Particle_ExpressionQuestion = null;
            Particle_ExpressionDrop = null;
            Particle_ExpressionAngry = null;
            Particle_TinyNumberOne = null;
            Particle_BlueNumberZero = null;
            Particle_BlueNumberOne = null;
            Particle_TinyNumberZero = null;
        }
    }
}
