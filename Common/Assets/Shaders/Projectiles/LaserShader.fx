sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 desiredPos;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float time;
float moveSpeed;

float4 LaserShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 baseColor = tex2D(uImage0, coords);
    float moveValue = moveSpeed * time;
    float2 movedCoords = float2(coords.x, coords.y + moveValue);
    movedCoords = frac(movedCoords);
    float4 noiseColor = tex2D(uImage1, movedCoords);
    float2 centeredCoords = coords * 2.0 - 1.0;
    
    float width = sin(time * 4) * 0.1 + 0.1;
    float distanceToCenterX = distance(abs(centeredCoords.x), width);
    float distanceToCenterY = distance(abs(centeredCoords.y), float2(0, 0));
    
    float2 xMul = (1, 1);
    xMul = float2(1, 1) - float2(distanceToCenterX, distanceToCenterY); // curviness
    float4 finalColor = noiseColor * (1 - distanceToCenterX) * length(xMul);
    
    if (finalColor.r < 0.1)
    { 
        finalColor = 0;
    }
    else if (finalColor.r < 0.3)
    {
        finalColor.rgb = 0;
    }
    return finalColor * 3;
}

technique Tech1
{
    pass LaserShader
    {
        PixelShader = compile ps_2_0 LaserShader();
    }
}