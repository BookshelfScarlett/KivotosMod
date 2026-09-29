// 标准的流动与淡入淡出shader，用于非噪波的shader
// 最终输出为 边缘透明度 * 输入颜色 * 你要覆盖的颜色 * 噪波的r通道
sampler uTexture1 : register(s0);
// 左侧淡出的长度比例, 例如 0.2 代表最后 20% 的长度会淡出
float uFadeoutLeftLength;
float uFadeinRigtLength;
float uFadeinTopLength;
float uFadeinBottomLength;
float2 UVOffset;
float2 UVMult;

float4 OverlayColor;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates ;
    float4 baseColor = tex2D(uTexture1, coords * UVMult + UVOffset);
    float fadeInL = smoothstep(0.0, uFadeoutLeftLength, coords.x);
    float fadeOutR = smoothstep(1.0, 1.0 - uFadeinRigtLength, coords.x);
    float fadeInT = smoothstep(0.0, uFadeinTopLength, coords.y);
    float fadeOutB = smoothstep(1.0, 1.0 - uFadeinBottomLength, coords.y);
    float alphaMult = 1 * fadeInL * fadeOutR * fadeInT * fadeOutB;
    return OverlayColor * alphaMult * input.Color * baseColor.r;
}

technique SpriteDrawing
{
    pass Pass0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};