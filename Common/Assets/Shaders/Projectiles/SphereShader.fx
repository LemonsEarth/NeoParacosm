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
float tolerance;
float borderWidth;
float4 centerColor;
float4 endColor;

float4 SphereShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    
    float moveValue = moveSpeed * uTime;
    float2 movedCoords = float2(coords.x, coords.y + moveValue);
    float4 noiseColor = tex2D(uImage1, movedCoords);
    
    float2 centeredCoords = coords * 2.0 - 1.0;
    
    float distanceToCenter = length(centeredCoords);
    distanceToCenter = clamp(distanceToCenter, 0, 1);
 
    float4 finalColor = noiseColor * centerColor;
    finalColor *= lerp(centerColor, endColor, distanceToCenter * 1);
    finalColor *= (1 - distanceToCenter) * 4;
    
    return finalColor * 3;
}

technique Tech1
{
    pass SphereShader
    {
        PixelShader = compile ps_2_0 SphereShader();
    }
}