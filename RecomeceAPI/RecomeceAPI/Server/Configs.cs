namespace RecomeceAPI.Server
{
  public static class Configs
  {
    public static string BancoOficial = "upscore";
    public static string BancoTeste = "limpaNome";
    public static string ServidorOficial = "69.62.96.249";
    //public static bool isDebug = true;

    public static string AppName = "UpScore";

    public static string RootFiles()
    {
      return $"/var/www/html/{AppName}";
    }
    public static Dictionary<int, string> Servers()
    {
      Dictionary<int, string> servers = new Dictionary<int, string>();
      servers.Add(1, "69.62.96.249");
      return servers;
    }
    public static Dictionary<int, string> Databases()
    {
      Dictionary<int, string> databases = new Dictionary<int, string>();
      databases.Add(1, "upscore");
      databases.Add(2, "limpaNome");
      return databases;
    }

    public static string GetServer(int server)
    {
      Dictionary<int, string> servers = Configs.Servers();
      return servers.FirstOrDefault(s => s.Key == server).Value;
    }
    public static string GetDatabase(int dataBase)
    {
      Dictionary<int, string> databases = Configs.Databases();
      return databases.FirstOrDefault(s => s.Key == dataBase).Value;
    }
  }
}
