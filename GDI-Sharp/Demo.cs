using GDI_Sharp.CPU;
using GDI_Sharp.GPU;
using GDI_Sharp.Shaders.CPU;
class Demo
{
    [STAThread]
    static void Main()
    {
        var renderer = new CPURenderer(800, 600);
        renderer.RenderEffect(new ColorWave());

        //GpuWindow window = new GpuWindow();
        //window.RenderEffect(new InvertEffect());
    }
}