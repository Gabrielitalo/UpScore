using MySqlX.XDevAPI;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server;
using System.Security.Claims;
using System.Security.Principal;

namespace RecomeceAPI.Middlewares
{
  public class AuthMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly List<string> _excludedRoutes;
    private ICryptService _cyptService;

    public AuthMiddleware(RequestDelegate next, List<string> excludedRoutes)
    {
      _next = next;
      _excludedRoutes = excludedRoutes;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        _cyptService = context.RequestServices.GetRequiredService<ICryptService>();

        // Verifica se a rota atual está na lista de rotas excluídas
        var path = context.Request.Path.Value ?? "";
        if (_excludedRoutes.Any(route => path.Contains(route)))
        {
          if (path == "/api/Login")
          {
            var modo = context.Request.Headers["Modo"].ToString();
            if (modo == "qa")
              SaveDev();
          }
          await _next(context); // Permite a requisição seguir sem verificar o token
          return;
        }

        // Tenta extrair o token do header de autorização
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
          var token = context.Request.Headers["Authorization"].ToString().Replace("Basic ", "");

          // Validação simples do token (você pode implementar uma lógica mais complexa)
          if (!string.IsNullOrWhiteSpace(token) && ValidateToken(token))
          {
            // Token é válido, segue a requisição
            await _next(context);
            return;
          }
        }

        // Se o token não for encontrado ou for inválido, retorna 401 Unauthorized
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Unauthorized: Token is missing or invalid.");
      }
      catch
      {
        // Se o token não for encontrado ou for inválido, retorna 401 Unauthorized
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Unauthorized: Token is missing or invalid.");
      }
    }

    // Método para validar o token (implementação pode variar)
    private bool ValidateToken(string token)
    {
      SessionModel sessionModel = new SessionModel();
      sessionModel.SetByJson(_cyptService.Decrypt(token));
      SaveIdentity(sessionModel);
      return true;
    }
    private static void SaveDev()
    {
      var claims = new[] {
                new Claim("DataBase", "2"),
                new Claim("Server", "1"),
            };
      var identity = new ClaimsIdentity(claims, "Autenticacao");
      var principal = new ClaimsPrincipal(identity);

      Thread.CurrentPrincipal = principal;
    }
    private static void SaveIdentity(SessionModel? s)
    {
      ClaimsIdentity identity;
      if (s != null)
      {
        var claims = new[] {
                new Claim("Username", s.Username),
                new Claim("IdUser", s.IdUser.ToString()),
                new Claim("IdCompany", s.IdCompany.ToString()),
                new Claim("DataBase", s.DataBase.ToString()),
                new Claim("Server", s.Server.ToString()),
                new Claim("IdType", s.IdType.ToString()),
                new Claim("IdRole", s.IdRole.ToString()),
                new Claim("IsAssociacao", s.IsAssociacao.ToString()),
            };
        identity = new ClaimsIdentity(claims, "Autenticacao");
      }
      else
      {
        var claims = new[] {
                new Claim("DataBase", Configs.BancoOficial),
                new Claim("Server", Configs.ServidorOficial),
            };
        identity = new ClaimsIdentity(claims, "Autenticacao");
      }

      var principal = new ClaimsPrincipal(identity);

      Thread.CurrentPrincipal = principal;
    }
  }

}
