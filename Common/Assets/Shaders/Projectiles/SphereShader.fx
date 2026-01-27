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
    
    // Rotating and weird wobbly effect
    float sinAngle = sin(distanceToCenter * 3.14 - uTime / 20);
    float cosAngle = cos(distanceToCenter * 3.14 - uTime / 15);
    float2 adjustedCoords = float2(cosAngle * centeredCoords.x - sinAngle * centeredCoords.y, sinAngle * centeredCoords.x + cosAngle * centeredCoords.y);
    adjustedCoords = (adjustedCoords + 1) / 2; 
    
    float4 noiseColor = tex2D(uImage0, adjustedCoords);
    noiseColor.r = 0;
    noiseColor.b = 0;
   
    float4 finalColor = 0;
    float animatedBorderWidth = borderWidth * ((sin(uTime / 12) + 3) * 0.25);
    float innerRingRadius = distance - animatedBorderWidth;
    float outerRingRadius = distance + animatedBorderWidth;
    if (distanceToCenter < distance)
    {
        finalColor += noiseColor;
    }
    if (distanceToCenter >= innerRingRadius && distanceToCenter <= outerRingRadius)
    {
        float midRadius = distance;
        finalColor += 1 - (abs(distanceToCenter - midRadius) / animatedBorderWidth);
    }
    return finalColor;

}

technique Tech1
{
    pass SphereShader
    {
        PixelShader = compile ps_2_0 SphereShader();
    }
}