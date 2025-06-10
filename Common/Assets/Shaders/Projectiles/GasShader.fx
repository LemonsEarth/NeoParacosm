sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 desiredPos;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float uTime;
float moveSpeed;
float4 uColor;
float4 color;
float velocityX;

float4 GasShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 noiseColor = tex2D(uImage1, float2(coords.x + 0.2 * velocityX, coords.y + uTime * 0.5));
    noiseColor *= color;
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);
    float4 finalColor = noiseColor * (1 - distanceToCenter + 0.2) * (sin(uTime) + 7) * 0.1;
    return finalColor;
}

technique Tech1
{
    pass GasShader
    {
        PixelShader = compile ps_2_0 GasShader();
    }
}