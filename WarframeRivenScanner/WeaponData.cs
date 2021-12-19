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
  public class WeaponData
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

  class RivenAttributesWrapper
  {
    public RivenAttributesPayload payload { get; set; }
  }
  class RivenAttributesPayload
  {
    public List<RivenAttributeInfo> attributes { get; set; }
  }
  public class RivenAttributeInfo
  {
    public string url_name { get; set; }
    public string group { get; set; }
    public bool positive_is_negative { get; set; }
    public bool search_only { get; set; }
    public string suffix { get; set; }
    public string id { get; set; }
    public string units { get; set; }
    public string effect { get; set; }
    public bool negative_only { get; set; }
    public List<String> exclusive_to { get; set; }
    public string prefix { get; set; }
  }

  public class GameDatabase
  {
    WeaponDataWrapper weapon_data;
    RivenAttributesWrapper riven_data;
    public GameDatabase()
    {
      weapon_data = System.Text.Json.JsonSerializer.Deserialize<WeaponDataWrapper>(File.ReadAllText(@".\game_data\all_items.json"));
      riven_data = System.Text.Json.JsonSerializer.Deserialize<RivenAttributesWrapper>(File.ReadAllText(@".\game_data\riven_attributes.json"));
    }

    public T FindMinBy<T>(List<T> list, Converter<T, int> projection) where T : new()
    {
      if (list.Count == 0)
      {
        throw new InvalidOperationException("Empty list");
      }
      int minValue = int.MaxValue;
      T result = new T();
      foreach (T item in list)
      {
        int value = projection(item);
        if (value < minValue)
        {
          minValue = value;
          result = item;
        }
      }
      return result;
    }
    public WeaponData MatchClosestWeapon(String test_name)
    {
      Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(test_name);
      var ClosestWeapon = FindMinBy(weapon_data.payload.items, x => lev.DistanceFrom(x.item_name));
      return ClosestWeapon;
    }
    public RivenAttributeInfo MatchClosesRivenAttribute(String test_name)
    {
      Fastenshtein.Levenshtein lev = new Fastenshtein.Levenshtein(test_name);
      var ClosestAttribute = FindMinBy(riven_data.payload.attributes, x => lev.DistanceFrom(x.effect));
      return ClosestAttribute;
    }
  }
}
