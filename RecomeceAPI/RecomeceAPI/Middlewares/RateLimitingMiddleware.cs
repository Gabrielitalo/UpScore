namespace RecomeceAPI.Middlewares
{
  public class RateLimitingMiddleware
  {
    private readonly RequestDelegate _next;
    private static Dictionary<string, RateLimitInfo> _requests = new Dictionary<string, RateLimitInfo>();
    private readonly int _maxRequests;
    private readonly TimeSpan _timeSpan;

    public RateLimitingMiddleware(RequestDelegate next, int maxRequests, TimeSpan timeSpan)
    {
      _next = next;
      _maxRequests = maxRequests;
      _timeSpan = timeSpan;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

      if (_requests.ContainsKey(ipAddress))
      {
        var rateLimitInfo = _requests[ipAddress];

        // Verifica se o tempo da janela expirou
        if (DateTime.UtcNow - rateLimitInfo.StartTime > _timeSpan)
        {
          // Reinicia a contagem de requisições após o tempo da janela
          rateLimitInfo.StartTime = DateTime.UtcNow;
          rateLimitInfo.RequestCount = 1;
        }
        else
        {
          // Incrementa o número de requisições dentro da janela de tempo
          rateLimitInfo.RequestCount++;
        }

        // Se o número de requisições exceder o limite, retorna 429
        if (rateLimitInfo.RequestCount > _maxRequests)
        {
          context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
          context.Response.Headers["Retry-After"] = _timeSpan.TotalSeconds.ToString(); // Informa quanto tempo esperar
          await context.Response.WriteAsync("Too many requests. Please try again later.");
          return;
        }
      }
      else
      {
        // Adiciona o IP à lista com a primeira requisição
        _requests[ipAddress] = new RateLimitInfo
        {
          StartTime = DateTime.UtcNow,
          RequestCount = 1
        };
      }

      // Se o limite não for excedido, segue o fluxo da requisição
      await _next(context);
    }
  }

  public class RateLimitInfo
  {
    public DateTime StartTime { get; set; }
    public int RequestCount { get; set; }
  }
}
