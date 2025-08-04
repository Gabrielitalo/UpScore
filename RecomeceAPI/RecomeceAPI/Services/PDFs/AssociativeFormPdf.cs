using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecomeceAPI.Services.Common;
using System.Globalization;

namespace RecomeceAPI.Services.PDFs
{
  public class AssociativeFormPdf : IDocument
  {
    public string Nome { get; set; }
    public string Documento { get; set; }
    public string Cidade { get; set; }
    public DateTime DataAssinatura { get; set; }

    public AssociativeFormPdf(string nome, string documento,  DateTime data, string cidade)
    {
      Nome = nome;
      Documento = documento;
      DataAssinatura = data;
      Cidade = cidade;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
      string dataFormatada = DataAssinatura.ToString("dd 'de' MMMM 'de' yyyy", new CultureInfo("pt-BR"));

      container.Page(page =>
      {
        page.Margin(50);
        page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

        page.Content().Column(col =>
        {
          // Título
          col.Item()
             .AlignCenter()
             .PaddingBottom(40)
             .Text("FICHA DE INSCRIÇÃO ASSOCIATIVA")
             .FontSize(14)
             .Bold();

          // Nome
          col.Item().PaddingBottom(5).Text($"NOME: {Nome.ToUpper()}");
          col.Item().PaddingBottom(20).Text($"CPF/CNPJ: {Documento}");

          // Parágrafo 1
          col.Item().PaddingTop(20).AlignLeft().Text("Por meio deste documento estou ciente da inclusão do meu nome como ASSOCIADO E\r\nAUTORIZO esta entidade a representar-me judicialmente e conforme previsto no artigo 5o, inciso\r\nXXI, da Constituição Federal, propor ações em defesa dos meus direitos e garantias em ações\r\ncoletivas ou individual, em face dos órgãos de proteção ao crédito, podendo recorrer a quaisquer\r\ninstâncias e tribunais, apresentar defesa nas ações contrárias conexas, bem como, para desistir das\r\nações propostas e praticar quaisquer atos extrajudiciais perante empresas e órgãos privados, tais\r\ncomo requer e apresentar documentos.")
          .FontSize(11);
          // Parágrafo 2
          col.Item().PaddingTop(20).AlignLeft().Text("Em respeito ao que dispõe à lei 13.709/2018 LGPD com redação alterada pela lei 13.853/2019 o\r\nASSOCIADO declara expressamente por via deste documento que Autoriza por tempo\r\nindeterminado e de forma irretratável que esta entidade, seus representantes, colaboradores, seus\r\nprofissionais e empresas que vier a contratar em cadeia na prestação de seus serviços possam trocar\r\nentre si ou com terceiros, todas as informações pessoais do ASSOCIADO, inclusive os dados\r\nconsiderados pela lei como sensíveis, bem como podendo armazená-los, manipulá-los,\r\ncompartilha-los, formatálos, divulgá-los e exibi-los em meio físico ou eletrônico de forma interna\r\nou externamente. O ASSOCIADO poderá a qualquer tempo revogar esta autorização, contudo só\r\nalcançará situações após a referida notificação conservando, portanto, como válido e perfeito as\r\nsituações anteriores que se contará a partir da assinatura deste.")
          .FontSize(11);
          // Fechamento
          col.Item().PaddingTop(20).AlignLeft().Text("E, para maior clareza, firmo a presente em duas vias.").FontSize(11);

          // Local e data
          col.Item().AlignCenter().PaddingTop(60).PaddingBottom(90)
             .Text($"{Cidade}, {dataFormatada}");

          // Assinatura
          col.Item().AlignCenter().Element(container =>
              container
                  .Width(200)
                  .PaddingBottom(2)
                  .LineHorizontal(1)
          );

          col.Item().AlignCenter().Text("Assinatura").FontSize(10);
        });
      });
    }
  }
}
