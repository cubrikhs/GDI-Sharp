using GDI_Sharp.CPU;
using System;
using System.Drawing;

namespace GDI_Sharp.Shaders.CPU
{
    public class ColorWave : ICpuEffect
    {
        public void Render(CPURenderer renderer, float time)
        {
            int width = renderer.Width;
            int height = renderer.Height;

            renderer.Clear(Color.Black);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte r = (byte)((Math.Sin((x + time * 50) * 0.05) * 0.5 + 0.5) * 255);
                    byte g = (byte)((Math.Sin((y + time * 50) * 0.05) * 0.5 + 0.5) * 255);
                    byte b = (byte)((Math.Sin((x + y + time * 50) * 0.05) * 0.5 + 0.5) * 255);
                    renderer.DrawPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
        }
    }
}
