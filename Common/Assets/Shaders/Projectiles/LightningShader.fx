sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float uTime;
float lightningLength;
int segmentCount;

float hash(float xCoords) // gets a pseudo random number between -1 and 1
{
    return frac(sin((xCoords + 100) * 127) * 45213.61248) * 2.0 - 1.0;
}

float getHashedY(float xCoords)
{
    float segmentPosX = xCoords * segmentCount;
    float fracPosX = frac(segmentPosX);
    float leftEdge = hash(floor(segmentPosX));
    float rightEdge = hash(floor(segmentPosX + 1));
    return lerp(leftEdge, rightEdge, fracPosX);
}

float4 LightningShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float2 scaleAdjustedCoords = float2(coords.x * lightningLength, coords.y);
    float baseY = 0.5;
    float testYOffset = sin(scaleAdjustedCoords.x * 6.28 * 4) * 0.1;
    float yOffset = getHashedY(scaleAdjustedCoords.x) * 0.1;
    float yPos = baseY + yOffset;
    
    if (abs(scaleAdjustedCoords.y - yPos) < 0.02)
    {
        return 1;
    }
    return color;
}

technique Tech1
{
    pass LightningShader
    {
        PixelShader = compile ps_2_0 LightningShader();
    }
}