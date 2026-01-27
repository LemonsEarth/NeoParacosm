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

float4 GasShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);
    
    // Making it stretch near the center
    float adjustedDistance = distanceToCenter * sqrt(distanceToCenter);
    float2 centerCoordsNormalized = normalize(centeredCoords);
    float2 adjustedCoords = centerCoordsNormalized * adjustedDistance;
    adjustedCoords = (adjustedCoords + 1) / 2;
    
    //float4 noiseColor = tex2D(uImage1, float2(coords.x + velocity.x * uTime, coords.y + velocity.y * uTime));
    float4 noiseColor = tex2D(uImage1, adjustedCoords + velocity * uTime);
    noiseColor *= color;
    float4 finalColor = noiseColor * (1 - adjustedDistance) * (sin(uTime) + 7) * 0.1;
    if (finalColor.r < tolerance && tolerance > 0)
    {
        finalColor = 0;
    }
    return finalColor;

}

technique Tech1
{
    pass GasShader
    {
        PixelShader = compile ps_2_0 GasShader();
    }
}