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
float4 uOpacity;

float4 RingShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);
    distanceToCenter = clamp(distanceToCenter, 0, 1);
    float4 baseColor = tex2D(uImage0, coords);
    float4 noiseColor1 = tex2D(uImage1, coords);
    float halfUTime = uTime / 2.0;
    float2 polarCoords = float2(atan2(centeredCoords.y + uTime, centeredCoords.x) + halfUTime / 4, distanceToCenter * uTime / 100.0 + noiseColor1.r);
    float4 noiseColor2 = tex2D(uImage1, polarCoords);
    
    float innerRingDistance = 0.3;
    if (distanceToCenter < innerRingDistance)
    {
        return 0;
    }
    
    if (distanceToCenter > 0.8)
    {
        return lerp(0.7, 0, (distanceToCenter - 0.8) / 0.2);
    }
          
    float distanceToInnerRing = abs(distanceToCenter - innerRingDistance);
    float distanceToOuterRing = abs(distanceToCenter - 0.8);
    if (distanceToInnerRing < 0.03 || distanceToOuterRing < 0.03)
    {
        return uColor;
    }
    float4 finalColor = uColor * noiseColor2.r * 2 + distanceToInnerRing * 0.4;

    finalColor.a = uOpacity;
    
    return finalColor;
}

technique Tech1
{
    pass RingShader
    {
        PixelShader = compile ps_2_0 RingShader();
    }
}