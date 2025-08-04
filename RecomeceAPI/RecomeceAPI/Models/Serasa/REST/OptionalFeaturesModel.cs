using Newtonsoft.Json;

namespace RecomeceAPI.Models.Serasa.REST
{
  public class OptionalFeaturesModel
  {
    [JsonProperty("negativeData")]
    public OptionalNegativeData NegativeData { get; set; } = new();

    [JsonProperty("facts")]
    public OptionalFacts Facts { get; set; } = new();

    [JsonProperty("attributes")]
    public OptionalAttributes Attributes { get; set; } = new();
  }

  public class OptionalNegativeData
  {
    [JsonProperty("spcNegative")]
    public OptionalSpcNegative SpcNegative { get; set; } = new();
  }

  public class OptionalSpcNegative
  {
    [JsonProperty("spcNegativeResponse")]
    public List<OptionalSpcNegativeResponse> SpcNegativeResponse { get; set; } = new();

    [JsonProperty("summary")]
    public OptionalSummary Summary { get; set; } = new();
  }

  public class OptionalSpcNegativeResponse
  {
    [JsonProperty("occurrenceDate")]
    public string OccurrenceDate { get; set; } = string.Empty;

    [JsonProperty("contractId")]
    public string ContractId { get; set; } = string.Empty;

    [JsonProperty("creditorName")]
    public string CreditorName { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public double Amount { get; set; }

    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("federalUnit")]
    public string FederalUnit { get; set; } = string.Empty;

    [JsonProperty("creditorCity")]
    public string CreditorCity { get; set; } = string.Empty;

    [JsonProperty("creditorFederalUnit")]
    public string CreditorFederalUnit { get; set; } = string.Empty;
  }

  public class OptionalSummary
  {
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("balance")]
    public double Balance { get; set; }

    [JsonProperty("firstOccurrence")]
    public string FirstOccurrence { get; set; } = string.Empty;

    [JsonProperty("lastOccurrence")]
    public string LastOccurrence { get; set; } = string.Empty;
  }

  public class OptionalFacts
  {
    [JsonProperty("spcInquires")]
    public OptionalSpcInquires SpcInquires { get; set; } = new();

    [JsonProperty("spcInquirySummary")]
    public OptionalSpcInquirySummary SpcInquirySummary { get; set; } = new();
  }

  public class OptionalSpcInquires
  {
    [JsonProperty("spcInquiriesResponse")]
    public List<SpcInquiriesResponse> SpcInquiriesResponse { get; set; } = new List<SpcInquiriesResponse>();
  }

  public class SpcInquiriesResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string segmentDescription { get; set; } = string.Empty;
  }


  public class OptionalSpcInquirySummary
  {
    [JsonProperty("summary")]
    public OptionalSummary Summary { get; set; } = new();
  }

  public class OptionalAttributes
  {
    [JsonProperty("incomeCommitment")]
    public OptionalIncomeCommitment IncomeCommitment { get; set; } = new();
  }

  public class OptionalIncomeCommitment
  {
    [JsonProperty("scoring")]
    public int Scoring { get; set; }

    [JsonProperty("attributeModel")]
    public string AttributeModel { get; set; } = string.Empty;

    [JsonProperty("codeMessage")]
    public int CodeMessage { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;
  }
}
