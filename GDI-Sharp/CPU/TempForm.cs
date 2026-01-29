using System.Drawing;
using System.Windows.Forms;

class TempForm : Form
{
    public TempForm(int width, int height)
    {
        ClientSize = new Size(width, height);
        Text = "GDI-Sharp Demo";
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }
}
