using MySql.Data.MySqlClient;
using RecomeceAPI.Server.Auth;

namespace RecomeceAPI.Server.Database
{
  public class ConnectDB
  {
    private readonly IConfiguration _config;

    public ConnectDB(IConfiguration config)
    {
      _config = config;
      Banco = Configs.BancoOficial;
      Servidor = Configs.ServidorOficial;
      connectionStringOficial = $"Server={Servidor};Database={Banco.ToLower()};Uid={_config["Database:User"]};Pwd={_config["Database:User"]};";
    }

    public string Banco { get; set; } = string.Empty;
    public string Servidor { get; set; } = string.Empty;  
    private MySqlConnection? DbCon { get; set; }
    private string? ConStr { get; set; }

    private string connectionStringOficial;

    public void connectionContext()
    {
      string dataBase = AppIdentity.GetClaimValue("DataBase");
      string server = AppIdentity.GetClaimValue("Server");
      if (string.IsNullOrEmpty(dataBase))
      {
        Banco = Configs.BancoOficial;
        Servidor = Configs.ServidorOficial;
      }
      else
      {
        Banco = Configs.GetDatabase(Convert.ToInt32(dataBase));
        Servidor = Configs.GetServer(Convert.ToInt32(server));
      }
      ConStr = connectionStringDinamica();
    }
    public string connectionStringDinamica()
    {
      return $"Server={Servidor};Database={Banco};Uid={_config["Database:User"]};Pwd={_config["Database:Password"]};";
    }
    public void ContextCon()
    {
      connectionContext();
      DbCon = new MySqlConnection(ConStr);
    }
    public MySqlConnection GetCon()
    {
      //ContextCon();
      //if (DbCon == null)
      //  return new MySqlConnection(connectionStringOficial);
      //else
      //  return DbCon;
      var dataBaseClaim = AppIdentity.GetClaimValue("DataBase");
      var serverClaim = AppIdentity.GetClaimValue("Server");

      string banco = string.IsNullOrEmpty(dataBaseClaim) ? Configs.BancoOficial : Configs.GetDatabase(Convert.ToInt32(dataBaseClaim));
      string servidor = string.IsNullOrEmpty(serverClaim) ? Configs.ServidorOficial : Configs.GetServer(Convert.ToInt32(serverClaim));

      var username = _config["Database:User"];
      var password = _config["Database:Password"];

      string connectionString = $"Server={servidor};Database={banco};Uid={username};Pwd={password};";

      return new MySqlConnection(connectionString);
    }
    public void CloseCon()
    {
      try
      {
        DbCon?.Close();
      }
      catch
      {

      }
    }
  }
}
