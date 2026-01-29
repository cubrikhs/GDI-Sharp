using SharpDX.Direct3D11;

namespace GDI_Sharp.GPU
{
    public interface IGpuEffect
    {
        string ShaderFile { get; }  // path to HLSL file
        string EntryPoint { get; }  // pixel shader entry point
        void Initialize(Device device);
        void Render(DeviceContext context, ShaderResourceView desktopTexture);
    }
}
