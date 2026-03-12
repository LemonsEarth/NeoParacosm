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

float hash(float xCoords) // gets a pseudo random number between -1 and 1
{
    return frac(sin((xCoords + 100) * 127) * 45213.61248) * 2.0 - 1.0;
}

float getHashedY(float xCoords)
{
    float segmentPosX = xCoords * 100; // turning coords into segments, where each segment is 1 unit wide
    float fracPosX = frac(segmentPosX); // x position of the current pixel within the segment (used as t for lerping, since at the start it's 0, and at the end it's 1
    float leftEdge = floor(segmentPosX); // left edge of the segment
    float leftEdgeY = hash(leftEdge); // Y value at that point
    float rightEdge = floor(segmentPosX + 1); // right edge of the segment/left edge of the next segment
    float rightEdgeY = hash(rightEdge); // Y value at that point
    return lerp(leftEdgeY, rightEdgeY, fracPosX);
}

float4 FireShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 scaleAdjustedCoords = float2(coords.x * 1, coords.y);
    float baseY = 0.5;
    float yOffset = getHashedY(scaleAdjustedCoords.x + uTime * 0.2) * 0.1;
    float factor = sin((uTime + yOffset + scaleAdjustedCoords.x) * 0.1);
    float yPos = baseY + yOffset + factor * 0.2;
    if (scaleAdjustedCoords.y > yPos)
    {
        return 1;
    }
    return 0;
}

technique Tech1
{
    pass FireShader
    {
        PixelShader = compile ps_2_0 FireShader();
    }
}