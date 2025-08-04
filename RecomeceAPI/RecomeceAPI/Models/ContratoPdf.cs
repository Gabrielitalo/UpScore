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
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA PRIMEIRA – DO OBJETO").Justify().LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
        col.Item()
           .PaddingTop(10)
           .AlignLeft()
           .Text(text =>
           {
             text.Span("O presente contrato tem como objeto a prestação de serviços de consultoria e assessoria jurídica em favor da CONTRATANTE pela CONTRATADA, sendo esta última a responsável pela retirada das restrições que constam perante os órgãos de proteção ao crédito, restauração de score e acompanhamento de processo administrativo ou judicial em face dos órgãos de controle de crédito.")
             .FontSize(fontSize).LineHeight(lineHeightNormal);
           });

        // CLÁUSULA  2
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA SEGUNDA – DAS OBRIGAÇÕES DA CONTRATADA").LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
        col.Item().Element(GetParagraph("2.1", "A CONTRATADA compromete-se a prestar os serviços solicitados pela CONTRATANTE conforme descrito na Cláusula Primeira – Do Objeto.", fontSize));
        col.Item().Element(GetParagraph("2.2", "O prazo para efetiva execução do serviço do objeto deste contrato é de 30 (trinta) a 60 (sessenta) dias úteis, a contar do pagamento e da entrega da documentação necessária para a propositura da ação.", fontSize));
        col.Item().Element(GetParagraph("2.3", "Até o final do prazo, a CONTRATADA deverá entregar o NADA CONSTA dos birôs de consulta dos órgãos de proteção ao crédito à CONTRATANTE.", fontSize));
        col.Item().Element(GetParagraph("2.4", "Na prestação de serviços, a CONTRATADA deverá manter sigilo total de todas as informações fornecidas pela CONTRATANTE, utilizando-se delas unicamente para fins de cumprimento do objeto do presente contrato.", fontSize));
        col.Item().Element(GetParagraph("2.5", "Não há garantia de crédito, mas a CONTRATADA compromete-se a diligenciar para que aumentem as chances do êxito.", fontSize));
        col.Item().Element(GetParagraph("2.6", "Na hipótese da CONTRATANTE contrair novas dívidas em um período de 12 (doze) meses, inexistentes até o ato da assinatura do presente Contrato, será facultado o direito a uma nova execução do serviço objeto deste instrumento, mediante um novo pagamento equivalente a 50% (cinquenta por cento) do valor expresso na Cláusula Quarta.", fontSize));
        col.Item().Element(GetParagraph("2.7", "A CONTRATADA não garante pontuação mínima ou máxima na retomada do score.", fontSize));
        col.Item().Element(GetParagraph("2.8", "Vale ressaltar que dentro da ação não serão feitas negociações, quitações, compras ou parcelamentos das dívidas. O serviço é baseado nos art. 42 e 43 do Código de Defesa do Consumidor (Lei n. 8.078/90).", fontSize));
        col.Item().Element(GetParagraph("2.9", "Salienta-se que a dívida ainda aparecerá internamente dentro da instituição credora e de visualização interna (cliente) no aplicativo do Serasa e/ou outros.", fontSize));

        // CLÁUSULA  3
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA TERCEIRA – DAS OBRIGAÇÕES DA CONTRATANTE").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("3.1", "A CONTRATANTE se obriga a cumprir fielmente o pagamento dos honorários aqui acordados, sob pena de, em caso de mora, extinguir-se a relação contratual e ser levado o presente contrato à execução judicial."));
        col.Item().Element(GetParagraph("3.2", "A CONTRANTE desde já se declara ciente de que a ação em questão obedece a procedimento previsto no Código de Processo Civil, Código Civil e Código de Defesa do Consumidor, não possuindo a CONTRATADA, poder para abreviar a prestação jurisdicional."));
        col.Item().Element(GetParagraph("3.3", "A CONTRATANTE fornecerá à CONTRATADA os documentos e meios necessários à comprovação processual do seu pretendido direito, sob pena de exclusão da responsabilidade causídico, inclusive dentro dos prazos legais."));

        // CLÁUSULA  4
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA QUARTA – DO PAGAMENTO").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("4.1", $"Pelo serviço objeto do presente contrato, a CONTRATANTE deverá pagar à CONTRATADA a quantia de {ExtensionService.AplicarMascaraMoeda(MovContrato.MovPropostasModel.ValorAprovado)}, de forma irrevogável e irretratável, {GetPagamentoText()}"));
        col.Item().Element(GetParagraph("4.2", "O pagamento é devido pela CONTRATANTE em favor da CONTRATADA por ação protocolada, ou seja, cada ação gera uma obrigação de pagamento nos valores constantes na cláusula anterior."));
        col.Item().Element(GetParagraph("4.3", "O não cumprimento do pagamento fará a ação ser revogada, sem direito à devolução de qualquer quantia paga, incorrendo em multa contratual de 2% (dois por cento) sobre os valores devidos, atualização monetária pelo INPC e juros monetário de 1% ao mês."));

        // CLÁUSULA  5
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA QUINTA – DA RESCISÃO CONTRATUAL").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("5.1", "Em caso de desistência da ação por parte da CONTRATANTE, se a ação já estiver em andamento, não haverá devolução de qualquer quantia paga."));
        col.Item().Element(GetParagraph("5.2", "A parte que descumprir qualquer das cláusulas deste contrato dará à outra o direito de rescindir o presente instrumento, cientificando-a com aviso prévio de 15 (quinze) dias, ficando desobrigada a parte inocente a dar continuidade a este contrato."));

        // CLÁUSULA  6
        col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA SEXTA – DISPOSIÇÕES GERAIS").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
        col.Item().Element(GetParagraph("6.1", "A retomada de relacionamento com o mercado financeiro é de 45 (quarenta e cinco) dias após a entrega do NADA CONSTA, ressaltando que não há a garantia de crédito conforme a cláusula segunda (2.5)."));
        col.Item().Element(GetParagraph("6.2", "A CONTRATANTE se responsabiliza por toda ou quaisquer tentativas frustradas de retomada no mercado antes do prazo de 45 (quarenta e cinco) dias, ciente de que esse ato poderá prejudicar a pontuação do Score."));
        col.Item().Element(GetParagraph("6.3", "O principal PROPÓSITO da CONTRATADA ao prestar esse serviço é a restruturação da vida financeira da CONTRATANTE, para que esta tenha novos hábitos financeiros a fim de sair da inadimplência e se tornar uma boa consumidora. Sendo assim, a CONTRATANTE se compromete a ter uma boa conduta perante o mercado financeiro."));
        col.Item().Element(GetParagraph("6.4", "O presente contrato é um título executivo extrajudicial conforme previsão legal e, em caso de inadimplemento da CONTRATANTE, permite a propositura de ação de execução autônoma para o recebimento dos honorários devidos e não pagos."));
        col.Item().Element(GetParagraph("6.5", "Fica pactuada a total inexistência de vínculo trabalhista entre as partes, excluindo as obrigações previdenciárias e os encargos sociais, não havendo entre as partes qualquer tipo de relação de subordinação."));
        col.Item().Element(GetParagraph("6.6", "Este contrato, cumpridas todas as formalidades legais, afasta a qualidade de empregado prevista no art. 3º da CLT, nos termos do art. 442-B da CLT."));
        col.Item().Element(GetParagraph("6.7", "A tolerância, por qualquer das partes, com relação ao descumprimento de qualquer termo ou condição aqui ajustado, não será considerada como desistência em exigir o cumprimento de disposição nele contida, nem representará novação com relação à obrigação passada, presente ou futura, no tocante ao termo ou condição cujo descumprimento foi tolerado."));
        col.Item().Element(GetParagraph("6.8", "Fica eleito o foro do município de Ribeirão Preto, Estado de São Paulo, com exclusão de qualquer outro, por mais privilegiado que seja, para dirimir eventuais conflitos oriundos do presente contrato."));


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
