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
      engine.SetVariable("matcher_bad_match_pad", .1);
      engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.%()+- ");
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
      try
      {
        using (var page = engine.Process(bmp))
        {
          textBox1.Text = page.GetText();
          var iter = page.GetIterator();
          ParseRiven(ref iter);
          iter.Dispose();
          page.Dispose();
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.Message);
      }
      TopMost = true;
      TopMost = false;
    }

    void ParseRiven(ref Tesseract.ResultIterator iter)
    {
      iter.Begin();
      while (iter.GetConfidence(PageIteratorLevel.Word) < 0.5)
      {
        iter.Next(PageIteratorLevel.Word);
      }
      var full_name = new StringBuilder();
      bool add_space = false;
      while (true)
      {
        var token = iter.GetText(PageIteratorLevel.Word).Trim();
        if (token.EndsWith("%") || token.Contains("+"))
        {
          break;
        }
        if (add_space)
        {
          full_name.Append(" ");
        }
        if (token.EndsWith("."))
        {
          token = token.Replace(".", "-");
        }
        full_name.Append(token);
        add_space = !(token.Length == 0 || token.EndsWith("-"));
        if (!iter.Next(PageIteratorLevel.Word))
        {
          break;
        }
      }
      var full_name_str = full_name.ToString();
      var weapon_name_start = full_name_str.LastIndexOf(" ");
      weapon_name_start = full_name_str.LastIndexOf(" ", weapon_name_start - 1);
      weapon_name_start = full_name_str.LastIndexOf(" ", weapon_name_start - 1) + 1;
      weaponNameBox.Text = full_name_str.Substring(weapon_name_start, full_name_str.LastIndexOf(" ") - weapon_name_start);
      rivenNameBox.Text = full_name_str.Substring(full_name_str.LastIndexOf(" ") + 1);
      return;
      do
      {
        do
        {
          do
          {
            do
            {
              if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
              {
                Console.WriteLine("<BLOCK>");
              }

              Console.Write("[" + iter.GetConfidence(PageIteratorLevel.Word) + "]");
              Console.Write(iter.GetText(PageIteratorLevel.Word));
              Console.Write(" ");

              if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
              {
                Console.WriteLine();
              }
            } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

            if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
            {
              Console.WriteLine();
            }
          } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
        } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
      } while (iter.Next(PageIteratorLevel.Block));

    }
  }
}
