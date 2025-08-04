namespace RecomeceAPI.Services.ASAAS
{
    public class AsaasHelper
    {
        public string GetPaymentMethod(int code)
        {
            Dictionary<int, string> pairs = new Dictionary<int, string>();
            pairs.Add(1, "BOLETO");
            pairs.Add(2, "CREDIT_CARD");
            pairs.Add(3, "PIX");

            return pairs.FirstOrDefault(f => f.Key == code).Value;
        }
    }
}
