sampler uImage0 : register(s0);
float3 uColor;
float uProgress;
float2 uTargetPosition;
float2 uScreenResolution;
float time;
float speed;
float4 color;
bool alwaysVisible;

float4 ShieldPulseShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float coordsLength = length(centeredCoords);
    float4 finalColor = coordsLength;
    if (coordsLength > frac(time * speed))
    {
        return 0;
    }
    float fadeOut = 1 - frac(time * speed);
    if (alwaysVisible)
    {
        fadeOut = 1;
    }
    return finalColor * color * fadeOut;
}

technique Tech1
{
    pass ShieldPulseShader
    {
        PixelShader = compile ps_2_0 ShieldPulseShader();
    }
}