using System.Security.Claims;

namespace RecomeceAPI.Server.Auth
{
  public static class AppIdentity
  {
    public static ClaimsPrincipal GetPrincipal()
    {
      if (Thread.CurrentPrincipal == null)
        return new ClaimsPrincipal();

      return (ClaimsPrincipal)Thread.CurrentPrincipal;
    }
    public static string GetClaimValue(string tipo)
    {
      ClaimsPrincipal x = GetPrincipal();
      return x.FindFirstValue(tipo) ?? "";
    }
    public static long GetIdUserValue()
    {
      return Convert.ToInt64(GetClaimValue("IdUser") ?? "0");
    }
    public static long GetIdCompanyValue()
    {
      return Convert.ToInt64(GetClaimValue("IdCompany") ?? "0");
    }
    public static bool GetIsAssociacaoValue()
    {
      var v = GetClaimValue("IsAssociacao") ?? "false";
      return v == "True" ? true : false;
    }
  }
}
