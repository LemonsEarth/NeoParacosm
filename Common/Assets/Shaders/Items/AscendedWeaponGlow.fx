sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float uTime;
float moveSpeed;
float4 color;

bool CheckNeighbors(float2 currentCoords)
{
    float2 texelSize = float2(1.0 / uImageSize0.x, 1.0 / uImageSize0.y);
    float4 sampleLeft = tex2D(uImage0, currentCoords + float2(-texelSize.x, 0));
    float4 sampleRight = tex2D(uImage0, currentCoords + float2(texelSize.x, 0));
    float4 sampleUp = tex2D(uImage0, currentCoords + float2(0, texelSize.y));
    float4 sampleDown = tex2D(uImage0, currentCoords + float2(0, -texelSize.y));
    return any(sampleLeft) || any(sampleRight) || any(sampleUp) || any(sampleDown);
}

float4 AscendedWeaponGlow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 textureColor = tex2D(uImage0, coords);
    float4 noiseColor = tex2D(uImage1, coords + float2(uTime * moveSpeed, uTime * moveSpeed));
    if (any(textureColor))
    {
        noiseColor *= color;
        return noiseColor;
    }
    return textureColor;
}


technique Tech1
{
    pass AscendedWeaponGlow
    {
        PixelShader = compile ps_2_0 AscendedWeaponGlow();
    }
}