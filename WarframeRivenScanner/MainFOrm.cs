using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    private WFMConnector wfm = new WFMConnector();

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
                {Combination.FromString("Control+F"), StartScreenshot},
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
      }
      catch (Exception ex)
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
      attr1ValueBox.Text = "";
      attr1EffectBox.Text = "";
      attr2ValueBox.Text = "";
      attr2EffectBox.Text = "";
      attr3ValueBox.Text = "";
      attr3EffectBox.Text = "";
      attr4ValueBox.Text = "";
      attr4EffectBox.Text = "";
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

    private async void loginBtn_Click(object sender, EventArgs e)
    {
      loginBtn.Enabled = false;
      uploadBtn.Enabled = false;
      if (await wfm.Login(emailBox.Text, passwordBox.Text))
      {
        uploadBtn.Enabled = true;
        tabControl1.SelectedTab = tabPage2;
      }
      else
      {
        MessageBox.Show("Failed to login");
      }
      loginBtn.Enabled = true;
    }

    private void uploadBtn_Click(object sender, EventArgs e)
    {
      var a = new AuctionCreateJson();
      a.starting_price = (int)priceBox.Value;
      a.buyout_price = (int)priceBox.Value;
      a.item.name = rivenNameBox.Text;
      a.item.mastery_level = (int)mrBox.Value;
      a.item.re_rolls = (int)rerollBox.Value;
      a.item.mod_rank = (int)modRankBox.Value;
      switch (polarityBox.SelectedIndex)
      {
        case 0:
          MessageBox.Show("You must select a polarity.");
          return;
        case 1:
          a.item.polarity = "madurai";
          break;
        case 2:
          a.item.polarity = "vazarin";
          break;
        case 3:
          a.item.polarity = "naramon";
          break;
      }
      a.item.weapon_url_name = gameDatabase.MatchClosestWeapon(weaponNameBox.Text).url_name;
      if (attr1ValueBox.Value != 0 && attr1EffectBox.Text.Length > 0)
      {
        var att = new RivenAttributeAuctionEntry();
        var att_entry = gameDatabase.MatchClosesRivenAttribute(attr1EffectBox.Text);
        att.url_name = att_entry.url_name;
        att.value = attr1ValueBox.Value;
        att.positive = att_entry.positive_is_negative ? attr1ValueBox.Value < 0 : attr1ValueBox.Value > 0;
        a.item.attributes.Add(att);
      }
      if (attr2ValueBox.Value != 0 && attr2EffectBox.Text.Length > 0)
      {
        var att = new RivenAttributeAuctionEntry();
        var att_entry = gameDatabase.MatchClosesRivenAttribute(attr2EffectBox.Text);
        att.url_name = att_entry.url_name;
        att.value = attr2ValueBox.Value;
        att.positive = att_entry.positive_is_negative ? attr2ValueBox.Value < 0 : attr2ValueBox.Value > 0;
        a.item.attributes.Add(att);
      }
      if (attr3ValueBox.Value != 0 && attr3EffectBox.Text.Length > 0)
      {
        var att = new RivenAttributeAuctionEntry();
        var att_entry = gameDatabase.MatchClosesRivenAttribute(attr3EffectBox.Text);
        att.url_name = att_entry.url_name;
        att.value = attr3ValueBox.Value;
        att.positive = att_entry.positive_is_negative ? attr3ValueBox.Value < 0 : attr3ValueBox.Value > 0;
        a.item.attributes.Add(att);
      }
      if (attr4ValueBox.Value != 0 && attr4EffectBox.Text.Length > 0)
      {
        var att = new RivenAttributeAuctionEntry();
        var att_entry = gameDatabase.MatchClosesRivenAttribute(attr4EffectBox.Text);
        att.url_name = att_entry.url_name;
        att.value = attr4ValueBox.Value;
        att.positive = att_entry.positive_is_negative ? attr4ValueBox.Value < 0 : attr4ValueBox.Value > 0;
        a.item.attributes.Add(att);
      }
      wfm.UploadRiven(a);
    }
  }
}
