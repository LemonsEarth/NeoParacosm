sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 desiredPos;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float moveSpeed;
float4 centerColor;
float4 endColor;
float uTime;

float4 DreadlordLaserShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    
    float moveValue = moveSpeed * uTime;
    float2 movedCoords = float2(coords.x, coords.y + moveValue);
    float4 noiseColor = tex2D(uImage1, movedCoords);
    
    float2 centeredCoords = coords * 2.0 - 1.0;
    
    float distanceToCenterX = abs(centeredCoords.x);
 
    float4 finalColor = noiseColor * centerColor;
    finalColor *= lerp(centerColor, endColor, distanceToCenterX);
    finalColor *= (1 - distanceToCenterX);
    
    return finalColor * 3;
}

technique Tech1
{
    pass DreadlordLaserShader
    {
        PixelShader = compile ps_2_0 DreadlordLaserShader();
    }
}