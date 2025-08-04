using RecomeceAPI.Interfaces;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;
using RecomeceAPI.Services;
using RecomeceAPI.Services.PDFs;
using RecomeceAPI.Services.Serasa;

namespace RecomeceAPI.DI
{
  public static class TablesServicesInjection
  {
    public static IServiceCollection AddTableServices(this IServiceCollection services)
    {
      // Registro de serviços

      services.AddScoped<CadProdutosService>();
      services.AddScoped<CadProdutosFaixasService>();
      services.AddScoped<EstadosService>();
      services.AddScoped<CidadesService>();
      services.AddScoped<CadProdutosFaixasPagamentoService>();
      services.AddScoped<MovPropostasService>();
      services.AddScoped<MovPropostasDuplicatasService>();
      services.AddScoped<MovPropostasBeneficiariosService>();
      services.AddScoped<MovPropostasArquivosService>();
      services.AddScoped<MovCadClientesSerasaService>();
      services.AddScoped<MovAuditoriaService>();
      services.AddScoped<CadClientesService>();
      services.AddScoped<CadEquipeService>();
      services.AddScoped<CadEmpresasContasBancariasService>();
      services.AddScoped<CadEmpresasService>();
      services.AddScoped<CadComissaoVendedoresService>();
      services.AddScoped<CadComissaoFranqueadosService>();
      services.AddScoped<CadComissaoService>();
      services.AddScoped<MovConsultasCompradasService>();
      services.AddScoped<MovContratosService>();
      services.AddScoped<MovLotesEmpresasService>();
      services.AddScoped<ProposalService>();
      services.AddScoped<ContractService>();
      services.AddScoped<LogConsultasService>();
      services.AddScoped<CadConsultasService>();
      services.AddScoped<MovContaCorrenteService>();
      services.AddScoped<MovLotesEmpresasBeneficiariosService>();
      services.AddScoped<MovLotesAssociacaoService>();

      // Retorne os serviços configurados
      return services;
    }
  }
}
