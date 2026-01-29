using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using System;
using System.IO;

namespace GDI_Sharp.GPU
{
    public class InvertEffect : IGpuEffect
    {
        public string ShaderFile => "InvertEffect.hlsl"; // just file name
        public string EntryPoint => "main";

        private PixelShader pixelShader;

        public void Initialize(Device device)
        {
            // Build absolute path at runtime
            string shaderPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Shaders", "GPU", ShaderFile
            );

            if (!File.Exists(shaderPath))
                throw new FileNotFoundException("Shader file not found", shaderPath);

            var psByteCode = ShaderBytecode.CompileFromFile(shaderPath, EntryPoint, "ps_5_0");
            pixelShader = new PixelShader(device, psByteCode);
        }

        public void Render(DeviceContext context, ShaderResourceView desktopTexture)
        {
            context.PixelShader.Set(pixelShader);
            context.PixelShader.SetShaderResource(0, desktopTexture);
        }
    }
}
