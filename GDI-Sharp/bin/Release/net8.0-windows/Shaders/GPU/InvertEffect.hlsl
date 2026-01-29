Texture2D desktopTexture : register(t0);
SamplerState samplerState : register(s0);

struct PS_INPUT
{
    float4 position : SV_POSITION;
    float2 tex : TEXCOORD0;
};

float4 main(PS_INPUT input) : SV_TARGET
{
    float4 color = desktopTexture.Sample(samplerState, input.tex);
    color.rgb = 1 - color.rgb; // invert
    return color;
}
