using RecomeceAPI.Interfaces;
using System.IO;
using System.IO.Compression;
using System.Numerics;

namespace RecomeceAPI.Services.Common
{
  public class FileService : IFileService
  {
    public void CreateDir(string path)
    {
      DirectoryInfo dir = new DirectoryInfo(path);

      if (!IsDirExists(path))
        Directory.CreateDirectory(path);
    }

    public bool CreateFile(string path, string name, string content)
    {
      using (StreamWriter vWriter = new StreamWriter($"{path}{name}"))
      {
        vWriter.Write(content);
        vWriter.Flush();
        vWriter.Close();
        vWriter.Dispose();
      }
      return IsFileExists(path);
    }

    public void DeleteFile(string path)
    {
      FileInfo file = new FileInfo(path);
      file.Delete();
    }

    public byte[] GetFileBytes(string path)
    {
     return File.ReadAllBytes(path);
    }

    public string GetFileContent(string path)
    {
      string content = "";
      if (IsFileExists(path))
      {
        using (FileStream stream = File.Open(path, FileMode.Open))
        {
          StreamReader reader = new StreamReader(stream);
          content = reader.ReadToEnd();
          reader.Dispose();
        }
      }
      return content;
    }

    public FileInfo GetFileInfo(string path)
    {
      return new FileInfo(path);
    }

    public bool IsDirExists(string path)
    {
      DirectoryInfo dir = new DirectoryInfo(path);
      return dir.Exists;
    }

    public bool IsFileExists(string path)
    {
      return File.Exists(path);
    }

    public void ZipDir(string path, string pathTo, string name)
    {
      if (!IsDirExists(pathTo))
        return;
      if (IsFileExists($"{pathTo}{name})"))
      {
        File.Delete($"{pathTo}{name})");
      }
      ZipFile.CreateFromDirectory(path, $"{pathTo}{name})");
    }
  }
}
