using MySql.Data.MySqlClient;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Server;
using RecomeceAPI.Server.Database;

namespace RecomeceAPI.Services.Common
{
  public class HandleCatchService
  {
    private readonly IFileService _fileService;

    public HandleCatchService(IFileService fileService)
    {
      _fileService = fileService;
    }

    public void HandleCatch(Exception ex)
    {
      string path = $"{Configs.RootFiles()}/Erros/";
      if (!_fileService.IsDirExists(path))
        _fileService.CreateDir(path);

      string msg = ex.Message + "\r\n" + ex.ToString();
      _fileService.CreateFile(path, $"Erro_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt", msg);
    }
    public void HandleCatch(Exception ex, string comp)
    {
      string path = $"{Configs.RootFiles()}/Erros/";
      if (!_fileService.IsDirExists(path))
        _fileService.CreateDir(path);

      string msg = $"{comp} \r\n \r\n {ex.Message} \r\n {ex}";
      _fileService.CreateFile(path, $"Erro_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt", msg);
    }
    public void HandleCatch( string comp)
    {
      string path = $"{Configs.RootFiles()}/Erros/";
      if (!_fileService.IsDirExists(path))
        _fileService.CreateDir(path);

      string msg = $"{comp} \r\n \r\n";
      _fileService.CreateFile(path, $"Erro_{DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss")}.txt", msg);
    }
  }
}
