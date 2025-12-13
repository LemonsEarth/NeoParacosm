sampler uImage0 : register(s0);
float2 uScreenResolution;
float noFogDistance;
float2 uTargetPosition;
float2 uScreenPosition;
float4 fogColor;
float time;
float maxFogOpacity;
float uProgress;
float uImageSize0;
float2 noiseSize;
float4 uSourceRect;
sampler useImage1 : register(s1);
float2 uImageSize1;
float2 playerVelocity;
float2 moveT;

float4 DCEffect(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 normalizedPixelCoords = (uScreenResolution / uScreenResolution.y);
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(useImage1, coords + float2(time / 100, 0) + (moveT / 50));
    float2 centeredCoords = coords * 2.0 - 1.0;
    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
    float3 gray = float3(luminance, luminance, luminance);
    float4 desaturatedColor = float4(0, 0, 0, 1);
    desaturatedColor.rgb = lerp(color.rgb, gray, uProgress * 0.4);
    desaturatedColor *= 1 - length(centeredCoords) * uProgress;
    noiseColor.a = noiseColor.r;
    
    float2 targetPositionDistanceOffset = uTargetPosition + float2(noFogDistance, 0);
    float2 uTargetPositionScreen = (uTargetPosition - uScreenPosition) / uScreenResolution;
    float2 targetPositionDistanceOffsetScreen = (targetPositionDistanceOffset - uScreenPosition) / uScreenResolution;
    float noFogDistanceScreen = distance(uTargetPositionScreen, targetPositionDistanceOffsetScreen);
    float coordsToPosDistance = length((coords - uTargetPositionScreen) * normalizedPixelCoords);
    float fogOpacity = clamp(coordsToPosDistance - noFogDistanceScreen, 0, maxFogOpacity);
    float4 finalNoiseColor = noiseColor * fogColor * fogOpacity * uProgress;
    finalNoiseColor.a = fogOpacity;
    return desaturatedColor + finalNoiseColor;
}

technique Tech1
{
    pass DCEffect
    {
        PixelShader = compile ps_2_0 DCEffect();
    }
}