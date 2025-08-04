using Newtonsoft.Json;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Server;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Common
{
  public class SessionModel
  {
    public int Active { get; set; }
    public int IdType { get; set; }
    public int IdRole { get; set; }
    public long IdUser { get; set; }
    public long IdCompany { get; set; }
    private DateTime DateSession { get; set; }
    public DateTime ExpiresSession { get; set; }
    public int DataBase { get; set; }
    public int Server { get; set; }
    public bool IsAssociacao { get; set; } = false;
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    public SessionModel()
    {
    }


    public string GetJson()
    {
      return JsonConvert.SerializeObject(this);
    }
    public void SetByJson(string json)
    {
      JsonConvert.PopulateObject(json, this);
    }
    public void SetDateSession(DateTime dateSession)
    {
      DateSession = dateSession;
    }
    public void SetExpiresSession()
    {
      ExpiresSession = DateSession.AddHours(4);
    }
  }
}
