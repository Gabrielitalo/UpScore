using RecomeceAPI.Interfaces;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.ASAAS;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.DI
{
  public static class AssasInjection
  {
    public static IServiceCollection AddAsaasServices(this IServiceCollection services)
    {
      // Registro de serviços
      services.AddScoped<Endpoints>();
      services.AddScoped<EndpointRequestService>();
      services.AddScoped<CustomerService>();
      services.AddScoped<PaymentService>();


      // Retorne os serviços configurados
      return services;
    }
  }
}
