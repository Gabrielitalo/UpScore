using RecomeceAPI.Models;
using RecomeceAPI.Models.ASAAS;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Server.Database;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Services.ASAAS
{
  public class CustomerService
  {
    private readonly EndpointRequestService _endpointRequestService;
    private readonly string _baseUrl = "customers";

    public CustomerService(EndpointRequestService endpointRequestService)
    {
      _endpointRequestService = endpointRequestService;
    }

    public async Task<PaginationModel<CustomerModel>> GetCpfCnpj(string inscricao)
    {
      return await _endpointRequestService
          .GetAsync<PaginationModel<CustomerModel>>($"{_baseUrl}", $"?cpfCnpj={inscricao}") ?? new PaginationModel<CustomerModel>();
    }

    public async Task<CustomerModel> Create(string name, string inscricao)
    {
      inscricao = ExtensionService.CleanSpecialChars(inscricao);
      var isExists = await GetCpfCnpj(inscricao);
      if(isExists.Data.Count > 0)
      {
        return isExists.Data[0];
      }
      var obj = new CustomerModel();
      obj.CpfCnpj = inscricao;
      obj.Name = name;
      obj.NotificationDisabled = true;
      return await _endpointRequestService.PostAsync<CustomerModel>(_baseUrl, "", obj) ?? new CustomerModel(); 
    }
  }
}
