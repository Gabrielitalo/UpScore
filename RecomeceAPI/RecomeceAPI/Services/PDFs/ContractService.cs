using QuestPDF.Fluent;
using RecomeceAPI.Models;
using System.Diagnostics.Contracts;

namespace RecomeceAPI.Services.PDFs
{
  public class ContractService
  {
    private readonly CadEmpresasService _cadEmpresasService;

    public ContractService(CadEmpresasService cadEmpresasService)
    {
      _cadEmpresasService = cadEmpresasService;
    }

    public async Task<Stream> GenerateContract(MovContratosModel movContrato, CadEmpresasModel cadEmpresas, List<MovPropostasDuplicatasModel> movPropostasDuplicatas, List<MovPropostasBeneficiariosModel> movPropostasBeneficiarios)
    {
      var stream = new MemoryStream();
      var company = await _cadEmpresasService.GetCompanyUserLogged();
      if (company.WhiteLabelConfig.ModeloProposta.ToLower() == "upscore")
      {
        var contrato = new ContratoPdf();
        contrato.CadEmpresas = cadEmpresas;
        contrato.MovContrato = movContrato;
        contrato.MovPropostasDuplicatas = movPropostasDuplicatas;
        contrato.MovPropostasBeneficiarios = movPropostasBeneficiarios;
        contrato.GeneratePdf(stream);
      }
      else if (company.WhiteLabelConfig.ModeloContrato.ToLower() == "recomece")
      {
        var contract = new ContratoRecomece();
        contract.CadEmpresas = cadEmpresas;
        contract.MovContrato = movContrato;
        contract.MovPropostasDuplicatas = movPropostasDuplicatas;
        contract.MovPropostasBeneficiarios = movPropostasBeneficiarios;
        contract.GeneratePdf(stream);
      }
      stream.Position = 0;
      return stream;
    }
  }
}
