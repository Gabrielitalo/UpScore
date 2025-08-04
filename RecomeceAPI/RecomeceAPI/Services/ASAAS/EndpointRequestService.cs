using RecomeceAPI.Services.Common;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecomeceAPI.Services.ASAAS
{
  public class Endpoints
  {
    private readonly string _baseUrl = "https://api.asaas.com/v3/";

    public Dictionary<string, string> EndpointsList()
    {
      return new Dictionary<string, string>
      {
        { "customers", "customers" },
        { "payments", "payments" },
      };
    }

    public string GetEndpoint(string name)
    {
      return EndpointsList().FirstOrDefault(d => d.Key == name).Value;
    }

    public string GetFullUrl(string name, string? queryString)
    {
      var endpoint = GetEndpoint(name);
      return string.IsNullOrWhiteSpace(endpoint) ? string.Empty : _baseUrl + endpoint + queryString;
    }
  }

  public class EndpointRequestService
  {
    private readonly HttpClient _httpClient;
    private readonly Endpoints _endpoints;
    private readonly string _apiKey;

    public EndpointRequestService(HttpClient httpClient, Endpoints endpoints, IConfiguration configuration)
    {
      _httpClient = httpClient;
      _endpoints = endpoints;
      _apiKey = configuration["Asaas:ApiKey"] ?? throw new ArgumentNullException("Asaas:ApiKey não configurada.");
    }


    public async Task<T?> GetAsync<T>(string name, string? queryString)
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      var url = _endpoints.GetFullUrl(name, queryString);
      if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentException("Endpoint inválido.");

      var request = new HttpRequestMessage(HttpMethod.Get, url);
      request.Headers.TryAddWithoutValidation("access_token", _apiKey);

      var response = await _httpClient.SendAsync(request);

      var json = await response.Content.ReadAsStringAsync();

      return HandleJsonService.FromJson<T>(json);
    }
    public async Task<T?> PostAsync<T>(string name, string? queryString, object payload)
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      var url = _endpoints.GetFullUrl(name, queryString);
      if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentException("Endpoint inválido.");

      var jsonContent = HandleJsonService.GetJson(payload);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var request = new HttpRequestMessage(HttpMethod.Post, url);
      request.Headers.TryAddWithoutValidation("access_token", _apiKey);
      request.Content = content;
      request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

      var response = await _httpClient.SendAsync(request);

      var responseJson = await response.Content.ReadAsStringAsync();
      return HandleJsonService.FromJson<T>(responseJson);
    }

    public async Task<T?> PutAsync<T>(string name, string? queryString, object payload)
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      var url = _endpoints.GetFullUrl(name, queryString);
      if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentException("Endpoint inválido.");

      var jsonContent = HandleJsonService.GetJson(payload);
      var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

      var request = new HttpRequestMessage(HttpMethod.Put, url);
      request.Headers.TryAddWithoutValidation("access_token", _apiKey);
      request.Content = content;
      request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

      var response = await _httpClient.SendAsync(request);

      var responseJson = await response.Content.ReadAsStringAsync();
      return HandleJsonService.FromJson<T>(responseJson);
    }

    public async Task<bool> DeleteAsync(string name, string? queryString)
    {
      ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
      var url = _endpoints.GetFullUrl(name, queryString);
      if (string.IsNullOrWhiteSpace(url))
        throw new ArgumentException("Endpoint inválido.");

      var request = new HttpRequestMessage(HttpMethod.Delete, url);
      request.Headers.TryAddWithoutValidation("access_token", _apiKey);

      var response = await _httpClient.SendAsync(request);
      return response.IsSuccessStatusCode;
    }
  }
}
