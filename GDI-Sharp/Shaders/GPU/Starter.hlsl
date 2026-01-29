struct VS_OUTPUT
{
    float4 Pos : SV_POSITION;
    float2 Tex : TEXCOORD0;
};

// Fullscreen triangle with SV_VertexID
VS_OUTPUT VSMain(uint id : SV_VertexID)
{
    float2 pos = float2((id << 1) & 2, id & 2);
    VS_OUTPUT output;
    output.Pos = float4(pos * float2(2.0, -2.0) + float2(-1.0, 1.0), 0, 1);
    output.Tex = pos;
    return output;
}

Texture2D desktopTexture : register(t0);
SamplerState samplerState : register(s0);

float4 main(VS_OUTPUT input) : SV_TARGET
{
    float4 color = desktopTexture.Sample(samplerState, input.Tex);
    color.rgb = 1 - color.rgb; // Invert
    return color;
}
