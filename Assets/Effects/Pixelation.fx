sampler uImage0 : register(s0);
// 画面呈现的低分辨率大小
float2 uTargetResolution;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 pixelatedCoords = floor(input.TextureCoordinates * uTargetResolution) / uTargetResolution;
    return tex2D(uImage0, pixelatedCoords) * input.Color;
}

technique SpriteDrawing
{
    pass Pass0
    {
        PixelShader = compile ps_3_0 MainPS();
    }
};