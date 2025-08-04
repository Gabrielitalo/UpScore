using RecomeceAPI.Models;
using Newtonsoft.Json;
using System.Xml.Linq;
using Newtonsoft.Json.Serialization;

namespace RecomeceAPI.Services.Common
{
  public class HandleJsonService
  {
    public static string GetJson<T>(T obj)
    {
      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.None
      };

      return JsonConvert.SerializeObject(obj, settings);
    }
    public static string GetJson<T>(List<T> list)
    {
      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.None,
        NullValueHandling = NullValueHandling.Ignore
      };

      return JsonConvert.SerializeObject(list, settings);
    }
    public static List<T> GetListByJson<T>(string json)
    {
      return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
    }
    public static T SetByJson<T>(string json, T obj)
    {
      if (obj == null)
        throw new ArgumentNullException(nameof(obj));

      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        NullValueHandling = NullValueHandling.Ignore // <- ESSENCIAL!
      };

      JsonConvert.PopulateObject(json, obj, settings);
      return obj;
    }
    public static T? FromJson<T>(string json)
    {
      var settings = new JsonSerializerSettings
      {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.None
      };

      return JsonConvert.DeserializeObject<T>(json, settings);
    }
    public static string XmlToJson(string xml)
    {
      var doc = XDocument.Parse(xml);
      return JsonConvert.SerializeXNode(doc);
    }
  }
}
