sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
float2 uImageSize0;
float2 uImageSize1;
float4 uSourceRect;
float2 desiredPos;
float2 uTargetPosition;
float2 uScreenPosition;
float2 uScreenResolution;
float uTime;
float moveSpeed;
float4 uColor;
float4 color;
float2 velocity;
float distance;
float tolerance;

float4 GravityForceShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float2 centeredCoords = coords * 2.0 - 1.0;
    float distanceToCenter = length(centeredCoords);
     
    //float4 noiseColor = tex2D(uImage1, float2(coords.x + velocity.x * uTime, coords.y + velocity.y * uTime));
    float4 noiseColor = tex2D(uImage1, float2(coords.x, coords.y * 0.2 - uTime * 2));
    float4 finalColor = noiseColor * color;
    //finalColor.a = min(finalColor.r, finalColor.b) * 2;
    float verticalDistanceFactor = 1 - abs(centeredCoords.y);
    finalColor.a *= verticalDistanceFactor * 0.8;
    finalColor *= verticalDistanceFactor;

    return finalColor * color.a;

}

technique Tech1
{
    pass GravityForceShader
    {
        PixelShader = compile ps_2_0 GravityForceShader();
    }
}