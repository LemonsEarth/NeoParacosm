sampler uImage0 : register(s0);
float3 uColor;
float uProgress;

float4 ScreenTintShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    color.rgb *= (uColor / uProgress);
    return color;
}

technique Tech1
{
    pass ScreenTintShader
    {
        PixelShader = compile ps_2_0 ScreenTintShader();
    }
}