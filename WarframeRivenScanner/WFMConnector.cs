using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Json;

namespace WarframeRivenScanner
{
  class WFMConnector
  {
    private HttpClient client = new HttpClient();
    public WFMConnector()
    {
      client.DefaultRequestHeaders.Add("accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "JWT");
    }
    class SignInJson
    {
      public String auth_type { get; set; } = "header";
      public String email { get; set; }
      public String password { get; set; }
      public String device_id { get; set; } = "WarframeRivenScanner";
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
  }

  public class AuctionCreateJson
  {
    public String note { get; set; }
    public int starting_price { get; set; }
    public int buyout_price { get; set; }
    public int minimal_increment { get; set; }
    public int minimal_reputation { get; set; }
    public bool @private { get; set; } = true;
    public RivenAuction item { get; set; }
  }
  public class RivenAuction
  {
    public String type { get; set; } = "riven";
    public List<RivenAttributeAuctionEntry> attributes { get; set; }
    public String name { get; set; }
    public int mastery_level { get; set; }
    public int re_rolls { get; set; }
    public String weapon_url_name { get; set; }
    public String polarity { get; set; } // [ madurai, vazarin, naramon, zenurik ]
    public int mod_rank { get; set; }
  }
  public class RivenAttributeAuctionEntry
  {
    public String positive { get; set; }
    public String value { get; set; }
    public String url_name { get; set; }
  }
}
