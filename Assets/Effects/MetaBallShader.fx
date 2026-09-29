// 传入的由粒子拼成的通道材质
sampler renderTargetTexture : register(s0);
// 传入的背景材质
sampler bakcGroundTexture : register(s1);

// 传入的目标大小
// 传入的目标大小最好要和shader应用目标的大小一致，否则会有很奇怪的拉伸
// 原因是
// 虽然alphaTexture的大小与renderTargetSize大小不一致，但是alphaTexture的大小与renderTargetSize最终取UV的时候依然是按照0-1

// 假设我的屏幕大小是1920，那这样就是先动态拼贴出来一张1920 x 1080的背景材质，并且宽高不是具体的像素，而是0 - 1
// 那这样在最终应用到具体的贴图时，贴图绘制会再按照0 - 1取一遍，就导致形成了类似于自适应的拉伸效果

// 传入的目标大小
float2 renderTargetSize;
// 传入的背景材质大小
float2 bakcGroundSize;
// 偏移
float uTime;
// 描边颜色
float4 edgeColor;

float4 MetaBallFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // 根据材质与坐标指定的基础颜色
    float4 baseColor = tex2D(renderTargetTexture, coords);
    
    // 如果没有颜色，则不做处理
    if (!any(baseColor))
        discard;
    
    // 转化为目标的像素坐标
    float2 pixelPos = coords * renderTargetSize;
    // 由于背景材质可能与目标大小不一样，所以需要转换为对应背景的UV坐标
    float2 bgUV = pixelPos / bakcGroundSize;
    bgUV = float2(bgUV.x + uTime * 0.01, bgUV.y);
    // 模一个1限制到0-1之间
    bgUV = frac(bgUV);
    
    // 输出颜色
    float4 outPutColor = tex2D(bakcGroundTexture, bgUV);
    
    // 描边
    // 获取每个像素的正确大小
    float dx = 1 / renderTargetSize.x;
    float dy = 1 / renderTargetSize.y;
    
    bool flag = false;
    // 对周围8格进行判定
    for (int i = -1; i <= 1; i++)
    {
        for (int j = -1; j <= 1; j++)
        {
            float4 c = tex2D(renderTargetTexture, coords + float2(dx * i, dy * j));
            // 如果任何一个像素没有颜色
            if (!any(c))
            {
                // 不知道为啥，这里直接return会被编译器安排，所以只能打标记了
                flag = true;
            }
        }
    }
    
    if (flag)
        return edgeColor;
    
    return outPutColor;
}

// 最终通道
technique Technique1
{
    pass LAPMetalBallPass
    {
        PixelShader = compile ps_2_0 MetaBallFunction();
    }
}