using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.D3DCompiler;
using Device = SharpDX.Direct3D11.Device;

namespace GDI_Sharp.GPU
{
    public class GpuWindow : Form
    {
        private Device device;
        private SwapChain swapChain;
        private RenderTargetView renderView;
        private VertexShader vertexShader;
        private SamplerState sampler;
        private SharpDX.Direct3D11.Texture2D desktopTexture;
        private ShaderResourceView desktopTextureView;

        private IGpuEffect effect;

        struct Vertex
        {
            public Vector3 Position;
            public Vector2 Tex;

        }

        internal static class Win32
        {
            public const int GWL_EXSTYLE = -20;
            public const int WS_EX_LAYERED = 0x80000;
            public const int WS_EX_TRANSPARENT = 0x20;

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

            [System.Runtime.InteropServices.DllImport("user32.dll")]
            public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
        }

        public GpuWindow()
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            BackColor = System.Drawing.Color.Black;
            TransparencyKey = System.Drawing.Color.Black;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);

            InitDirect3D();
            InitVertexShader();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // WS_EX_TRANSPARENT | WS_EX_LAYERED makes the window click-through
            int exStyle = (int)Win32.GetWindowLong(this.Handle, Win32.GWL_EXSTYLE);
            exStyle |= Win32.WS_EX_LAYERED | Win32.WS_EX_TRANSPARENT;
            Win32.SetWindowLong(this.Handle, Win32.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void InitDirect3D()
        {
            var swapDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(
                    Screen.PrimaryScreen.Bounds.Width,
                    Screen.PrimaryScreen.Bounds.Height,
                    new Rational(60, 1),
                    Format.R8G8B8A8_UNorm),
                Usage = Usage.RenderTargetOutput,
                OutputHandle = Handle,
                SampleDescription = new SampleDescription(1, 0),
                IsWindowed = true,
                SwapEffect = SwapEffect.Discard
            };

            Device.CreateWithSwapChain(
                SharpDX.Direct3D.DriverType.Hardware,
                DeviceCreationFlags.BgraSupport,
                swapDesc,
                out device,
                out swapChain);

            var backBuffer = SharpDX.Direct3D11.Resource.FromSwapChain<SharpDX.Direct3D11.Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);

            device.ImmediateContext.OutputMerger.SetRenderTargets(renderView);

            var viewport = new Viewport(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            device.ImmediateContext.Rasterizer.SetViewport(viewport);
        }

        private void InitVertexShader()
        {
            string shaderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Shaders", "GPU", "Starter.hlsl");
            if (!File.Exists(shaderPath))
                throw new FileNotFoundException("Starter shader not found", shaderPath);

            var vsByteCode = ShaderBytecode.CompileFromFile(shaderPath, "VSMain", "vs_5_0");
            vertexShader = new VertexShader(device, vsByteCode);

            sampler = new SamplerState(device, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            });
        }

        private void CaptureDesktop()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            var data = bmp.LockBits(
                new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            if (desktopTexture != null) desktopTexture.Dispose();
            desktopTexture = new SharpDX.Direct3D11.Texture2D(device, new Texture2DDescription()
            {
                Width = bmp.Width,
                Height = bmp.Height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.B8G8R8A8_UNorm,
                Usage = ResourceUsage.Default,
                SampleDescription = new SampleDescription(1, 0),
                BindFlags = BindFlags.ShaderResource
            }, new DataRectangle(data.Scan0, data.Stride));

            if (desktopTextureView != null) desktopTextureView.Dispose();
            desktopTextureView = new ShaderResourceView(device, desktopTexture);

            bmp.UnlockBits(data);
            bmp.Dispose();
        }

        public void RenderEffect(IGpuEffect gpuEffect)
        {
            effect = gpuEffect;
            effect.Initialize(device);

            Application.Idle += (s, e) =>
            {
                CaptureDesktop();

                device.ImmediateContext.ClearRenderTargetView(renderView, new Color4(0, 0, 0, 0));
                device.ImmediateContext.VertexShader.Set(vertexShader);
                effect.Render(device.ImmediateContext, desktopTextureView);
                device.ImmediateContext.PixelShader.SetSampler(0, sampler);
                device.ImmediateContext.Draw(3, 0); // draw fullscreen triangle
                swapChain.Present(1, PresentFlags.None);
            };

            Application.Run(this);
        }
    }
}
