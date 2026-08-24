sampler uImage0 : register(s0);
float uTime;
float uProgress;

float4 OutlineShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    if (!any(color))
    {
        float distanceToCheck = 0.02f;
        float4 colorAbove = tex2D(uImage0, coords + float2(0, -distanceToCheck));
        float4 colorBelow = tex2D(uImage0, coords + float2(0, distanceToCheck));
        float4 colorLeft = tex2D(uImage0, coords + float2(-distanceToCheck, 0));
        float4 colorRight = tex2D(uImage0, coords + float2(distanceToCheck, 0));
        if (any(colorLeft) || any(colorRight) || any(colorAbove) || any(colorBelow))
        {
            return 1;
        }

    }
    return color;
}

technique Tech1
{
    pass OutlineShader
    {
        PixelShader = compile ps_2_0 OutlineShader();
    }
}