using System.Diagnostics;
using System.Text.Json.Serialization;

namespace RecomeceAPI.Models.Serasa.REST.PJ
{

  // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

  public class RelatorioAvancadoPjModel
  {
    public List<Report> reports { get; set; } = new List<Report>();
    public OptionalFeatures optionalFeatures { get; set; } = new OptionalFeatures();
    public double debitTotal { get; set; } = 0;
    public int debitQtd { get; set; } = 0;

    public void CalcDebit()
    {
      reports[0].CalcDebit();
      debitTotal = reports[0].debitTotal;
      debitQtd = reports[0].debitQtd;
      if (optionalFeatures?.spcAnnotations?.summary?.balance > 0)
      {
        debitTotal += optionalFeatures.spcAnnotations.summary.balance;
        debitQtd += optionalFeatures.spcAnnotations.summary.count;
      }
    }
  }

  public class Address
  {
    public string addressLine { get; set; } = string.Empty;
    public string zipCode { get; set; } = string.Empty;
    public string district { get; set; } = string.Empty;
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
    public string eventDate { get; set; } = string.Empty;
    public string origin { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
    public string varaCourt { get; set; } = string.Empty;
    public string eventType { get; set; } = string.Empty;
  }

  public class Check
  {
    public List<CheckResponse> checkResponse { get; set; } = new List<CheckResponse>();
    public Summary summary { get; set; } = new Summary();
  }

  public class CheckResponse
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public int alinea { get; set; }
    public string bankName { get; set; } = string.Empty;
    public int bankAgencyId { get; set; }
    public string checkNumber { get; set; } = string.Empty;
    public int checkCount { get; set; }
    public string city { get; set; } = string.Empty;
    public string federalUnit { get; set; } = string.Empty;
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
    public string debtMode { get; set; } = string.Empty;
  }

  public class CompanyData
  {
    public double socialCapitalValue { get; set; }
    public double accomplishedValue { get; set; }
    public string informationUpdateDate { get; set; } = string.Empty;
    public string countryOrigin { get; set; } = string.Empty;
    public string controlType { get; set; } = string.Empty;
    public string nature { get; set; } = string.Empty;
  }

  public class CreditInquiriesQuantity
  {
    public string inquiryDate { get; set; } = string.Empty;
    public int occurrences { get; set; }
  }

  public class DirectorCompleteReport
  {
    public List<DirectorsList> directorsList { get; set; } = new List<DirectorsList>();
  }

  public class DirectorsList
  {
    public string documentType { get; set; } = string.Empty;
    public string documentId { get; set; } = string.Empty;
    public string role { get; set; } = string.Empty;
    public string sinceDate { get; set; } = string.Empty;
    public string nationality { get; set; } = string.Empty;
    public bool restrictionSign { get; set; }
    public bool documentConsistency { get; set; }
    public string informationUpdateDate { get; set; } = string.Empty;
  }

  public class Facts
  {
    public JudgementFilings judgementFilings { get; set; } = new JudgementFilings();
    public Bankrupts bankrupts { get; set; } = new Bankrupts();
    public InquiryCompanyResponse inquiryCompanyResponse { get; set; } = new InquiryCompanyResponse();
  }

  public class Historical
  {
    public string inquiryDate { get; set; } = string.Empty;
    public int occurrences { get; set; }
  }

  public class IdentificationReport
  {
    public string updateDate { get; set; } = string.Empty;
    public string documentNumber { get; set; } = string.Empty;
    public string statusRegistration { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string companyAlias { get; set; } = string.Empty;
    public Address address { get; set; } = new Address();
    public Phone phone { get; set; } = new Phone();
    public string companyUrl { get; set; } = string.Empty;
    public string partnership { get; set; } = string.Empty;
    public string companyRegister { get; set; } = string.Empty;
    public string companyRegisterDate { get; set; } = string.Empty;
    public string companyFoundation { get; set; } = string.Empty;
    public int numberEmployees { get; set; }
    public string taxOption { get; set; } = string.Empty;
    public string stateRegistration { get; set; } = string.Empty;
    public string economicActivity { get; set; } = string.Empty;
    public double importPurchases { get; set; }
    public double exportSales { get; set; }
    public string cnae { get; set; } = string.Empty;
    public string serasaActiveCode { get; set; } = string.Empty;
    public string branchOffices { get; set; } = string.Empty;
    public string nireNumber { get; set; } = string.Empty;
    public List<PredecessorList> predecessorList { get; set; } = new List<PredecessorList>();
    public string legalNatureCode { get; set; } = string.Empty;
  }

  public class InquiryCompanyResponse
  {
    public List<Result> results { get; set; } = new List<Result>();
    public Quantity quantity { get; set; } = new Quantity();
  }

  public class InquiryQuantity
  {
    public int actual { get; set; }
    public List<CreditInquiriesQuantity> creditInquiriesQuantity { get; set; } = new List<CreditInquiriesQuantity>();
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
    public double amount { get; set; }
    public string distributor { get; set; } = string.Empty;
    public string civilCourt { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string state { get; set; } = string.Empty;
    public bool principal { get; set; }
  }

  public class NegativeData
  {
    public Pefin pefin { get; set; } = new Pefin();
    public Refin refin { get; set; } = new Refin();
    public CollectionRecords collectionRecords { get; set; } = new CollectionRecords();
    public Check check { get; set; } = new Check();
    public Notary notary { get; set; } = new Notary();
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

  public class OptionalFeatures
  {
    public double debitTotal { get; set; } = 0;
    public int debitQtd { get; set; } = 0;
    public SpcAnnotations spcAnnotations { get; set; } = new SpcAnnotations();
    public SpcMarketInquiries spcMarketInquiries { get; set; } = new SpcMarketInquiries();
    public SpcMarketInquirySummary spcMarketInquirySummary { get; set; } = new SpcMarketInquirySummary();

    public void CalcDebit()
    {
    }
  }

  public class PartnerCompleteReport
  {
    public List<PartnersList> partnersList { get; set; } = new List<PartnersList>();
  }

  public class PartnersList
  {
    public string documentType { get; set; } = string.Empty;
    public string documentId { get; set; } = string.Empty;
    public string nationality { get; set; } = string.Empty;
    public bool restrictionSign { get; set; }
    public bool documentConsistency { get; set; }
    public double capitalTotalValue { get; set; }
    public double capitalVoterValue { get; set; }
    public string name { get; set; } = string.Empty;
    public string sinceDate { get; set; } = string.Empty;
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
    public string areaCode { get; set; } = string.Empty;
    public string phoneNumber { get; set; } = string.Empty;
  }

  public class PredecessorList
  {
    public string predecessorName { get; set; } = string.Empty;
    public string predecessorDate { get; set; } = string.Empty;
  }

  public class QSAReport
  {
    public CompanyData companyData { get; set; } = new CompanyData();
    public PartnerCompleteReport partnerCompleteReport { get; set; } = new PartnerCompleteReport();
    public DirectorCompleteReport directorCompleteReport { get; set; } = new DirectorCompleteReport();
  }

  public class Quantity
  {
    public int actual { get; set; }
    public List<Historical> historical { get; set; } = new List<Historical>();
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
    public bool principal { get; set; }
  }


  public class Registration
  {
    [JsonPropertyName("companyDocument")]
    public string companyDocument { get; set; } = string.Empty;

    [JsonPropertyName("companyName")]
    public string companyName { get; set; } = string.Empty;

    [JsonPropertyName("foundationDate")]
    public string foundationDate { get; set; } = string.Empty;

    [JsonPropertyName("statusRegistration")]
    public string statusRegistration { get; set; } = string.Empty;

    [JsonPropertyName("address")]
    public Address address { get; set; } = new Address();
  }

  public class Report
  {
    public double debitTotal { get; set; } = 0;
    public int debitQtd { get; set; } = 0;
    public string reportName { get; set; } = string.Empty;
    public IdentificationReport identificationReport { get; set; } = new IdentificationReport();
    public Registration registration { get; set; } = new Registration();
    public QSAReport QSAReport { get; set; } = new QSAReport();
    public NegativeData negativeData { get; set; } = new NegativeData();
    public Facts facts { get; set; } = new Facts();
    public Score score { get; set; } = new Score();
    public Scores scores { get; set; } = new Scores();
    public Attributes attributes { get; set; } = new Attributes();

    public void CalcDebit()
    {
      debitTotal = negativeData.pefin.summary.balance + negativeData.refin.summary.balance + negativeData.notary.summary.balance + negativeData.check.summary.balance + negativeData.collectionRecords.summary.balance + facts.judgementFilings.summary.balance;
      debitQtd = negativeData.pefin.summary.count + negativeData.refin.summary.count + negativeData.notary.summary.count + negativeData.check.summary.count + negativeData.collectionRecords.summary.count + facts.judgementFilings.summary.count;
    }
  }

  public class Scores
  {
    public List<ScoreResponse> scoreResponse { get; set; } = new List<ScoreResponse>();
  }
  public class ScoreResponse
  {
    public string score { get; set; } = string.Empty;
    public string scoreModel { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
    public bool billing { get; set; }
  }

  public class Attributes
  {
    public List<AttributesResponse> attributesResponse { get; set; } = new List<AttributesResponse>();
  }

  public class AttributesResponse
  {
    public string scoring { get; set; } = string.Empty;
    public string attributeModel { get; set; } = string.Empty;
    public string codeMessage { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
  }

  public class Result
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string companyDocumentId { get; set; } = string.Empty;
    public int daysQuantity { get; set; }
  }

  public class Score
  {
    public double score { get; set; }
    public string scoreModel { get; set; } = string.Empty;
    public string defaultRate { get; set; } = string.Empty;
    public string message { get; set; } = string.Empty;
  }

  public class SpcAnnotations
  {
    public List<SpcAnnotationsResponseList> spcAnnotationsResponseList { get; set; } = new List<SpcAnnotationsResponseList>();
    public Summary summary { get; set; } = new Summary();
  }

  public class SpcAnnotationsResponseList
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string inclusionDate { get; set; } = string.Empty;
    public string contractId { get; set; } = string.Empty;
    public string creditorName { get; set; } = string.Empty;
    public string city { get; set; } = string.Empty;
    public string federalUnit { get; set; } = string.Empty;
    public double amount { get; set; }
    public bool principal { get; set; }
  }

  public class SpcMarketInquiries
  {
    public List<SpcMarketInquiriesResponseList> spcMarketInquiriesResponseList { get; set; } = new List<SpcMarketInquiriesResponseList>();
  }

  public class SpcMarketInquiriesResponseList
  {
    public string occurrenceDate { get; set; } = string.Empty;
    public string companyName { get; set; } = string.Empty;
    public string companyDocumentId { get; set; } = string.Empty;
    public int daysQuantity { get; set; }
  }


  public class SpcMarketInquirySummary
  {
    public InquiryQuantity inquiryQuantity { get; set; } = new InquiryQuantity();
  }

  public class Summary
  {
    public string firstOccurrence { get; set; } = string.Empty;
    public string lastOccurrence { get; set; } = string.Empty;
    public int count { get; set; }
    public double balance { get; set; }
  }


}
