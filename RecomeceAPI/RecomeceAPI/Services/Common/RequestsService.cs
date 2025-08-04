using RecomeceAPI.Interfaces;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RecomeceAPI.Services.Common
{
  public class RequestsService : IRequestsService
  {
    private readonly HttpClient _httpClient;

    public RequestsService(HttpClient httpClient)
    {
      _httpClient = httpClient;
    }

    public async Task<string> GetAsync(string url, string? token)
    {
      try
      {
        // Cria a requisição HTTP
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url))
        {
          // Adiciona os cabeçalhos, se o token estiver presente
          if (!string.IsNullOrEmpty(token))
          {
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", token);
          }

          // Envia a requisição e obtém a resposta
          HttpResponseMessage response = await _httpClient.SendAsync(request);
          // Lê o conteúdo da resposta
          return await response.Content.ReadAsStringAsync();
        }
      }
      catch (HttpRequestException ex)
      {
        // Lida com possíveis exceções de rede
        throw new Exception($"Erro ao fazer requisição GET: {ex.Message}", ex);
      }
    }

    public async Task<string> PostAsync(string url, string json, string? token)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("POST"), url))
      {
        try
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
          request.Headers.TryAddWithoutValidation("Accept", "application/json");
          if (!string.IsNullOrEmpty(token))
          {
            request.Headers.TryAddWithoutValidation("Authorization", token);
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
          }
          HttpResponseMessage response = await _httpClient.SendAsync(request);
          // Lê o conteúdo da resposta
          return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
          throw new Exception($"Erro ao fazer requisição GET: {ex.Message}", ex);
        }
      }
    }
    public async Task<string> PutAsync(string url, string json, string? token)
    {
      using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PUT"), url))
      {
        try
        {
          ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
          request.Headers.TryAddWithoutValidation("Accept", "application/json");
          if (!string.IsNullOrEmpty(token))
          {
            request.Headers.TryAddWithoutValidation("Authorization", token);
            request.Content = new StringContent(json);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
          }
          HttpResponseMessage response = await _httpClient.SendAsync(request);
          // Lê o conteúdo da resposta
          return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
          throw new Exception($"Erro ao fazer requisição GET: {ex.Message}", ex);
        }
      }
    }
    public async Task<string> DeleteAsync(string url, string? token)
    {
      try
      {
        // Cria a requisição HTTP
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url))
        {
          // Adiciona os cabeçalhos, se o token estiver presente
          if (!string.IsNullOrEmpty(token))
          {
            request.Headers.TryAddWithoutValidation("Accept", "application/json");
            request.Headers.TryAddWithoutValidation("Authorization", token);
          }

          // Envia a requisição e obtém a resposta
          HttpResponseMessage response = await _httpClient.SendAsync(request);
          // Lê o conteúdo da resposta
          return await response.Content.ReadAsStringAsync();
        }
      }
      catch (HttpRequestException ex)
      {
        // Lida com possíveis exceções de rede
        throw new Exception($"Erro ao fazer requisição GET: {ex.Message}", ex);
      }
    }
  }
}
