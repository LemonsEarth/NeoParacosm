sampler uImage0 : register(s0);
float uTime;
float uProgress;

float4 DesaturateShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0, float4 position : SV_Position) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float luminance = dot(color.rgb, float3(0.299, 0.587, 0.114));
    float3 gray = float3(luminance, luminance, luminance);
    float4 desaturatedColor = float4(0, 0, 0, 1);
    desaturatedColor.rgb = lerp(color.rgb, gray, uProgress);
    
    return desaturatedColor;
}

technique Tech1
{
    pass DesaturateShader
    {
        PixelShader = compile ps_2_0 DesaturateShader();
    }
}