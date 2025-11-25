sampler uImage0 : register(s0);
float3 uColor;
float time;
float uProgress;

float4 NauseaShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float sinValue = (sin(time / 10));
    float2 centerCoords = coords * 2.0 - 1;
    float cy = coords.y;

    cy += sin(time / 100 + coords.x * 20) * 0.1;
    
    float4 color = tex2D(uImage0, float2(coords.x * 0.8 + 0.1, cy * 0.8 + 0.1));

    return color;
}

technique Tech1
{
    pass NauseaShader
    {
        PixelShader = compile ps_2_0 NauseaShader();
    }
}