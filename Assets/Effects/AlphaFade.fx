// 用于Alpha裁切的材质
sampler uTexture1 : register(s0);
// 左侧淡出的长度比例, 例如 0.2 代表最后 20% 的长度会淡出
float uFadeoutLeftLength;
float uFadeinRigtLength;
float uFadeinTopLength;
float uFadeinBottomLength;
float2 UVOffset;
float2 UVMult;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coords = input.TextureCoordinates;
    float4 baseColor = tex2D(uTexture1, coords * UVMult + UVOffset);
    float fadeInL = smoothstep(0.0, uFadeoutLeftLength, coords.x);
    float fadeOutR = smoothstep(1.0, 1.0 - uFadeinRigtLength, coords.x);
    float fadeInT = smoothstep(0.0, uFadeinTopLength, coords.y);
    float fadeOutB = smoothstep(1.0, 1.0 - uFadeinBottomLength, coords.y);
    baseColor.a *= fadeInL * fadeOutR * fadeInT * fadeOutB;
    return baseColor * input.Color;
}

technique SpriteDrawing
{
    pass Pass0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};