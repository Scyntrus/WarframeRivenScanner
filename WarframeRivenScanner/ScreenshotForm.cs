using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarframeRivenScanner
{
  public partial class ScreenshotForm : Form
  {
    Rectangle workingArea;
    Bitmap captureBitmap;
    Graphics captureGraphics;
    Graphics bufferGraphics;
    Pen outlinePen;
    MainForm parent;

    public Bitmap outputStatsBitmap;
    public ScreenshotForm(MainForm p)
    {
      parent = p;
      InitializeComponent();
      outlinePen = new Pen(Color.Red, 1);
      workingArea = Screen.AllScreens.Select(screen => screen.Bounds).Aggregate(Rectangle.Union);
      captureBitmap = new Bitmap(workingArea.Width, workingArea.Height, PixelFormat.Format32bppArgb);
      captureGraphics = Graphics.FromImage(captureBitmap);
      Bounds = workingArea;
      bufferGraphics = pictureBox1.CreateGraphics();
    }

    public void ReloadScreenshot()
    {
      captureGraphics.CopyFromScreen(workingArea.Location, new Point(0, 0), workingArea.Size);
      DrawRectangles(Cursor.Position);
    }

    private Rectangle Offset(Rectangle src, Point offset)
    {
      src.Offset(offset);
      return src;
    }
    Rectangle fullRect = new Rectangle(-120, -20, 240, 350);
    Rectangle statsRect = new Rectangle(-114, 62, 228, 210);
    private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
    {
      DrawRectangles(e.Location);
    }

    private void DrawRectangles(Point position)
    {
      bufferGraphics.DrawImage(captureBitmap, new Point(0, 0));
      bufferGraphics.DrawRectangle(outlinePen, Offset(fullRect, position));
      bufferGraphics.DrawRectangle(outlinePen, Offset(statsRect, position));
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
      var position = Cursor.Position;
      {
        outputStatsBitmap = new Bitmap(statsRect.Width, statsRect.Height, PixelFormat.Format32bppArgb);
        var g = Graphics.FromImage(outputStatsBitmap);
        g.Clear(Color.Red);
        g.DrawImage(captureBitmap, 0, 0, Offset(statsRect, position), GraphicsUnit.Pixel);
        g.Save();
      }
      Hide();
      parent.FinishScreenshot();
    }
  }
}
