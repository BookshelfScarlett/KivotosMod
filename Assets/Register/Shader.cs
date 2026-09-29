using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace KivotosMod.Assets.Register
{
    public class KivotosShaderAssets : ModSystem
    {
        //当未提供特定着色器时，用作基本绘图的默认值。此着色器仅渲染顶点颜色数据，无需修改。
        private const string ShaderPath = "KivotosMod/Assets/Effects/";
        internal const string ShaderPrefix = "KivotosMod";
        public static Effect AlphaFade;
        public static Effect AlphaFadeNoiseColor;
        public static Effect StandardFlowShader;
        public static Effect MetaBallShader;
        public static Effect Pixelation;
        public override void Load()
        {
            if (Main.dedServ)
                return;

            static Effect LoadShader(string path)
            {
                return Request<Effect>($"{ShaderPath}{path}", AssetRequestMode.ImmediateLoad).Value;
            }
            AlphaFade = LoadShader(nameof(AlphaFade));
            AlphaFadeNoiseColor = LoadShader("AlphaFade_Noise_OColor");
            MetaBallShader = LoadShader(nameof(MetaBallShader));
            StandardFlowShader = LoadShader(nameof(StandardFlowShader));
            Pixelation = LoadShader(nameof(Pixelation));

            RegisterMiscShader(AlphaFade, ToPassName(nameof(AlphaFade)), nameof(AlphaFade));
            RegisterMiscShader(StandardFlowShader, ToPassName(nameof(StandardFlowShader)), nameof(StandardFlowShader));
            RegisterMiscShader(MetaBallShader, ToPassName(nameof(MetaBallShader)), nameof(MetaBallShader));
            RegisterMiscShader(AlphaFadeNoiseColor, ToPassName("AlphaFade_Noise_OColor"), "AlphaFade_Noise_OColor");
            RegisterMiscShader(Pixelation, ToPassName(nameof(Pixelation)), nameof(Pixelation));
        }
        public static string ToPassName(string oriShadername) => ShaderPrefix + oriShadername + "Pass";
        public static void RegisterMiscShader(Effect shader, string passName, string registrationName)
        {
            Ref<Effect> shaderPointer = new(shader);
            MiscShaderData passParamRegistration = new(shaderPointer, passName);
            GameShaders.Misc[$"{ShaderPrefix}:{registrationName}"] = passParamRegistration;
        }
        public override void Unload()
        {
            AlphaFade = null;
            AlphaFadeNoiseColor = null;
            MetaBallShader = null;
            StandardFlowShader = null;
            Pixelation = null;
        }
    }
}
