namespace RecomeceAPI.Models.ASAAS
{
  public class PaymentWebHookModel
  {
    public string Event {  get; set; } = string.Empty;
    public string dateCreated {  get; set; } = string.Empty;

    public PaymentModel Payment { get; set; } = new PaymentModel();
  }
}
