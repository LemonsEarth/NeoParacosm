sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float uTime;
float4 uColor;
float flameHeightDownward;

float4 FireShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float centeredCoordsLength = length(centeredCoords);
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(coords.x, coords.y + uTime));
    noiseColor.rgb -= pow(coords.y, flameHeightDownward); // multiply more to make it smaller
    noiseColor.rgb = 1-step(noiseColor.rgb, 0.2); // main flame should be black
    noiseColor.rgb = uColor.rgb; // set the color
    //noiseColor *= noiseColor.a; // multiply because
    
    float4 noiseColorAgain = tex2D(uImage1, float2(coords.x, coords.y + uTime));

    float sizeMul = 16;
    float exponent = 2;
    float4 finalColor = (noiseColor - noiseColorAgain.r  * centeredCoordsLength * 2) * pow((1 - centeredCoordsLength), exponent) * sizeMul;
    finalColor.rgb = uColor;
    finalColor.rgb *= finalColor.a;
    return color + finalColor;
    
}

technique Tech1
{
    pass FireShader
    {
        PixelShader = compile ps_2_0 FireShader();
    }
}