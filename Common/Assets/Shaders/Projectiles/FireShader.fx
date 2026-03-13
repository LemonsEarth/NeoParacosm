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

float hash(float xCoords) // gets a pseudo random number between -1 and 1
{
    return frac(sin((xCoords + 100) * 127) * 45213.61248) * 2.0 - 1.0;
}

float getHashedY(float xCoords)
{
    float segmentPosX = xCoords * 20; // turning coords into segments, where each segment is 1 unit wide
    float fracPosX = frac(segmentPosX); // x position of the current pixel within the segment (used as t for lerping, since at the start it's 0, and at the end it's 1
    float leftEdge = floor(segmentPosX); // left edge of the segment
    float leftEdgeY = hash(leftEdge); // Y value at that point
    float rightEdge = floor(segmentPosX + 1); // right edge of the segment/left edge of the next segment
    float rightEdgeY = hash(rightEdge); // Y value at that point
    return lerp(leftEdgeY, rightEdgeY, fracPosX);
}

float4 FireShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float centeredCoordsLength = length(centeredCoords);
    float4 color = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, float2(coords.x + uTime, coords.y + uTime));
    noiseColor.rgb -= pow(coords.y, flameHeightDownward); // multiply more to make it smaller
    noiseColor.rgb = 1-step(noiseColor.rgb, 0.2); // main flame should be black
    noiseColor.a = 1-noiseColor.r; // white -> transparent
    noiseColor.rgb = uColor.rgb; // set the color
    noiseColor *= noiseColor.a; // multiply because
    
    float4 noiseColorAgain = tex2D(uImage1, float2(coords.x + uTime, coords.y + uTime));

    float sizeMul = 16;
    float exponent = 2;
    float4 finalColor = (noiseColor - noiseColorAgain.r  * centeredCoordsLength * 2) * pow((1 - centeredCoordsLength), exponent) * sizeMul;
    finalColor.rgb = uColor;
    finalColor.rgb *= finalColor.a;
    return finalColor;
    
}

technique Tech1
{
    pass FireShader
    {
        PixelShader = compile ps_2_0 FireShader();
    }
}