public class PaymentModel
{
  public string Object { get; set; } = string.Empty;
  public string Id { get; set; } = string.Empty;
  public DateTime DateCreated { get; set; }
  public string CustomerId { get; set; } = string.Empty;
  public string? Subscription { get; set; }
  public string? Installment { get; set; }
  public string? CheckoutSession { get; set; }
  public string? PaymentLink { get; set; }
  public decimal Value { get; set; }
  public decimal NetValue { get; set; }
  public decimal? OriginalValue { get; set; }
  public decimal? InterestValue { get; set; }
  public string? Description { get; set; }
  public string BillingType { get; set; } = string.Empty;
  public CreditCardInfo? CreditCard { get; set; }
  public bool CanBePaidAfterDueDate { get; set; }
  public string? PixTransaction { get; set; }
  public string? PixQrCodeId { get; set; }
  public string Status { get; set; } = string.Empty;
  public DateTime DueDate { get; set; }
  public DateTime OriginalDueDate { get; set; }
  public DateTime? PaymentDate { get; set; }
  public DateTime? ClientPaymentDate { get; set; }
  public int? InstallmentNumber { get; set; }
  public string? InvoiceUrl { get; set; }
  public string? InvoiceNumber { get; set; }
  public string? ExternalReference { get; set; }
  public bool Deleted { get; set; }
  public bool Anticipated { get; set; }
  public bool Anticipable { get; set; }
  public DateTime? CreditDate { get; set; }
  public DateTime? EstimatedCreditDate { get; set; }
  public string? TransactionReceiptUrl { get; set; }
  public string? NossoNumero { get; set; }
  public string? BankSlipUrl { get; set; }
  public DiscountInfo? Discount { get; set; }
  public FineInfo? Fine { get; set; }
  public InterestInfo? Interest { get; set; }
  public List<SplitInfo> Split { get; set; } = new();
  public bool PostalService { get; set; }
  public int? DaysAfterDueDateToRegistrationCancellation { get; set; }
  public ChargebackInfo? Chargeback { get; set; }
  public EscrowInfo? Escrow { get; set; }
  public List<RefundInfo>? Refunds { get; set; }
}


public class PaymentsModel()
{
  public List<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
}

public class CreditCardInfo
{
  public string CreditCardNumber { get; set; } = string.Empty;
  public string CreditCardBrand { get; set; } = string.Empty;
  public string? CreditCardToken { get; set; }
}

public class DiscountInfo
{
  public decimal Value { get; set; }
  public int DueDateLimitDays { get; set; }
  public string Type { get; set; } = string.Empty;
}

public class FineInfo
{
  public decimal Value { get; set; }
}

public class InterestInfo
{
  public decimal Value { get; set; }
}

public class SplitInfo
{
  public string Id { get; set; } = string.Empty;
  public string WalletId { get; set; } = string.Empty;
  public decimal FixedValue { get; set; }
  public decimal? PercentualValue { get; set; }
  public decimal TotalValue { get; set; }
  public string CancellationReason { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public string? ExternalReference { get; set; }
  public string? Description { get; set; }
}

public class ChargebackInfo
{
  public string Id { get; set; } = string.Empty;
  public string Payment { get; set; } = string.Empty;
  public string Installment { get; set; } = string.Empty;
  public string CustomerAccount { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public string Reason { get; set; } = string.Empty;
  public DateTime DisputeStartDate { get; set; }
  public decimal Value { get; set; }
  public DateTime PaymentDate { get; set; }
  public CreditCardChargebackInfo CreditCard { get; set; } = new();
  public string DisputeStatus { get; set; } = string.Empty;
  public DateTime DeadlineToSendDisputeDocuments { get; set; }
}

public class CreditCardChargebackInfo
{
  public string Number { get; set; } = string.Empty;
  public string Brand { get; set; } = string.Empty;
}

public class EscrowInfo
{
  public string Id { get; set; } = string.Empty;
  public string Status { get; set; } = string.Empty;
  public DateTime ExpirationDate { get; set; }
  public DateTime FinishDate { get; set; }
  public string FinishReason { get; set; } = string.Empty;
}

public class RefundInfo
{
  public DateTime DateCreated { get; set; }
  public string Status { get; set; } = string.Empty;
  public decimal Value { get; set; }
  public string? EndToEndIdentifier { get; set; }
  public string? Description { get; set; }
  public DateTime EffectiveDate { get; set; }
  public string? TransactionReceiptUrl { get; set; }
  public List<RefundedSplitInfo> RefundedSplits { get; set; } = new();
}

public class RefundedSplitInfo
{
  public string Id { get; set; } = string.Empty;
  public decimal Value { get; set; }
  public bool Done { get; set; }
}
