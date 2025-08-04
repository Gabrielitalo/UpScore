namespace RecomeceAPI.Interfaces
{
  public interface IFileService
  {
    bool IsDirExists(string path);
    void CreateDir(string path);
    void ZipDir(string path, string pathTo, string name);
    bool IsFileExists(string path);
    FileInfo GetFileInfo(string path);
    string GetFileContent(string path);
    byte[] GetFileBytes(string path);
    bool CreateFile(string path, string name, string content);
    void DeleteFile(string path);
  }
}
