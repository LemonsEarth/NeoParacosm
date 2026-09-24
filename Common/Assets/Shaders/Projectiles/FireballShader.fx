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
float uOpacity;
float4 color;
float2 velocity;
float distance;
float tolerance;
float noiseStepThreshold;

float4 FireballShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);

    float4 glowballColor = tex2D(uImage0, coords);
    float2 vel = any(velocity) == false ? float2(0, -1) : velocity;
    float4 noiseColor = tex2D(uImage1, coords + uTime / 4 * -velocity);
    float4 finalColor = step(noiseColor.r, noiseStepThreshold) + glowballColor;
    finalColor.a = glowballColor.r * 2;
    finalColor.rgb *= uColor.rgb;
    finalColor += (1 - distanceToCenter) * 0.1;
    return finalColor * uOpacity;

}

technique Tech1
{
    pass FireballShader
    {
        PixelShader = compile ps_2_0 FireballShader();
    }
}