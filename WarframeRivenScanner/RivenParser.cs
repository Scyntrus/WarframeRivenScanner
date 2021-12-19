using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace WarframeRivenScanner
{
  class RivenAttribute
  {
    public String effect { get; set; }
    public Decimal value { get; set; }
  }
  class RivenData
  {
    public String weapon_name { get; set; }
    public String riven_name { get; set; }
    public List<RivenAttribute> attributes = new List<RivenAttribute>();
    public int mastery_rank { get; set; }
    public int rerolls { get; set; }
  }
  class RivenParser
  {
    TesseractEngine engine;
    GameDatabase db;
    RivenData result = new RivenData();
    public RivenParser(GameDatabase db, TesseractEngine engine)
    {
      this.engine = engine;
      this.db = db;
    }
    public RivenData GetResult()
    {
      return result;
    }
    public void ParseNameAndAttributes(Bitmap bmp)
    {
      engine.SetVariable("matcher_bad_match_pad", .1);
      engine.SetVariable("tessedit_char_whitelist", "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.%()+- ");

      using (var page = engine.Process(bmp))
      {
        Console.WriteLine(page.GetText());
        using (var iter = page.GetIterator())
        {
          iter.Begin();
          // Trim trash
          while (iter.GetConfidence(PageIteratorLevel.Word) < 0.5)
          {
            iter.Next(PageIteratorLevel.Word);
          }
          ParseName(iter);
          ParseAttributes(iter);
        }
      }
    }
    private void ParseName(Tesseract.ResultIterator iter)
    {
      var full_name = new StringBuilder();
      bool add_space = false;
      while (true)
      {
        var token = iter.GetText(PageIteratorLevel.Word).Trim();
        if (token.EndsWith("%") || token.Contains("+") || token.StartsWith("-"))
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
      weapon_name_start = full_name_str.LastIndexOf(" ", Math.Max(weapon_name_start - 1, 0));
      weapon_name_start = full_name_str.LastIndexOf(" ", Math.Max(weapon_name_start - 1, 0)) + 1;
      var weapon_name = full_name_str.Substring(weapon_name_start, full_name_str.LastIndexOf(" ") - weapon_name_start);
      weapon_name = db.MatchClosestWeapon(weapon_name).item_name;
      result.weapon_name = weapon_name;
      result.riven_name = full_name_str.Substring(full_name_str.LastIndexOf(" ") + 1);
    }

    decimal ExtractNumeric(String input)
    {
      StringBuilder buffer = new StringBuilder();
      bool seenDecimal = false;
      foreach (var x in input)
      {
        if ((x == '+' || x == '-') && buffer.Length == 0)
        {
          buffer.Append(x);
        }
        else if (Char.IsDigit(x))
        {
          buffer.Append(x);
        }
        else if (x == '.' && !seenDecimal)
        {
          seenDecimal = true;
          buffer.Append(x);
        }
        else if (x == '%')
        {
          break;
        }
      }
      if (buffer.Length == 0)
      {
        return 0;
      }
      return Convert.ToDecimal(buffer.ToString());
    }

    private void ParseAttributes(Tesseract.ResultIterator iter)
    {
      while (result.attributes.Count < 4 && !iter.IsAtFinalOf(PageIteratorLevel.Block, PageIteratorLevel.Word))
      {
        var attribute = new RivenAttribute();
        var value_token = iter.GetText(PageIteratorLevel.Word).Trim();
        attribute.value = ExtractNumeric(value_token);
        if (attribute.value == 0)
        {
          return;
        }
        var attr_name_builder = new StringBuilder();
        while (iter.Next(PageIteratorLevel.Word))
        {
          var token = iter.GetText(PageIteratorLevel.Word).Trim();
          if (token.EndsWith("%") || token.Contains("+") || token.StartsWith("-"))
          {
            break;
          }
          attr_name_builder.Append(token);
          attr_name_builder.Append(' ');
        }
        attribute.effect = db.MatchClosesRivenAttribute(attr_name_builder.ToString().Trim()).effect;
        result.attributes.Add(attribute);
      }
    }

    public void ParseMasteryRank(Bitmap bmp)
    {
      engine.SetVariable("matcher_bad_match_pad", .1);
      engine.SetVariable("tessedit_char_whitelist", "MR 0123456789");

      using (var page = engine.Process(bmp))
      {
        Console.WriteLine(page.GetText());
        result.mastery_rank = (int) ExtractNumeric(page.GetText());
      }
    }
    public void ParseRerolls(Bitmap bmp)
    {
      engine.SetVariable("matcher_bad_match_pad", .1);
      engine.SetVariable("tessedit_char_whitelist", "0123456789");

      using (var page = engine.Process(bmp))
      {
        Console.WriteLine(page.GetText());
        result.rerolls = (int)ExtractNumeric(page.GetText());
      }
    }
  }
}
