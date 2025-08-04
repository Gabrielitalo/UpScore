namespace RecomeceAPI.Interfaces
{
  public interface IRequestsService
  {
    Task<string> GetAsync(string url, string? token);
    Task<string> PostAsync(string url, string json, string? token);
    Task<string> PutAsync(string url, string json, string? token);
    Task<string> DeleteAsync(string url, string? token);
  }
}
