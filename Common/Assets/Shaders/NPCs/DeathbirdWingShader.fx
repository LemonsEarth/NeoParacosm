sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float uTime;
float moveSpeed;
float4 color;
float tolerance;
float darkColorBoost;

float4 DeathbirdWingShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float2 framedCoords = float2(coords.x, coords.y * 0.2);
    float2 noiseCoords = uImageSize1 / uImageSize1.y;
    float4 textureColor = tex2D(uImage0, coords);
    float4 noiseColor = float4(0, 0, 0, 0);
    float2 centeredCoords = float2(framedCoords.x * 2 - 1, framedCoords.y * 2 - 0.2);
    float lightValue = 1 + length(centeredCoords);
    float darkValue = darkColorBoost + length(centeredCoords);
    if (framedCoords.x < 0.5)
    {
        noiseColor = tex2D(uImage1, float2(framedCoords.x + uTime * moveSpeed, framedCoords.y * 10 + uTime * moveSpeed));
    }
    else
    {
        noiseColor = tex2D(uImage1, float2(framedCoords.x + -uTime * moveSpeed, framedCoords.y * 10 + uTime * moveSpeed));
    }
    float4 finalColor;
    if (any(textureColor))
    {
        if (noiseColor.r < tolerance)
        {
            noiseColor *= darkValue;
            noiseColor.a = 1;
        }
        
        finalColor = noiseColor;
    }
    else
    {
        finalColor = textureColor;
    }
    return finalColor * lightValue;
}


technique Tech1
{
    pass DeathbirdWingShader
    {
        PixelShader = compile ps_2_0 DeathbirdWingShader();
    }
}