using RecomeceAPI.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RecomeceAPI.Services.Serasa
{
  public class SerasaTokenService : ISerasaTokenService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SerasaTokenService> _logger;

    private string _accessToken = string.Empty;
    private DateTime _tokenExpiration = DateTime.MinValue;
    private readonly IConfiguration _config;

    public SerasaTokenService(IHttpClientFactory httpClientFactory, ILogger<SerasaTokenService> logger, IConfiguration config)
    {
      _httpClientFactory = httpClientFactory;
      _logger = logger;
      _config = config;
    }

    public async Task<string> GetTokenAsync()
    {
      if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _tokenExpiration)
      {
        return _accessToken;
      }

      var base_Url = _config["Serasa:Base_Url"];
      var client_Id = _config["Serasa:Client_Id"];
      var client_Secret = _config["Serasa:Client_Secret"];

      var client = _httpClientFactory.CreateClient();
      var request = new HttpRequestMessage(HttpMethod.Post, $"{base_Url}/security/iam/v1/client-identities/login");

      // Gera o base64(client_id:client_secret)
      var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{client_Id}:{client_Secret}"));
      request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);


      var response = await client.SendAsync(request);
      if (!response.IsSuccessStatusCode)
      {
        _logger.LogError("Erro ao obter token do Serasa: {StatusCode}", response.StatusCode);
        throw new Exception("Erro ao obter token");
      }

      var content = await response.Content.ReadAsStringAsync();
      var json = JsonDocument.Parse(content);
      _accessToken = json.RootElement.GetProperty("accessToken").GetString() ?? string.Empty;
      var expiresIn = json.RootElement.GetProperty("expiresIn").GetString();
      var unixExpiration = long.Parse(expiresIn);

      _tokenExpiration = DateTimeOffset.FromUnixTimeSeconds(unixExpiration - 60).UtcDateTime;

      return _accessToken;
    }
  }

}
