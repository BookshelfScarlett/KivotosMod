// 传入的背景材质
sampler FlowTexture : register(s0);
float2 FlowTextureSize;
// 长度
float2 targetSize;
// 偏移
float uTime;
// 用于染色的颜色 (从C#代码传入)
float4 uColor;
// 末端淡出的长度比例, 例如 0.2 代表最后 20% 的长度会淡出
float uFadeoutLength;
// 头部淡入的长度比例, 例如 0.1 代表最前 10% 的长度会淡入
float uFadeinLength;

float4 StandarFlowFunction(float2 coords : TEXCOORD0) : COLOR0
{ 
    // 根据材质与坐标指定的基础颜色
    // 转化为目标的像素坐标
    float2 pixelPos = coords * targetSize;
    // 将对应像素坐标转换为激光材质的UV坐标
    float2 bgUV = pixelPos / FlowTextureSize;
    // 流动
    bgUV = float2(bgUV.x + uTime * 0.01, bgUV.y);
    // 模一个1限制到0-1之间
    bgUV = frac(bgUV);
   // 采样流动的激光颜色
    float4 finalColor = tex2D(FlowTexture, bgUV);

    // 1. 应用染色
    // 将采样出的颜色与我们传入的 uColor 相乘
    // finalColor.rgb *= uColor.rgb; // 仅染色RGB通道
    finalColor *= uColor; // 染色的同时，也受 uColor 的 Alpha 值影响
    // 2. 计算淡入和淡出
    // 计算淡入: 从 0.0 到 uFadeinLength 的位置，alpha 从 0 平滑过渡到 1
    float fadeIn = smoothstep(0.0, uFadeinLength, coords.x);
    // 计算淡出: 从 (1.0 - uFadeoutLength) 到 1.0 的位置，alpha 从 1 平滑过渡到 0
    float fadeOut = smoothstep(1.0, 1.0 - uFadeoutLength, coords.x);
    // 将计算出的 fade 值应用到最终颜色的 Alpha 通道上
    finalColor.a *= fadeIn * fadeOut;
    return finalColor;
}

// 最终通道
technique Technique1
{
    pass UCAStandardFlowPass
    {
        PixelShader = compile ps_2_0 StandarFlowFunction();
    }
}