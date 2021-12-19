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
using Tesseract;

namespace WarframeRivenScanner
{
  public partial class MainForm : Form
  {
    private IKeyboardMouseEvents m_GlobalHook;
    ScreenshotForm screenForm;
    TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
    GameDatabase gameDatabase = new GameDatabase();

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

      gray = (int)Math.Pow(gray, 1.1);

      gray = Math.Min(Math.Max(gray, 0), 255);
      return Color.FromArgb(gray, gray, gray);
    }

    Bitmap ApplyPurpleTransform(Bitmap src)
    {
      Bitmap bmp = new Bitmap(src.Width, src.Height);
      for (int y = 0; y < bmp.Height; y++)
      {
        for (int x = 0; x < bmp.Width; x++)
        {
          bmp.SetPixel(x, y, FilterPurple(src.GetPixel(x, y)));
        }
      }
      return bmp;
    }

    public void FinishScreenshot()
    {
      var parser = new RivenParser(gameDatabase, engine);
      rivenPicBox.Image = screenForm.outputOriginalBitmap;
      try
      {
        Bitmap bmp = ApplyPurpleTransform(screenForm.outputStatsBitmap);
        statsPicBox.Image = bmp;
        parser.ParseNameAndAttributes(bmp);
      } catch (Exception ex)
      {
        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
      }
      try
      {
        Bitmap bmp = ApplyPurpleTransform(screenForm.outputMrBitmap);
        mrPicBox.Image = bmp;
        parser.ParseMasteryRank(bmp);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
      }
      try
      {
        Bitmap bmp = ApplyPurpleTransform(screenForm.outputRerollsBitmap);
        rerollsPicBox.Image = bmp;
        parser.ParseRerolls(bmp);
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
      }
      var riven_data = parser.GetResult();
      weaponNameBox.Text = riven_data.weapon_name;
      rivenNameBox.Text = riven_data.riven_name;
      if (riven_data.attributes.Count >= 1)
      {
        attr1ValueBox.Text = riven_data.attributes[0].value.ToString();
        attr1EffectBox.Text = riven_data.attributes[0].effect;
      }
      if (riven_data.attributes.Count >= 2)
      {
        attr2ValueBox.Text = riven_data.attributes[1].value.ToString();
        attr2EffectBox.Text = riven_data.attributes[1].effect;
      }
      if (riven_data.attributes.Count >= 3)
      {
        attr3ValueBox.Text = riven_data.attributes[2].value.ToString();
        attr3EffectBox.Text = riven_data.attributes[2].effect;
      }
      if (riven_data.attributes.Count >= 4)
      {
        attr4ValueBox.Text = riven_data.attributes[3].value.ToString();
        attr4EffectBox.Text = riven_data.attributes[3].effect;
      }
      mrBox.Text = riven_data.mastery_rank.ToString();
      rerollBox.Text = riven_data.rerolls.ToString();
      polarityBox.SelectedIndex = 0;
      TopMost = true;
      TopMost = false;
    }
  }
}
