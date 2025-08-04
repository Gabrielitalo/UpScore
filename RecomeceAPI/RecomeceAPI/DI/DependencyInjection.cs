using DinkToPdf.Contracts;
using DinkToPdf;
using RecomeceAPI.Interfaces;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services;
using RecomeceAPI.Services.ASAAS;
using RecomeceAPI.Services.BoaVista;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services.Serasa;

namespace RecomeceAPI.DI
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      // Registro de serviços
      services.AddScoped<ConnectDB>();
      services.AddScoped<DbContext>();
      services.AddScoped<PaginationService>();
      services.AddScoped<GenericModelValidatorService>();
      services.AddScoped<HandleCatchService>();
      services.AddScoped<HandleExcelService>();
      services.AddScoped<LoginService>();
      services.AddScoped<ICryptService, CryptService>();
      services.AddTransient<IFileService, FileService>();

      services.AddHttpClient();
      services.AddSingleton<ISerasaTokenService, SerasaTokenService>();

      services.AddHttpClient<IRequestsService, RequestsService>();
      services.AddTransient<IRequestsService, RequestsService>();

      services.AddScoped<RestSerasaService>();
      services.AddScoped<BoaVistaConsultaService>();

      services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

      // Retorne os serviços configurados
      return services;
    }
  }
}
