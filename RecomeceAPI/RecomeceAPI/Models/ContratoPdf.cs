using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecomeceAPI.Models;
using RecomeceAPI.Services.Common;
using System.Globalization;

public class ContratoPdf : IDocument
{
  public MovContratosModel MovContrato = new MovContratosModel();
  public CadEmpresasModel CadEmpresas = new CadEmpresasModel();
  public List<MovPropostasBeneficiariosModel> MovPropostasBeneficiarios = new List<MovPropostasBeneficiariosModel>();
  public List<MovPropostasDuplicatasModel> MovPropostasDuplicatas = new List<MovPropostasDuplicatasModel>();
  public string PagamentoStr { get; set; } = string.Empty;
  public int fontSize = 14;
  public float lineHeightParagrafo = (float)2.5;
  public float lineHeightNormal = (float)2;

  public string ContratadaStr()
  {
    return $"{ExtensionService.AplicarMascaraCpfCnpj(CadEmpresas.Inscricao)}, com escritório profissional no município de {CadEmpresas.Cidades.NomeCidade}, Estado de {CadEmpresas.Cidades.Estados.Nome}, à {CadEmpresas.Logradouro}, n. {CadEmpresas.Numero}, {CadEmpresas.Complemento}, CEP {CadEmpresas.Cep}";
  }
  public string ContratanteStr()
  {
    string tipo = MovContrato.MovPropostasModel.CadClientes.Inscricao.Length == 14 ? "CNPJ" : "CPF";
    return $"{MovContrato.MovPropostasModel.CadClientes.Profissao}, {MovContrato.MovPropostasModel.CadClientes.EstadoCivil}, inscrito(a) no {tipo} sob o n. {ExtensionService.AplicarMascaraCpfCnpj(MovContrato.MovPropostasModel.CadClientes.Inscricao)}, residente e domiciliado(a) no município de {MovContrato.MovPropostasModel.CadClientes.Cidades.NomeCidade}, no Estado de {MovContrato.MovPropostasModel.CadClientes.Cidades.Estados.Nome}, à {MovContrato.MovPropostasModel.CadClientes.Logradouro}, n. {MovContrato.MovPropostasModel.CadClientes.Numero}, CEP {MovContrato.MovPropostasModel.CadClientes.Cep}";
  }
  public string GetPagamentoText()
  {
    var entrada = MovPropostasDuplicatas.FirstOrDefault(d => d.Descricao == "Entrada");
    var parcela = MovPropostasDuplicatas.FirstOrDefault(d => d.Descricao != "Entrada");
    int parcelas = MovPropostasDuplicatas.Count;
    string entradaTxt = "", parcelasTxt = "";
    if (entrada?.MovPropostas.Id > 0)
    {
      entradaTxt = $"concordando com uma entrada de {ExtensionService.AplicarMascaraMoeda(entrada.Valor)} no ato da assinatura deste instrumento ";
      parcelas -= 1;
    }

    if (parcela?.MovPropostas.Id > 0)
    {
      parcelasTxt = $"e o restante em {parcelas} parcelas mensais, iguais e sucessivas de {ExtensionService.AplicarMascaraMoeda(parcela.Valor)} nos meses subsequentes.";
      parcelas -= 1;
    }

    return $"{entradaTxt}{parcelasTxt}";
  }
  public string GetLocalData()
  {
    var dt = DateTime.Now;
    var mes = dt.ToString("MMMM", new CultureInfo("pt-BR"));
    string local = $"{CadEmpresas.Cidades.NomeCidade} - {CadEmpresas.Cidades.Estados.UF}, {dt.Day} de {mes} de {dt.Year}";
    return local;
  }
  public byte[] GetLogo()
  {
    string path = Path.Combine(Directory.GetCurrentDirectory(), "Docs", "logoContrato.png");
    return File.ReadAllBytes(path);
  }
  public string GetNewProcessValue()
  {
    decimal valorTotal = 250 * MovPropostasBeneficiarios.Count;
    return $"{ExtensionService.AplicarMascaraMoeda(valorTotal)}";
  }
  public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

  public void Compose(IDocumentContainer container)
  {
    container.Page(page =>
    {
      page.Margin(50);
      page.DefaultTextStyle(x => x.FontSize(fontSize).FontFamily("Calibri"));

      // Adiciona a logo no topo direito
      page.Header().AlignRight().Height(110).Width(110).Image(GetLogo());

      page.Footer()
          .AlignCenter()
          .Text("Up Score © Todos os direitos reservados.")
          .FontSize(10)
          .Italic()
          .FontColor(Colors.Grey.Lighten2);

      page.Content().Column(col =>
      {
        // Título
        col.Item()
           .AlignCenter()
           .PaddingBottom(40)
           .Text("Contrato de Prestação de Serviços")
           .Justify()
           .FontSize(18)
           .Bold();


        // Contratada
        col.Item()
           .PaddingTop(10)
           .AlignLeft()
           .Text(text =>
           {
             text.Span(CadEmpresas.Nome).Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
             text.Span($", pessoa jurídica de direito privado, inscrita no CNPJ sob o n. {ContratadaStr()}, doravante denominada ").FontSize(fontSize).LineHeight(lineHeightNormal);
             text.Span("CONTRATADA").Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
             text.Span("; e").FontSize(fontSize).LineHeight(lineHeightNormal);
           });

        // Contratante
        col.Item()
           .PaddingTop(10)
           .AlignLeft()
           .Text(text =>
           {
             text.Span(MovContrato.MovPropostasModel.CadClientes.Nome).Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
             text.Span($", brasileiro(a),  {ContratanteStr()}, doravante denominada ").FontSize(fontSize).LineHeight(lineHeightNormal);
             text.Span("CONTRATANTE;").Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
           });

        // Partes
        col.Item()
           .PaddingTop(10)
           .AlignLeft()
           .Text(text =>
           {
             text.Span("Decidem as partes, na melhor forma de direito, celebrar o presente CONTRATO DE PRESTAÇÃO DE SERVIÇOS, que reger-se-á mediante às cláusulas e condições adiante estipuladas.").FontSize(fontSize).LineHeight(lineHeightNormal);
           });

        // CLÁUSULA  1
        col.Item().PaddingTop(20).AlignLeft().Text("I – Do objeto").Justify().LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
        col.Item().Element(GetParagraph("1.1", "O presente contrato tem como objeto o ajuizamento de PEDIDO DE URGÊNCIA pela Contratada, sob a intenção de baixar restrições da Contratante em órgãos de proteção ao crédito", fontSize));
  

        // CLÁUSULA  2
        col.Item().PaddingTop(20).AlignLeft().Text("II – Do pagamento").LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
        col.Item().Element(GetParagraph("2.1", $"Pelo serviço objeto do presente contrato, a CONTRATANTE deverá pagar à CONTRATADA a quantia total de {ExtensionService.AplicarMascaraMoeda(MovContrato.MovPropostasModel.ValorAprovado)}, de forma irrevogável e irretratável, {GetPagamentoText()}"));
        col.Item().Element(GetParagraph("2.2", " O não cumprimento do pagamento fará o serviço objeto deste contrato ser revogado, sem direito à devolução de qualquer quantia, incorrendo ainda em multa contratual de 10% (dez por cento) sobre os valores devidos, atualização monetária e juro de mora de 1% (um por cento) ao mês", fontSize));
    
        // CLÁUSULA  3
        col.Item().PaddingTop(20).AlignLeft().Text("III – Das obrigações da Contratada").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("3.1", "A Contratada compromete-se a prestar fielmente os serviços objeto do presente contrato"));
        col.Item().Element(GetParagraph("3.2", "O prazo para execução do serviço objeto deste contrato é de até 60 (sessenta) dias úteis, a contar do ato de assinatura deste instrumento"));
        col.Item().Element(GetParagraph("3.3", "Até o final do prazo acima fixado, a Contratada entregará o relatório de nada consta para restrições em órgãos de proteção ao crédito à Contratante"));
        col.Item().Element(GetParagraph("3.4", "A Contratada manterá sigilo total de todas as informações fornecidas pela Contratante, nutilizando-se delas única e exclusivamente para o bom e fiel cumprimento do objeto do presente contrato."));
        col.Item().Element(GetParagraph("3.5", "Os efeitos do serviço objeto deste contrato não se estendem a qualquer futura restrição que a Contratante venha a incorrer perante órgãos de proteção ao crédito, apenas às existentes no ato da contratação do serviço"));
        col.Item().Element(GetParagraph("3.6", "As obrigações da Contratada limitam-se à execução do serviço objeto do presente contrato, conforme o ordenamento jurídico vigente na data de celebração deste instrumento, caracterizando-se como obrigação de meio e não de fim (resultado)."));

        // CLÁUSULA  4
        col.Item().PaddingTop(20).AlignLeft().Text("IV – Das obrigações da Contratante").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("4.1", "A Contratante se obriga a cumprir fielmente o pagamento da quantia aqui acordada, sob pena de, em caso de mora, extinguir-se a relação contratual e ser levado o presente contrato à execução judicial, estando sujeita à correção monetária, juros e multa"));
        col.Item().Element(GetParagraph("4.2", "A CONTRANTE declara estar ciente de que o serviço objeto deste contrato obedece estritamente a legislação brasileira"));
        col.Item().Element(GetParagraph("4.3", "A Contratante fornecerá todos os documentos e meios necessários para a plena execução do serviço objeto deste contrato, sob pena de isentar a Contratada de responsabilidades acerca da prestação do serviço nos termos aqui acordados"));

        // CLÁUSULA  5
        col.Item().PaddingTop(20).AlignLeft().Text("V – Da manutenção processual").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("5.1", $"Em caso de indeferimento, revogação, anulação ou extinção da medida judicial objeto do presente contrato, a Contratante, querendo, poderá requerer à Contratada, mantendo os demais termos desse instrumento, a manutenção processual da ação mediante pagamento antecipado de nova quantia sob o valor de {GetNewProcessValue()}."));

        // CLÁUSULA  6
        col.Item().PaddingTop(20).AlignLeft().Text("VI – Da rescisão contratual").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("6.1", "Em caso de rescisão do presente contrato por desistência ou inadimplência da Contratante, a Contratada ficará desobrigada de continuar a prestação de serviço, ficando ainda a Contratante responsável por eventuais pagamentos pendentes."));
        col.Item().Element(GetParagraph("6.1.2", "Em caso de rescisão deste instrumento por desistência ou inadimplência da Contratante, caso esta deseje retomar o serviço em data posterior, deverá celebrar um novo contrato com a Contratada sob as condições vigentes à época da nova adesão, mediante novo pagamento"));
        col.Item().Element(GetParagraph("6.2", "O atraso superior a 30 (trinta) dias corridos em qualquer parcela caracterizará rescisão automática deste contrato, independentemente de aviso prévio, sem prejuízo às responsabilidades com eventuais pagamentos pendentes"));

        // CLÁUSULA  7
        col.Item().PaddingTop(20).AlignLeft().Text("VII – Das disposições gerais").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("7.1", "A Contratada não realiza negociações, quitações, compras ou parcelamentos de dívidas de consumidores."));
        col.Item().Element(GetParagraph("7.2", "Restrições ainda podem aparecer internamente aos credores e em visualização própria (consumidor) nos aplicativos de órgãos de proteção ao crédito."));
        col.Item().Element(GetParagraph("7.3", "A Contratada não garante pontuação de score (pontuação de crédito) com a prestação do serviço objeto deste contrato."));
        col.Item().Element(GetParagraph("7.4", "A manutenção de atos processuais e suporte jurídico estão condicionados ao pagamento das quantias acordadas neste instrumento."));
        col.Item().Element(GetParagraph("7.5", "O principal propósito da Contratada ao prestar o serviço objeto deste contrato é a restruturação da vida financeira da Contratante e, para que esta última tenha novos hábitos financeiros, a fim de sair da inadimplência. Sendo assim, a Contratante se compromete a ter uma boa conduta perante o mercado financeiro."));
        col.Item().Element(GetParagraph("7.6", "As partes reconhecem que a prestação dos serviços objeto deste contrato é baseada na legislação e entendimento jurisprudencial vigentes na data de sua assinatura."));
        col.Item().Element(GetParagraph("7.7", "Caso haja alteração no Código de Defesa do Consumidor (CDC), em normas correlatas ou em decisões judiciais que modifiquem ou inviabilizem a tese jurídica utilizada, a Contratada não será responsável por eventual perda de eficácia de efeitos processuais, como extinção ou revogação, não sendo devida a devolução de valores já pagos."));
        col.Item().Element(GetParagraph("7.8", "O presente contrato é um título executivo extrajudicial conforme previsão legal e, em caso de inadimplemento da Contratante, permite a propositura de ação de execução autônoma para o recebimento dos honorários devidos e não pagos."));
        col.Item().Element(GetParagraph("7.9", "Fica pactuada a total inexistência de vínculo trabalhista entre as partes, excluindo as obrigações previdenciárias e os encargos sociais, não havendo entre as partes qualquer tipo de relação de subordinação."));
        col.Item().Element(GetParagraph("7.10", "Este contrato, cumpridas todas as formalidades legais, afasta a qualidade de empregado prevista no art. 3º da CLT, nos termos do art. 442-B da CLT"));
        col.Item().Element(GetParagraph("7.11", "A tolerância, por qualquer das partes, com relação ao descumprimento de qualquer termo ou condição aqui ajustado, não será considerada como desistência em exigir o cumprimento de disposição nele contida, nem representará novação com relação à obrigação\r\npassada, presente ou futura, no tocante ao termo ou condição cujo descumprimento foi tolerado"));
        col.Item().Element(GetParagraph("7.12", "Fica eleito o foro do município de Ribeirão Preto, Estado de São Paulo, com exclusão de qualquer outro, por mais privilegiado que seja, para dirimir eventuais conflitos oriundos do presente contrato"));


        col.Item().PaddingTop(20).AlignCenter().Text("E, por assim estarem as partes justas e acordadas, assinam o presente contrato.").FontSize(fontSize);
        col.Item().PaddingTop(5).AlignCenter().Text(GetLocalData()).FontSize(fontSize);

        // Assinatura Contratante
        col.Item().PaddingTop(80).AlignCenter().Element(container =>
            container
                .Width(280)
                .PaddingBottom(2)
                .LineHorizontal(1)
        );

        col.Item().AlignCenter().Text(MovContrato.MovPropostasModel.CadClientes.Nome).FontSize(fontSize).Bold();
        col.Item().AlignCenter().Text("(CONTRATANTE)").FontSize(fontSize);

        // Assinatura Contratada
        col.Item().PaddingTop(80).AlignCenter().Element(container =>
            container
                .Width(280)
                .PaddingBottom(2)
                .LineHorizontal(1)
        );

        col.Item().AlignCenter().Text("UPSCORE BRASIL RECUPERAÇÃO DE CRÉDITO LTDA").FontSize(fontSize).Bold();
        col.Item().AlignCenter().Text("(CONTRATADA)").FontSize(fontSize);
      });
    });
  }

  public Action<IContainer> GetParagraph(string numero, string texto, float fontSize = 14)
  {
    return container => container
        .PaddingTop(10)
        .AlignLeft()
        .Text(text =>
        {
          text.Span($"{numero} - ").Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
          text.Span(texto).FontSize(fontSize).LineHeight(lineHeightNormal);
        });
  }
}
