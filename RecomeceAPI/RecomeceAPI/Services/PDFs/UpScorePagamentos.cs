using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecomeceAPI.Models;
using RecomeceAPI.Models.Serasa.Layout;
using RecomeceAPI.Services.Common;


namespace RecomeceAPI.Services.PDFs
{
  public class UpScorePagamentos : IDocument
  {
    public string GridHeaderColor { get; set; } = "blue";
    public decimal TotalDivida { get; set; }
    public List<CadProdutosFaixasPagamentoModel> CadProdutosFaixasPagamento = new List<CadProdutosFaixasPagamentoModel>();
    public MovPropostasModel MovPropostas = new MovPropostasModel();
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
              // Dados de proposta
              col.Item().PaddingTop(20).AlignCenter().Text("Proposta de Serviço").Bold().FontSize(18);
              col.Item().PaddingTop(20).Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn(3); // Nome (maior)
                  columns.RelativeColumn(2); // CPF
                  columns.RelativeColumn(1); // Score
                  columns.RelativeColumn(2); // Dívida
                  columns.RelativeColumn(2); // Serviço
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("NOME").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("CPF/CNPJ").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("SCORE").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Valor da Dívida").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Valor do Serviço").FontColor(Colors.White).Bold()));

                });

                void AddRowServico(string nome, string insc, string score, string divida, string servico)
                {
                  table.Cell().Element(CellStyle).Text(nome).FontSize(8);
                  table.Cell().Element(CellStyle).Text(insc).FontSize(8);
                  table.Cell().Element(CellStyle).Text(score).FontSize(8);
                  table.Cell().Element(CellStyle).Text(divida).FontSize(8);
                  table.Cell().Element(CellStyle).Text(servico).FontSize(8);
                }

                foreach (var item in MovPropostasBeneficiarios)
                {
                  AddRowServico(item.CadClientes.Nome, ExtensionService.AplicarMascaraCpfCnpj(item.CadClientes.Inscricao), item.Score.ToString(), ExtensionService.AplicarMascaraMoeda(item.ValorDivida), ExtensionService.AplicarMascaraMoeda(item.ValorContrato));
                }
              });


              col.Item().PaddingTop(20).AlignRight().Text($"Total do Serviço: {ExtensionService.AplicarMascaraMoeda(MovPropostas.ValorContrato)}").Bold().FontSize(11);
              col.Item().PaddingTop(5).AlignRight().Text($"Total do Desconto: {ExtensionService.AplicarMascaraMoeda(MovPropostas.ValorContrato - MovPropostas.ValorAprovado)}").Bold().FontSize(11);
              col.Item().PaddingTop(5).AlignRight().Text($"Valor da Proposta: {ExtensionService.AplicarMascaraMoeda(MovPropostas.ValorAprovado)}").Bold().FontSize(11);

              // Condições de Pagamento
              col.Item().PaddingTop(20).AlignCenter().Text("Condições de Pagamento").Bold().FontSize(18);
              col.Item().PaddingTop(20).Table(table =>
              {
                table.ColumnsDefinition(columns =>
                {
                  columns.RelativeColumn(2); // Nome (maior)
                  columns.RelativeColumn(2); // CPF
                  columns.RelativeColumn(1); // Score
                  columns.RelativeColumn(3); // Dívida
                });

                table.Header(header =>
                {
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Tipo de Pagamento").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Forma de Pagamento").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Nº de Parcelas").FontColor(Colors.White).Bold()));
                  header.Cell().Element(container => GetHeaderColor(container, GridHeaderColor).Element(c => c.Text("Descrição").FontColor(Colors.White).Bold()));

                });

                void AddRowPagamento(string tipo, string modo, string maxParc, string descr)
                {
                  table.Cell().Element(CellStyle).Text(tipo).FontSize(8);
                  table.Cell().Element(CellStyle).Text(modo).FontSize(8);
                  table.Cell().Element(CellStyle).Text(maxParc).FontSize(8);
                  table.Cell().Element(CellStyle).Text(descr).FontSize(8);
                }

                foreach (var item in CadProdutosFaixasPagamento)
                {
                  AddRowPagamento(GetTipo(item.Tipo), GetModo(item.Modo), GetParcela(item.ValorMaxParcelas), item.Descricao);
                }
              });


            });
      });
    }

    public string GetParcela(byte parc)
    {
      if (parc == 0)
        return " - ";
      else
        return parc.ToString();
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

    static IContainer BlueCellStyle(IContainer container)
    {
      return container
          .Background("#016087")         // Fundo laranja
          .AlignCenter()                 // Centraliza horizontalmente
          .AlignMiddle()                 // Centraliza verticalmente
          .Padding(4);                   // Padding interno
    }
    static IContainer OrangeCellStyle(IContainer container)
    {
      return container
          .Background("#FF6600")         // Fundo laranja
          .AlignCenter()                 // Centraliza horizontalmente
          .AlignMiddle()                 // Centraliza verticalmente
          .Padding(4);                   // Padding interno
    }
  }
}
