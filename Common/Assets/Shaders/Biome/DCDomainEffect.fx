sampler uImage0 : register(s0);
float2 uScreenResolution;
float2 uTargetPosition;
float2 uScreenPosition;
float uOpacity;
float2 worldSize;
float2 screenPos;
float4 fogColor;
float time;
float uProgress;
float uImageSize0;
float2 noiseSize;
float4 uSourceRect;
sampler useImage1 : register(s1);
float2 uImageSize1;
float range;

float4 DCDomainEffect(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 normalizedPixelCoords = (uScreenResolution / uScreenResolution.y);
    // Where there shouldn't be fog
    float2 targetPositionDistanceOffset = uTargetPosition + float2(range * 2, 0);
    float2 uTargetPositionScreen = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 targetPositionDistanceOffsetScreen = (targetPositionDistanceOffset - uScreenPosition) / uScreenResolution;
    float colorDistanceScreen = distance(uTargetPositionScreen, targetPositionDistanceOffsetScreen);
    float coordsToPosDistance = length((coords - uTargetPositionScreen) * normalizedPixelCoords);
    
    float borderThickness = 0.005f;
    
    if (coordsToPosDistance < colorDistanceScreen)
    {
        return color;
    }
    else if (coordsToPosDistance >= colorDistanceScreen && coordsToPosDistance <= colorDistanceScreen + borderThickness)
    {
        return color + uOpacity;
    }
    else if (coordsToPosDistance >= colorDistanceScreen * 4) 
    {
        return color;
    }
    return color * (1 - uOpacity);
}

technique Tech1
{
    pass DCDomainEffect
    {
        PixelShader = compile ps_2_0 DCDomainEffect();
    }
}