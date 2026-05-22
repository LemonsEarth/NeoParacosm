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
float2 velocity;
float distance;
float tolerance;
float borderWidth;

float4 SphereShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);
   
    float4 noiseColor = tex2D(uImage0, float2(coords.x, coords.y + uTime));
   
    float4 finalColor = 0;
    float animatedBorderWidth = borderWidth * ((sin(uTime / 12) + 3) * 0.25);
    float innerRingRadius = distance - animatedBorderWidth;
    float outerRingRadius = distance + animatedBorderWidth;
    if (distanceToCenter < distance)
    {
        finalColor += noiseColor;
        finalColor += distanceToCenter;
    }
    if (distanceToCenter >= innerRingRadius && distanceToCenter <= outerRingRadius)
    {
        float midRadius = distance;
        finalColor += 1 - (abs(distanceToCenter - midRadius) / animatedBorderWidth);
    }
    finalColor.rgb *= float3(0.5, 1, 0.5);
    return finalColor;

}

technique Tech1
{
    pass SphereShader
    {
        PixelShader = compile ps_2_0 SphereShader();
    }
}