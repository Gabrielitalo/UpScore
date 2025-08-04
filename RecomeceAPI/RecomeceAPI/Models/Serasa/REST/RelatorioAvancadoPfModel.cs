using System.Text.Json.Serialization;

namespace RecomeceAPI.Models.Serasa.REST
{
  // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

  public class RelatorioAvancadoPfModel
  {
    public List<Report> reports { get; set; } = new List<Report>();
    public OptionalFeaturesModel optionalFeatures { get; set; } = new OptionalFeaturesModel();
    public double debitTotal { get; set; } = 0;
    public int debitQtd { get; set; } = 0;
    public void CalcDebit()
    {
      reports[0].CalcDebit();
      debitTotal = reports[0].debitTotal;
      debitQtd = reports[0].debitQtd; 
      if (optionalFeatures?.NegativeData?.SpcNegative?.Summary?.Balance > 0)
      {
        debitTotal += optionalFeatures.NegativeData.SpcNegative.Summary.Balance;
        debitQtd += optionalFeatures.NegativeData.SpcNegative.Summary.Count;
      }
    }
  }


  public class Address
  {
    public string addressLine { get; set; } = string.Empty;
    public string zipCode { get; set; } = string.Empty;
    public string country { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
  }

  public class Bankrupts
  {
    public List<BankruptsResponse> bankruptsResponse { get; set; } = new List<BankruptsResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class BankruptsResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string companyDocumentId { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string companyLegalNatureID { get; set; } = string.Empty;
    public string companyLegalNature { get; set; } = string.Empty;
  }

  public class Check
  {
    public List<CheckResponse> checkResponse { get; set; } = new List<CheckResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class CheckResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string legalSquare { get; set; } = string.Empty;
    public int bankId { get; set; }
    public string bankName { get; set; } = string.Empty;
    public int bankAgencyId { get; set; }
    public int checkCount { get; set; }
    public string city { get; set; } = string.Empty;
    public string federalUnit { get; set; } = string.Empty;
    public string checkNumber { get; set; } = string.Empty;
    public int alinea { get; set; }
  }

  public class CollectionRecords
  {
    public List<CollectionRecordsResponse> collectionRecordsResponse { get; set; } = new List<CollectionRecordsResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class CollectionRecordsResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string legalNatureId { get; set; } = string.Empty;
    public string legalNature { get; set; } = string.Empty;
    public string contractId { get; set; } = string.Empty;
    public string creditorName { get; set; } = string.Empty;
    public double amount { get; set; }
    public string city { get; set; } = string.Empty;
    public string federalUnit { get; set; } = string.Empty;
    public bool principal { get; set; }
  }

  public class CreditInquiriesQuantity
  {
    public string inquiryDate { get; set; } = string.Empty;
    public int occurrences { get; set; }
  }

  public class Facts
  {
    public Inquiry inquiry { get; set; } = new Inquiry();
    public InquirySummary inquirySummary { get; set; } = new InquirySummary();
    public StolenDocuments stolenDocuments { get; set; } = new StolenDocuments();
    public JudgementFilings judgementFilings { get; set; } = new JudgementFilings();
    public Bankrupts bankrupts { get; set; } = new Bankrupts();
  }

  public class Inquiry
  {
    public List<InquiryResponse> inquiryResponse { get; set; } = new List<InquiryResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class InquiryQuantity
  {
    public int actual { get; set; }
    public List<CreditInquiriesQuantity> creditInquiriesQuantity { get; set; } = new List<CreditInquiriesQuantity>();
  }

  public class InquiryResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string segmentDescription { get; set; } = string.Empty;
    public int daysQuantity { get; set; }
  }

  public class InquirySummary
  {
    public InquiryQuantity inquiryQuantity { get; set; } = new InquiryQuantity();
    public Summary summary { get; set; } = new Summary();
  }

  public class JudgementFilings
  {
    public List<JudgementFilingsResponse> judgementFilingsResponse { get; set; } = new List<JudgementFilingsResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class JudgementFilingsResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string legalNatureId { get; set; } = string.Empty;
    public string legalNature { get; set; } = string.Empty;
    public string civilCourt { get; set; } = string.Empty;
    public double amount { get; set; }
    public string distributor { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
  }

  public class NegativeData
  {
    public Pefin pefin { get; set; } = new Pefin();
    public Refin refin { get; set; } = new Refin();
    public Notary notary { get; set; } = new Notary();
    public Check check { get; set; } = new Check();
    public CollectionRecords collectionRecords { get; set; } = new CollectionRecords();
  }

  public class Notary
  {
    public List<NotaryResponse> notaryResponse { get; set; } = new List<NotaryResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class NotaryResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public double amount { get; set; }
    public string officeNumber { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string federalUnit { get; set; } = string.Empty;
  }

  public class Partner
  {
    public List<PartnershipResponse> partnershipResponse { get; set; } = new List<PartnershipResponse>();
  }

  public class PartnershipResponse
  {
    public string businessDocument { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public double participationPercentage { get; set; }
    public string companyStatus { get; set; } = string.Empty;
    public string companyState { get; set; } = string.Empty;
    public string updateDate { get; set; } = string.Empty;
    public string participationInitialDate { get; set; } = string.Empty;
    public bool hasNegative { get; set; }
  }

  public class Pefin
  {
    public List<PefinResponse> pefinResponse { get; set; } = new List<PefinResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class PefinResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string legalNatureId { get; set; } = string.Empty;
    public string legalNature { get; set; } = string.Empty;
    public string contractId { get; set; } = string.Empty;
    public string creditorName { get; set; } = string.Empty;
    public double amount { get; set; }
    public string federalUnit { get; set; } = string.Empty;
    public bool principal { get; set; }
  }

  public class Phone
  {
    public int regionCode { get; set; }
    public int areaCode { get; set; }
    public int phoneNumber { get; set; }
    public string phoneType { get; set; } = string.Empty;
  }

  public class PhoneNumber
  {
    public int regionCode { get; set; }
    public int areaCode { get; set; }
    public int phoneNumber { get; set; }
  }

  public class Refin
  {
    public List<RefinResponse> refinResponse { get; set; } = new List<RefinResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class RefinResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string legalNatureId { get; set; } = string.Empty;
    public string legalNature { get; set; } = string.Empty;
    public string contractId { get; set; } = string.Empty;
    public string creditorName { get; set; } = string.Empty;
    public double amount { get; set; }
    public string federalUnit { get; set; } = string.Empty;
    public bool principal { get; set; }
  }

  public class Registration
  {
    public string documentNumber { get; set; } = string.Empty;
    public string consumerName { get; set; } = string.Empty;
    public string motherName { get; set; } = string.Empty;
    public string consumerGender { get; set; } = string.Empty;
    public string birthDate { get; set; } = string.Empty;
    public string profession { get; set; } = string.Empty;
    public string occupation { get; set; } = string.Empty;
    public string statusRegistration { get; set; } = string.Empty;
    public string statusDate { get; set; } = string.Empty;
    public Address address { get; set; } = new Address();
    public List<Phone> phones { get; set; } = new List<Phone>();
  }

  public class Report
  {
    public double debitTotal { get; set; } = 0;
    public int debitQtd { get; set; } = 0;
    public string reportName { get; set; } = string.Empty;
    public Registration registration { get; set; } = new Registration();
    public NegativeData negativeData { get; set; } = new NegativeData();
    public Facts facts { get; set; } = new Facts();
    public Partner partner { get; set; } = new Partner();
    public Score score { get; set; } = new Score();
    public Attributes attributes { get; set; } = new Attributes();

    public void CalcDebit()
    {
      debitTotal = negativeData.pefin.summary.balance + negativeData.refin.summary.balance + negativeData.notary.summary.balance + negativeData.check.summary.balance + negativeData.collectionRecords.summary.balance + facts.judgementFilings.summary.balance;
      debitQtd = negativeData.pefin.summary.count + negativeData.refin.summary.count + negativeData.notary.summary.count + negativeData.check.summary.count + negativeData.collectionRecords.summary.count + facts.judgementFilings.summary.count;
    }
  }
  public class Attributes
  {
    public List<AttributesResponse> attributesResponse { get; set; } = new List<AttributesResponse> ();
  }

  public class AttributesResponse
  {
    public string scoring { get; set; } = string.Empty;
    public string attributeModel { get; set; } = string.Empty;
    public string codeMessage { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
  }
  public class StolenDocuments
  {
    public List<StolenDocumentsResponse> stolenDocumentsResponse { get; set; } = new List<StolenDocumentsResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class StolenDocumentsResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public DateTime inclusionDate { get; set; }
    public string documentType { get; set; } = string.Empty;
    public string documentNumber { get; set; } = string.Empty;
    public string issuingAuthority { get; set; } = string.Empty;
    public string detailedReason { get; set; } = string.Empty;
    public string occurrenceState { get; set; } = string.Empty;
    public PhoneNumber phoneNumber { get; set; } = new PhoneNumber();
  }

  public class Summary
  {
    public int count { get; set; }
    public double balance { get; set; }
    public string firstOccurrence { get; set; } = string.Empty;
    public string lastOccurrence { get; set; } = string.Empty;
  }

  public class Score()
  {
    [JsonPropertyName("score")]
    public int score { get; set; }

    [JsonPropertyName("scoreModel")]
    public string scoreModel { get; set; } = string.Empty;

    [JsonPropertyName("range")]
    public string range { get; set; } = string.Empty;

    [JsonPropertyName("defaultRate")]
    public string defaultRate { get; set; } = string.Empty;

    [JsonPropertyName("codeMessage")]
    public int codeMessage { get; set; }

    [JsonPropertyName("message")]
    public string message { get; set; } = string.Empty;
  }

}