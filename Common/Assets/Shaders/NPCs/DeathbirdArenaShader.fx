sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 desiredPos;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float uProgress;
float time;

float4 DeathbirdArenaShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    float2 normalizedPixelCoords = (uScreenResolution / uScreenResolution.y);
    float noiseScale = 0.25f;
    float4 noiseColor = tex2D(uImage1, coords * noiseScale * normalizedPixelCoords + float2(0, time * 0.5));
    float2 centeredCoords = coords * 2.0 - 1.0;
    float2 targetPos = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 coordsToPos = (targetPos - coords) * normalizedPixelCoords;
    float distanceToPos = length(coordsToPos);
    float threshold = 0.5;
    float luminosity = (noiseColor.r + noiseColor.g + noiseColor.b) / 3;
    if (distanceToPos > threshold && distanceToPos < 3)
    {
        float sinValue = (sin(time * 2) + 1) * 0.1;
        float colorMult = (distanceToPos - threshold - sinValue);
        return baseColor + ((1 + noiseColor.r) * colorMult) * uProgress;
    }
    return baseColor;
}

technique Tech1
{
    pass DeathbirdArenaShader
    {
        PixelShader = compile ps_2_0 DeathbirdArenaShader();
    }
}