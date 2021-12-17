using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WarframeRivenScanner
{
  public partial class MainForm : Form
  {
    private IKeyboardMouseEvents m_GlobalHook;
    ScreenshotForm screenForm;

    public MainForm()
    {
      InitializeComponent();
      m_GlobalHook = Hook.GlobalEvents();
      screenForm = new ScreenshotForm(this);
      screenForm.Show();
    }
    ~MainForm()
    {
      m_GlobalHook.Dispose();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      var map = new Dictionary<Combination, Action>
            {
                {Combination.FromString("Control+C"), StartScreenshot},
            };
      m_GlobalHook.OnCombination(map);
      screenForm.Hide();
    }

    private void StartScreenshot()
    {
      screenForm.ReloadScreenshot();
      screenForm.TopMost = true;
      screenForm.Show();
      screenForm.TopMost = false;
    }

    Color FilterPurple(Color src)
    {
      int red = 172;
      int green = 131;
      int blue = 213;

      int gray = (int)Math.Sqrt(Math.Pow(src.R - red, 2) + Math.Pow(src.G - green, 2) + Math.Pow(src.B - blue, 2));

      gray = (int) Math.Pow(gray, 1.1);

      gray = Math.Min(Math.Max(gray, 0), 255);
      return Color.FromArgb(gray, gray, gray);
    }

    public void FinishScreenshot()
    {
      Bitmap bmp = new Bitmap(screenForm.outputStatsBitmap.Width, screenForm.outputStatsBitmap.Height);
      for (int y = 0; y < bmp.Height; y++)
      {
        for (int x = 0; x < bmp.Width; x++)
        {
          bmp.SetPixel(x, y, FilterPurple(screenForm.outputStatsBitmap.GetPixel(x, y)));
        }
      }
      statsPicBox.Image = bmp;
      TopMost = true;
      TopMost = false;
    }
  }
}
