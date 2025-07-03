sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float uTime;
float moveSpeed;
float4 color;

float4 AscendedWeaponGlow(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
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