using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Serasa.Layout;
using RecomeceAPI.Models.Serasa.REST;
using RecomeceAPI.Models.Serasa.REST.PJ;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Services.PDFs
{
  public class UpScoreBeneficiario : IDocument
  {
    public decimal TotalDivida { get; set; }
    public string GridHeaderColor { get; set; } = "blue";
    public int PersonType { get; set; } = 1;
    public Concentre Concentre = new Concentre();
    public RelatorioAvancadoPfModel RelatoroAvancadoPf = new RelatorioAvancadoPfModel();
    public RelatorioAvancadoPjModel RelatoroAvancadoPj = new RelatorioAvancadoPjModel();
    public MovPropostasModel MovPropostas = new MovPropostasModel();
    public List<CadProdutosFaixasPagamentoModel> CadProdutosFaixasPagamento = new List<CadProdutosFaixasPagamentoModel>();
    public List<MovPropostasBeneficiariosModel> MovPropostasBeneficiarios = new List<MovPropostasBeneficiariosModel>();
    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      container.Page(page =>
      {
        page.Margin(30);
        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

        page.Content()
            .Column(col =>
            {
              // Cabeçalho
              col.Item().AlignCenter().Text(PersonType == 1 ? RelatoroAvancadoPf.reports[0].registration.consumerName : RelatoroAvancadoPj.reports[0].identificationReport.companyName).SemiBold().FontSize(16);
              col.Item().PaddingVertical(10).Row(row =>
              {
                row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(10).Text($"CPF/CNPJ\n{ExtensionService.AplicarMascaraCpfCnpj(PersonType == 1 ? RelatoroAvancadoPf.reports[0].registration.documentNumber : RelatoroAvancadoPj.reports[0].identificationReport.documentNumber)}");
                row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(10).AlignCenter().Text(text => { text.Line("SCORE"); text.Line(PersonType == 1 ? RelatoroAvancadoPf.reports[0].score.score.ToString() : RelatoroAvancadoPj.reports[0].score.score.ToString()); });

                row.RelativeItem().Background(Colors.Grey.Lighten3).Padding(10).Text($"VALOR DA DÍVIDA\n {ExtensionService.AplicarMascaraMoeda(PersonType == 1 ? (decimal)RelatoroAvancadoPf.reports[0].debitTotal : (decimal)RelatoroAvancadoPj.reports[0].debitTotal)}").AlignRight();
              });

              if (PersonType == 1)
              {
                col.Item().PaddingTop(20).Element(container =>
                {
                  container
                      .Border(1)
                      .BorderColor(Colors.Grey.Lighten2)
                      .Background(Colors.White)
                      .Padding(10)
                      .Column(column =>
                      {
                        column.Spacing(2);
                        column.Item().Text("Renda Estimada")
                  .FontSize(14);

                        column.Item().Text(CalcRendaEstimada())
                  .FontColor(GetHexColor(GridHeaderColor))
                  .Bold()
                  .FontSize(14);
                      });
                });
              }

              if (PersonType == 2)
              {
                col.Item().PaddingTop(20).Element(container =>
                {
                  container
                      .Border(1)
                      .BorderColor(Colors.Grey.Lighten2) // Laranja
                      .Background(Colors.White)
                      .Padding(10)
                      .Column(column =>
                      {
                        column.Spacing(2);
                        column.Item().Text("Limite de Crédito PJ")
                  .FontSize(14);

                        column.Item().Text(CalcLimiteCredito())
                  .FontColor(GetHexColor(GridHeaderColor))
                  .Bold()
                  .FontSize(14);
                      });
                });
              }

              // Pendências Comerciais
              col.Item().PaddingTop(20).Text("Pendências Comerciais (PEFIN)").Bold().FontSize(14);
              col.Item().Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.ConstantColumn(100);
                  columns.ConstantColumn(120);
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("MODALIDADE").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("NOME").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("DATA").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("VALOR").FontColor(Colors.White).Bold()));
                });

                void AddRow(string modalidade, string nome, string data, string valor)
                {
                  table.Cell().Element(CellStyle).Text(modalidade).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(nome).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(data).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(valor).FontSize(8).AlignCenter();
                }

                if (PersonType == 1)
                {
                  foreach (var item in RelatoroAvancadoPf.reports[0].negativeData.pefin.pefinResponse)
                  {
                    AddRow(item.legalNature, item.creditorName, ExtensionService.ConverterParaDataBr(item.occurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPf?.reports[0]?.negativeData?.pefin?.pefinResponse?.Count == 0)
                    GetNadaConsta(col);
                }
                else if (PersonType == 2)
                {
                  foreach (var item in RelatoroAvancadoPj.reports[0].negativeData.pefin.pefinResponse)
                  {
                    AddRow(item.legalNature, item.creditorName, ExtensionService.ConverterParaDataBr(item.occurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPj?.reports[0]?.negativeData?.pefin?.pefinResponse?.Count == 0)
                    GetNadaConsta(col);
                }
              });

              // Pendências Bancárias
              col.Item().PaddingTop(20).Text("Pendências Bancárias (REFIN)").Bold().FontSize(14);
              col.Item().Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn();
                  columns.RelativeColumn();
                  columns.ConstantColumn(100);
                  columns.ConstantColumn(50);
                  columns.ConstantColumn(120);
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("MODALIDADE").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CREDOR").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("DATA").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("UF").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("VALOR").FontColor(Colors.White).Bold()));
                });

                void AddBankRow(string modalidade, string companhia, string data, string uf, string valor)
                {
                  table.Cell().Element(CellStyle).Text(modalidade).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(companhia).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(data).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(uf).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(valor).FontSize(8).AlignCenter();
                }

                if (PersonType == 1)
                {
                  foreach (var item in RelatoroAvancadoPf.reports[0].negativeData.refin.refinResponse)
                  {
                    AddBankRow(item.legalNature, item.creditorName, ExtensionService.ConverterParaDataBr(item.occurrenceDate), item.federalUnit, ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPf.reports[0]?.negativeData?.refin?.refinResponse?.Count == 0)
                    GetNadaConsta(col);
                }
                else if (PersonType == 2)
                {
                  foreach (var item in RelatoroAvancadoPj.reports[0].negativeData.refin.refinResponse)
                  {
                    AddBankRow(item.legalNature, item.creditorName, ExtensionService.ConverterParaDataBr(item.occurrenceDate), "-", ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPj.reports[0]?.negativeData?.refin?.refinResponse?.Count == 0)
                    GetNadaConsta(col);
                }
              });

              // Protestos
              col.Item().PaddingTop(20).Text("Protestos").Bold().FontSize(14);
              col.Item().Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.ConstantColumn(50);
                  columns.RelativeColumn();
                  columns.ConstantColumn(100);
                  columns.ConstantColumn(120);
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("UF").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CIDADE").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("DATA").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("VALOR").FontColor(Colors.White).Bold()));
                });

                void AddProtesto(string uf, string cidade, string data, string valor)
                {
                  table.Cell().Element(CellStyle).Text(uf).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(cidade).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(data).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(valor).FontSize(8).AlignCenter();
                }

                if (PersonType == 1)
                {
                  foreach (var item in RelatoroAvancadoPf.reports[0].negativeData.notary.notaryResponse)
                  {
                    AddProtesto(item.federalUnit, item.city, ExtensionService.ConverterParaDataBr(item.occurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPf.reports[0].negativeData?.notary?.notaryResponse?.Count == 0)
                    GetNadaConsta(col);
                }
                else if (PersonType == 2)
                {
                  foreach (var item in RelatoroAvancadoPj.reports[0].negativeData.notary.notaryResponse)
                  {
                    AddProtesto(item.federalUnit, item.city, ExtensionService.ConverterParaDataBr(item.occurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.amount));
                  }

                  if (RelatoroAvancadoPj.reports[0]?.negativeData?.notary?.notaryResponse?.Count == 0)
                    GetNadaConsta(col);
                }
              });

              // Participações Societárias
              if (PersonType == 1)
              {
                col.Item().PaddingTop(20).Text("Participações Societárias").Bold().FontSize(14);
                col.Item().Table(table =>
                {
                  table.ColumnsDefinition(columns =>
                  {
                    columns.ConstantColumn(100);
                    columns.RelativeColumn();
                    columns.ConstantColumn(100);
                  });

                  table.Header(header =>
                  {
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CNPJ").FontColor(Colors.White).Bold()));
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("RAZÃO SOCIAL").FontColor(Colors.White).Bold()));
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CAPITAL").FontColor(Colors.White).Bold()));
                  });

                  void AddPart(string cnpj, string razsoc, string capital)
                  {
                    table.Cell().Element(CellStyle).Text(cnpj).FontSize(8).AlignCenter();
                    table.Cell().Element(CellStyle).Text(razsoc).FontSize(8).AlignCenter();
                    table.Cell().Element(CellStyle).Text(capital).FontSize(8).AlignCenter();
                  }

                  foreach (var item in RelatoroAvancadoPf.reports[0].partner.partnershipResponse)
                  {
                    AddPart(ExtensionService.AplicarMascaraCpfCnpj(item.businessDocument), item.companyName, ExtensionService.AplicarMascaraMoeda((decimal)item.participationPercentage));
                  }
                });
                if (RelatoroAvancadoPf.reports[0]?.partner?.partnershipResponse?.Count == 0)
                  GetNadaConsta(col);
              }

              // Quadro societário - QSA
              if (PersonType == 2)
              {
                col.Item().PaddingTop(20).Text("Quadro societário").Bold().FontSize(14);
                col.Item().Table(table =>
                {
                  table.ColumnsDefinition(columns =>
                  {
                    columns.RelativeColumn();
                    columns.ConstantColumn(100);
                    columns.ConstantColumn(100);
                  });

                  table.Header(header =>
                  {
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("SÓCIO/ACIONISTA").FontColor(Colors.White).Bold()));
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CAPITAL").FontColor(Colors.White).Bold()));
                    header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("ENTRADA").FontColor(Colors.White).Bold()));
                  });

                  void AddPart(string cnpj, string razsoc, string capital)
                  {
                    table.Cell().Element(CellStyle).Text(cnpj).FontSize(8).AlignCenter();
                    table.Cell().Element(CellStyle).Text(razsoc).FontSize(8).AlignCenter();
                    table.Cell().Element(CellStyle).Text(capital).FontSize(8).AlignCenter();
                  }

                  foreach (var item in RelatoroAvancadoPj.reports[0].QSAReport.partnerCompleteReport.partnersList)
                  {
                    AddPart(item.name, ExtensionService.AplicarMascaraMoeda((decimal)item.capitalTotalValue), ExtensionService.ConverterParaDataBr(item.sinceDate));
                  }
                });

                if (RelatoroAvancadoPj.reports[0]?.QSAReport?.partnerCompleteReport?.partnersList?.Count == 0)
                  GetNadaConsta(col);
              }

              // Anotações SPC
              col.Item().PaddingTop(20).Text("Consultas SPC").Bold().FontSize(14);
              col.Item().Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.ConstantColumn(50);
                  columns.ConstantColumn(100);
                  columns.RelativeColumn(250);
                  columns.ConstantColumn(100);
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("DATA").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CNPJ").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("RAZÃO SOCIAL").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("QUANTIDADE").FontColor(Colors.White).Bold()));
                });

                void AddQuerySPC(string data, string valor, string credor, string cidade)
                {
                  table.Cell().Element(CellStyle).Text(data).FontSize(8);
                  table.Cell().Element(CellStyle).Text(valor).FontSize(8);
                  table.Cell().Element(CellStyle).Text(credor).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(cidade).FontSize(8).AlignCenter();
                }

                if (PersonType == 1)
                {
                  foreach (var item in RelatoroAvancadoPf.optionalFeatures.Facts.SpcInquires.SpcInquiriesResponse)
                  {
                    AddQuerySPC(ExtensionService.ConverterParaDataBr(item.occurrenceDate), "-", item.segmentDescription, "-");
                  }

                  if (RelatoroAvancadoPf.optionalFeatures?.Facts?.SpcInquires?.SpcInquiriesResponse?.Count == 0)
                    GetNadaConsta(col);
                }
                else if (PersonType == 2)
                {
                  foreach (var item in RelatoroAvancadoPj.optionalFeatures.spcMarketInquiries.spcMarketInquiriesResponseList)
                  {
                    AddQuerySPC(ExtensionService.ConverterParaDataBr(item.occurrenceDate), item.companyDocumentId, item.companyName, item.daysQuantity.ToString());
                  }

                  if (RelatoroAvancadoPj.optionalFeatures?.spcMarketInquiries?.spcMarketInquiriesResponseList?.Count == 0)
                    GetNadaConsta(col);
                }

              });

              // Anotações SPC
              col.Item().PaddingTop(20).Text("Anotações SPC").Bold().FontSize(14);
              col.Item().Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.ConstantColumn(50);
                  columns.ConstantColumn(50);
                  columns.RelativeColumn();
                  columns.ConstantColumn(100);
                  columns.ConstantColumn(40);
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("DATA").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("VALOR").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CREDOR").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CIDADE").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("UF").FontColor(Colors.White).Bold()));
                });

                void AddSPC(string data, string valor, string credor, string cidade, string uf)
                {
                  table.Cell().Element(CellStyle).Text(data).FontSize(8);
                  table.Cell().Element(CellStyle).Text(valor).FontSize(8);
                  table.Cell().Element(CellStyle).Text(credor).FontSize(8).AlignCenter();
                  table.Cell().Element(CellStyle).Text(cidade).FontSize(8);
                  table.Cell().Element(CellStyle).Text(uf).FontSize(8);
                }

                if (PersonType == 1)
                {
                  foreach (var item in RelatoroAvancadoPf.optionalFeatures.NegativeData.SpcNegative.SpcNegativeResponse)
                  {
                    AddSPC(ExtensionService.ConverterParaDataBr(item.OccurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.Amount), item.CreditorName, item.City, item.FederalUnit);
                  }

                  if (RelatoroAvancadoPf.optionalFeatures?.NegativeData?.SpcNegative?.SpcNegativeResponse?.Count == 0)
                    GetNadaConsta(col);
                }
                else if (PersonType == 2)
                {
                  foreach (var item in RelatoroAvancadoPj.optionalFeatures.spcAnnotations.spcAnnotationsResponseList)
                  {
                    AddSPC(ExtensionService.ConverterParaDataBr(item.occurrenceDate), ExtensionService.AplicarMascaraMoeda((decimal)item.amount), item.creditorName, item.city, item.federalUnit);
                  }

                  if (RelatoroAvancadoPj.optionalFeatures?.spcAnnotations.spcAnnotationsResponseList.Count == 0)
                    GetNadaConsta(col);
                }
              });
            });
      });
    }

    public TextBlockDescriptor GetNadaConsta(ColumnDescriptor col)
    {
      return col.Item().PaddingTop(20).Text("NADA CONSTA").FontSize(12).AlignCenter();
    }
    public string CalcRendaEstimada()
    {
      if (RelatoroAvancadoPf.reports[0].attributes?.attributesResponse.Count > 0)
        return $"{ExtensionService.AplicarMascaraMoeda(ExtensionService.SafeConvert<decimal>(RelatoroAvancadoPf.reports[0].attributes?.attributesResponse[0].scoring ?? "0"))}" ?? "Não Calculado";
      else
        return "Não Calculado";
    }
    public string CalcLimiteCredito()
    {
      if (RelatoroAvancadoPj.reports[0].scores.scoreResponse.Count > 0)
        return $"{ExtensionService.AplicarMascaraMoeda(ExtensionService.SafeConvert<decimal>(RelatoroAvancadoPj.reports[0].scores?.scoreResponse[0].score ?? "0"))}" ?? "Não Calculado";
      else
        return "Não Calculado";
    }
    public string GetTipo(byte tipo)
    {
      return tipo == 1 ? "A Vista" : "Parcelado";
    }
    public string GetModo(byte modo)
    {
      if (modo == 1)
        return "Boleto";
      else if (modo == 2)
        return "Cartão de Crédito";
      else if (modo == 3)
        return "Pix";
      else
        return "";
    }
    static IContainer CellStyle(IContainer container)
    {
      return container
          .BorderBottom(1)
          .BorderColor(Colors.Grey.Lighten2)
          .PaddingVertical(4)
          .PaddingHorizontal(2);
    }
    static IContainer GetHeaderColor(IContainer container, string color)
    {
      return color switch
      {
        "orange" => OrangeCellStyle(container),
        "blue" => BlueCellStyle(container),
        _ => BlueCellStyle(container)
      };
    }

    static string GetHexColor(string color)
    {
      return color == "blue" ? "#016087" : "#FF6600";
    }

    static IContainer OrangeCellStyle(IContainer container)
    {
      return container
          .Background("#FF6600")         // Fundo laranja
          .AlignCenter()                 // Centraliza horizontalmente
          .AlignMiddle()                 // Centraliza verticalmente
          .Padding(4);                   // Padding interno
    }
    static IContainer BlueCellStyle(IContainer container)
    {
      return container
          .Background("#016087")         // Fundo laranja
          .AlignCenter()                 // Centraliza horizontalmente
          .AlignMiddle()                 // Centraliza verticalmente
          .Padding(4);                   // Padding interno
    }
  }
}
