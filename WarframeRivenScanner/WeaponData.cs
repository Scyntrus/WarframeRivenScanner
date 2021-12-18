using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarframeRivenScanner
{
  class WeaponDataWrapper
  {
    public WeaponDataPayload payload { get; set; }
  }
  class WeaponDataPayload
  {
    public List<WeaponData> items { get; set; }
  }
  class WeaponData
  {
    public string group { get; set; }
    public string item_name { get; set; }
    public string riven_type { get; set; }
    public string id { get; set; }
    public string url_name { get; set; }
    public string icon { get; set; }
    public string thumb { get; set; }
    public string icon_format { get; set; }
  }

  public class GameDatabase
  {
    WeaponDataWrapper weapon_data;
    public GameDatabase()
    {
      weapon_data = System.Text.Json.JsonSerializer.Deserialize<WeaponDataWrapper>(File.ReadAllText(@".\game_data\all_items.json"));
    }
  }
}
