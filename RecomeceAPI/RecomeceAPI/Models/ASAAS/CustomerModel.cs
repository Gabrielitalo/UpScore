namespace RecomeceAPI.Models.ASAAS
{
  public class CustomerModel
  {
    public string Object { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string AddressNumber { get; set; } = string.Empty;
    public string Complement { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string CpfCnpj { get; set; } = string.Empty;
    public string PersonType { get; set; } = string.Empty;
    public bool Deleted { get; set; }
    public string AdditionalEmails { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public bool NotificationDisabled { get; set; }
    public string Observations { get; set; } = string.Empty;
    public bool ForeignCustomer { get; set; }
  }

  public class CustomersModel
  {
    public List<CustomerModel> Customers { get; set; } = new List<CustomerModel>();
  }
}
