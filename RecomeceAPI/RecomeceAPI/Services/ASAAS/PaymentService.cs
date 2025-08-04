using RecomeceAPI.Models.ASAAS;
using RecomeceAPI.Models;

namespace RecomeceAPI.Services.ASAAS
{
  public class PaymentService
  {
    private readonly EndpointRequestService _endpointRequestService;
    private readonly string _baseUrl = "payments";

    public PaymentService(EndpointRequestService endpointRequestService)
    {
      _endpointRequestService = endpointRequestService;
    }

    public async Task<PaginationModel<PaymentsModel>> GetAllFromCustomer(string customerId)
    {
      return await _endpointRequestService.GetAsync<PaginationModel<PaymentsModel>>(_baseUrl, $"customer=${customerId}") ?? new PaginationModel<PaymentsModel>();
    }
    public async Task<PaymentModel> GetById(string id)
    {
      return await _endpointRequestService.GetAsync<PaymentModel>(_baseUrl, $"/{id}") ?? new PaymentModel();
    }
    public async Task<PaymentModel> Create(string customerId, string billingType, decimal value, DateTime dueDate, string description = "")
    {
      var obj = new
      {
        customer = customerId,
        billingType,
        value,
        description,
        dueDate = dueDate.ToString("yyyy-MM-dd")
      };
      return await _endpointRequestService.PostAsync<PaymentModel>(_baseUrl, "", obj) ?? new PaymentModel();
    }
  }
}
