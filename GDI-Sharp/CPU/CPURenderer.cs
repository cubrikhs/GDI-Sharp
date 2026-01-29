using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GDI_Sharp.CPU
{
    public class CPURenderer
    {
        private Bitmap buffer;
        private int width;
        private int height;
        private byte[] pixels;
        private BitmapData bmpData;
        private IntPtr ptr;

        public int Width => width;
        public int Height => height;

        public CPURenderer(int width, int height)
        {
            this.width = width;
            this.height = height;
            buffer = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            pixels = new byte[width * height * 4];
        }

        public Bitmap GetBitmap() => buffer;

        public void Clear(Color color)
        {
            for (int i = 0; i < pixels.Length; i += 4)
            {
                pixels[i + 0] = color.B;
                pixels[i + 1] = color.G;
                pixels[i + 2] = color.R;
                pixels[i + 3] = color.A;
            }
        }

        public void DrawPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            int index = (y * width + x) * 4;
            pixels[index + 0] = color.B;
            pixels[index + 1] = color.G;
            pixels[index + 2] = color.R;
            pixels[index + 3] = color.A;
        }

        public void DrawLine(int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                DrawPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 > -dy) { err -= dy; x0 += sx; }
                if (e2 < dx) { err += dx; y0 += sy; }
            }
        }

        public void Present()
        {
            Rectangle rect = new Rectangle(0, 0, width, height);
            bmpData = buffer.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmpData.Scan0, pixels.Length);
            buffer.UnlockBits(bmpData);
        }

        public void RenderEffect(ICpuEffect shader)
        {
            Form form = new TempForm(width, height);
            form.ClientSize = new Size(width, height);

            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            float time = 0f;

            timer.Interval = 16;
            timer.Tick += (s, e) =>
            {
                time += 0.016f;
                shader.Render(this, time);
                Present();
                form.Invalidate();
            };
            timer.Start();

            form.Paint += (s, e) =>
            {
                e.Graphics.DrawImage(buffer, 0, 0);
            };

            System.Windows.Forms.Application.Run(form);
        }
    }
}
