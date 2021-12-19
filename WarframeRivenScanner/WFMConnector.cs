using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace WarframeRivenScanner
{
  class SignInJson
  {
    public String auth_type { get; set; } = "header";
    public String email { get; set; }
    public String password { get; set; }
    public String device_id { get; set; } = "WarframeRivenScanner";
  }

  public class AuctionCreateJson
  {
    public String note { get; set; } = "Uploaded with Warframe Riven Scanner";
    public int starting_price { get; set; }
    public int buyout_price { get; set; }
    //public int minimal_increment { get; set; }
    //public int minimal_reputation { get; set; }
    public bool @private { get; set; } = false; // Set to true when debugging
    public RivenAuction item { get; set; } = new RivenAuction();
  }
  public class RivenAuction
  {
    public String type { get; set; } = "riven";
    public List<RivenAttributeAuctionEntry> attributes { get; set; } = new List<RivenAttributeAuctionEntry>();
    public String name { get; set; }
    public int mastery_level { get; set; }
    public int re_rolls { get; set; }
    public String weapon_url_name { get; set; }
    public String polarity { get; set; } // [ madurai, vazarin, naramon, zenurik ]
    public int mod_rank { get; set; }
  }
  public class RivenAttributeAuctionEntry
  {
    public bool positive { get; set; }
    public Decimal value { get; set; }
    public String url_name { get; set; }
  }
  class WFMConnector
  {
    private HttpClient client = new HttpClient();
    public WFMConnector()
    {
      client.DefaultRequestHeaders.Add("accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "JWT");
    }
    public async Task<bool> Login(String email, String password)
    {
      var args = new SignInJson();
      args.email = email;
      args.password = password;
      var response = await client.PostAsync("https://api.warframe.market/v1/auth/signin", JsonContent.Create(args));
      if (response.StatusCode == System.Net.HttpStatusCode.OK)
      {
        client.DefaultRequestHeaders.Remove("Authorization");
        var auth = response.Headers.GetValues("Authorization").First();
        client.DefaultRequestHeaders.Add("Authorization", auth);
        return true;
      } else
      {
        client.DefaultRequestHeaders.Remove("Authorization");
        client.DefaultRequestHeaders.Add("Authorization", "JWT");
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        return false;
      }
    }

    public async void UploadRiven(AuctionCreateJson args)
    {
      if (MessageBox.Show(JsonSerializer.Serialize(args), "Preview Upload, Continue?", MessageBoxButtons.OKCancel) == DialogResult.OK)
      {
        var response = await client.PostAsync("https://api.warframe.market/v1/auctions/create", JsonContent.Create(args));
        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
          MessageBox.Show("Success!");
        }
        else
        {
          MessageBox.Show(await response.Content.ReadAsStringAsync(), "Failed to create Riven listing");
        }
      }
    }
  }
}
