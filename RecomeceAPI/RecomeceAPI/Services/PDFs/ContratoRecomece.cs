using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using RecomeceAPI.Models;
using RecomeceAPI.Services.Common;
using System.Globalization;

namespace RecomeceAPI.Services.PDFs
{
  public class ContratoRecomece : QuestPDF.Infrastructure.IDocument
  {
    public MovContratosModel MovContrato = new MovContratosModel();
    public CadEmpresasModel CadEmpresas = new CadEmpresasModel();
    public List<MovPropostasBeneficiariosModel> MovPropostasBeneficiarios = new List<MovPropostasBeneficiariosModel>();
    public List<MovPropostasDuplicatasModel> MovPropostasDuplicatas = new List<MovPropostasDuplicatasModel>();
    public string PagamentoStr { get; set; } = string.Empty;
    public int fontSize = 14;
    public float lineHeightParagrafo = (float)2.5;
    public float lineHeightNormal = (float)2;
    public string[] condicaoPagamento = ["Boleto", "Cartão de Crédito", "Pix"];

    public string ContratadaStr()
    {
      return $"{CadEmpresas.NomeResponsavel}, Brasileiro(a), inscrito no CPF sob o nº {ExtensionService.AplicarMascaraCpfCnpj(CadEmpresas.CpfResponsavel)}, com endereço comercia na {CadEmpresas.Logradouro}, {CadEmpresas.Numero}, {CadEmpresas.Complemento}, {CadEmpresas.Bairro}, {CadEmpresas.Cidades.NomeCidade} - {CadEmpresas.Cidades.Estados.UF}, {CadEmpresas.Cep}";
    }
    public string ContratanteStr()
    {
      string tipo = MovContrato.MovPropostasModel.CadClientes.Inscricao.Length == 14 ? "CNPJ" : "CPF";
      return $"Brasileiro(a), {MovContrato.MovPropostasModel.CadClientes.EstadoCivil}, {MovContrato.MovPropostasModel.CadClientes.Profissao}, inscrito no {tipo} sob o nº {ExtensionService.AplicarMascaraCpfCnpj(MovContrato.MovPropostasModel.CadClientes.Inscricao)}, com endereço na {MovContrato.MovPropostasModel.CadClientes.Logradouro}, {MovContrato.MovPropostasModel.CadClientes.Numero}, {MovContrato.MovPropostasModel.CadClientes.Bairro}, {MovContrato.MovPropostasModel.CadClientes.Cidades.NomeCidade} - {MovContrato.MovPropostasModel.CadClientes.Cidades.Estados.UF}, CEP {MovContrato.MovPropostasModel.CadClientes.Cep}";
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
            .Text("Recomece Brasil © Todos os direitos reservados.")
            .FontSize(10)
            .Italic()
            .FontColor(Colors.Grey.Lighten2);

        page.Content().Column(col =>
        {
          // Título
          col.Item()
             .AlignCenter()
             .PaddingBottom(40)
             .Text("CONTRATO DE CONSULTORIA E ASSESSORIA FINANCEIRA\r\n")
             .Justify()
             .FontSize(18)
          .Bold();

          // Contratante
          col.Item()
             .PaddingTop(10)
             .AlignLeft()
             .Text(text =>
             {
               text.Span("CONTRATANTE: ").Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
               text.Span(MovContrato.MovPropostasModel.CadClientes.Nome).Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
               text.Span($", brasileiro(a),  {ContratanteStr()}").FontSize(fontSize).LineHeight(lineHeightNormal);
             });

          // Contratada
          col.Item()
             .PaddingTop(10)
             .AlignLeft()
             .Text(text =>
             {
               text.Span("CONTRATADA: ").Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
               text.Span(CadEmpresas.Nome).Bold().FontSize(fontSize).LineHeight(lineHeightNormal);
               text.Span($", inscrita no CNPJ sob o nº. {ExtensionService.AplicarMascaraCpfCnpj(CadEmpresas.Inscricao)}, neste ato representado por {ContratadaStr()}").FontSize(fontSize).LineHeight(lineHeightNormal);
               text.Span("").FontSize(fontSize).LineHeight(lineHeightNormal);
             });

          // CLÁUSULA  1
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA PRIMEIRA – DO OBJETO E SERVIÇOS A SEREM PRESTADOS").Justify().LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
          col.Item().Element(GetParagraph("1.1", "O objeto deste contrato é a prestação de serviços de consultoria financeira especializada, com foco na orientação e intermediação de processos coletivos de consumidores, realizados por meio de uma associação de consumidores, com o objetivo de remover restrições de crédito junto aos principais órgãos de proteção ao crédito, a saber: Serasa, SPC e Boa Vista", fontSize));
          col.Item().Element(GetParagraph("1.2", "O CONTRATANTE, ao assinar este contrato, concorda em também se associar à associação de consumidores indicada pela CONTRATADA, que será responsável por conduzir as ações necessárias para a obtenção de liminares e outras medidas judiciais em favor do CONTRATANTE.", fontSize));
          col.Item().Element(GetParagraph("1.3", "A CONTRATADA não garante resultados específicos, visto que o sucesso de ações judiciais depende de fatores externos e alheios ao controle da CONTRATADA e da associação de consumidores, como decisões judiciais e interpretações legais. Contudo, a CONTRATADA compromete-se a intermediar e acompanhar o processo para buscar o melhor resultado possível para o CONTRATANTE", fontSize));
          col.Item().Element(GetParagraphLine("Parágrafo único: A CONTRATADA atua exclusivamente como uma consultoria financeira e não presta serviços jurídicos. As ações judiciais são conduzidas pela associação de consumidores à qual o CONTRATANTE se associa como parte deste contrato. ", fontSize));

          // CLÁUSULA  2
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA SEGUNDA – DAS OBRIGAÇÕES DA CONTRATADA").LineHeight(lineHeightParagrafo).FontSize(fontSize).Bold();
          col.Item().Element(GetParagraph("2.1", "O presente contrato pode beneficiar, além do CONTRATANTE, as seguintes pessoas físicas e/ou jurídicas (doravante denominados \"BENEFICIÁRIOS\"), cujos dados serão utilizados para a execução dos serviços descritos na Cláusula Primeira: ", fontSize));

          col.Item().Table(table =>
          {
            table.ColumnsDefinition(columns =>
            {
              columns.RelativeColumn(2); // Nome do Beneficiário
              columns.RelativeColumn(1); // CPF/CNPJ
            });

            // Cabeçalho
            table.Header(header =>
            {
              header.Cell().Element(CellStyle).Text("Nome do Beneficiário").Bold();
              header.Cell().Element(CellStyle).Text("CPF/CNPJ").Bold();
            });

            foreach(var beneficiario in MovPropostasBeneficiarios)
            {
              table.Cell().Element(CellStyle).Text(beneficiario.CadClientes.Nome);
              table.Cell().Element(CellStyle).Text(beneficiario.CadClientes.Inscricao);
            }
          });

          col.Item().Element(GetParagraph("2.2", "O CONTRATANTE declara estar autorizado a incluir os BENEFICIÁRIOS listados acima neste contrato, sendo responsável por quaisquer informações fornecidas e por garantir que os BENEFICIÁRIOS estejam cientes e de acordo com os termos e condições deste contrato.", fontSize));
          col.Item().Element(GetParagraph("2.3", "O CONTRATANTE permanece integralmente responsável pelo pagamento dos valores devidos à CONTRATADA, independentemente da quantidade de BENEFICIÁRIOS incluídos neste contrato. ", fontSize));
          col.Item().Element(GetParagraph("2.4", "A CONTRATADA prestará os serviços de acordo com as condições previstas neste contrato para todos os BENEFICIÁRIOS listados, sendo que a inadimplência ou descumprimento por parte do CONTRATANTE poderá resultar na suspensão ou cancelamento dos serviços para todos os BENEFICIÁRIOS. ", fontSize));

          // CLÁUSULA  3
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA TERCEIRA – DO ORÇAMENTO").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraph("3.1", "Pelos serviços prestados conforme o objeto deste contrato, o CONTRATANTE pagará à CONTRATADA o valor de R$ 1.000,00 ( um mil reais).", fontSize));
          col.Item().Element(GetParagraph("3.2", "As condições de pagamento são detalhadas abaixo:", fontSize));

          col.Item().Table(table =>
          {
            table.ColumnsDefinition(columns =>
            {
              columns.RelativeColumn(1); // Parcela
              columns.RelativeColumn(2); // Forma de Pagamento
              columns.RelativeColumn(2); // Data
              columns.RelativeColumn(2); // Valor
            });

            // Cabeçalho
            table.Header(header =>
            {
              header.Cell().Element(CellStyle).Text("Parcela").Bold();
              header.Cell().Element(CellStyle).Text("Forma de Pagamento").Bold();
              header.Cell().Element(CellStyle).Text("Data").Bold();
              header.Cell().Element(CellStyle).Text("Valor").Bold();
            });

            foreach (var dup in MovPropostasDuplicatas)
            {
              table.Cell().Element(CellStyle).Text(dup.Indice.ToString());
              table.Cell().Element(CellStyle).Text(condicaoPagamento[dup.CondPagamento - 1]);
              table.Cell().Element(CellStyle).Text(dup.DataVencimento.ToString("dd/MM/yyyy"));
              table.Cell().Element(CellStyle).Text(ExtensionService.AplicarMascaraMoeda(dup.Valor));
            }
          });
          col.Item().Element(GetParagraphLine("Parágrafo primeiro: Todas as quantias deverão ser quitadas através de boleto bancário, cartão de crédito ou depósito em conta de titularidade da CONTRATADA, cujo custo ocorrerá por conta do CONTRATANTE.", fontSize));
          col.Item().Element(GetParagraphLine("Parágrafo segundo: No caso de eventual atraso no pagamento, a CONTRATADA poderá aplicar juros de mora de 1% (um por cento) ao mês ou fração e correção monetária, calculada pro rata die, de acordo com a variação do IGP-M publicado pela Fundação Getúlio Vargas, além da aplicação de multa moratória de 2% (dois por cento) sobre o valor do débito em atraso.", fontSize));


          // CLÁUSULA  4
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA QUARTA – DAS OBRIGAÇÕES DO CONTRATANTE").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("4.1. O CONTRATANTE compromete-se a: a) Fornecer todas as informações e documentos pertinentes aos serviços a serem executados, solicitados tanto pelos órgãos judiciais e/ou administrativos quanto pela CONTRATADA; b) Assinar a ficha associativa necessária para a inclusão no processo coletivo conduzido pela associação de consumidores indicada pela CONTRATADA; c) Dar ciência à CONTRATADA dos comunicados, ofícios ou qualquer informação referente aos serviços remetidos pelos órgãos envolvidos; d) Pagar o valor referente aos serviços a serem executados; e) Cumprir rigorosamente a legislação vigente no âmbito Federal, Estadual e Municipal. ", fontSize));

          // CLÁUSULA  5
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA QUINTA – DAS OBRIGAÇÕES DA CONTRATADA").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraph("5.1", "A CONTRATADA deverá: ", fontSize));
          col.Item().Element(GetParagraph("a)", "Informar ao CONTRATANTE sobre os anexos, ofícios, documentos ou qualquer informação pertinente aos serviços contratados; ", fontSize));
          col.Item().Element(GetParagraph("b)", "Apresentar os documentos entregues pelo CONTRATANTE referentes aos serviços prestados logo após a solicitação dos órgãos competentes; ", fontSize));
          col.Item().Element(GetParagraph("c)", "Fornecer ao CONTRATANTE o comprovante de pagamento dos serviços prestados;", fontSize));
          col.Item().Element(GetParagraph("d)", "Zelar pelos serviços prestados e executá-los no prazo determinado.", fontSize));
          col.Item().Element(GetParagraphLine("5.1. A CONTRATADA deverá: a) Informar ao CONTRATANTE sobre os anexos, ofícios, documentos ou qualquer informação pertinente aos serviços contratados; b) Apresentar os documentos entregues pelo CONTRATANTE referentes aos serviços prestados logo após a solicitação dos órgãos competentes; c) Fornecer ao CONTRATANTE o comprovante de pagamento dos serviços prestados; d) Zelar pelos serviços prestados e executá-los no prazo determinado. ", fontSize));

          // CLÁUSULA  6
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA SEXTA – DAS ISENÇÕES DA CONTRATADA ").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("6.1. Fica a CONTRATADA isenta de responder: a) Pela perda ou extravio de qualquer documento que esteja na posse do CONTRATANTE ou de terceiros; b) Pela mora na emissão dos documentos necessários para prestação dos serviços contratados pelo CONTRATANTE; c) Por qualquer prejuízo acarretado ao CONTRATANTE devido ao não pagamento dos valores acordados neste contrato; d) Por qualquer prejuízo acarretado devido ao não cumprimento de prazo estabelecido pela CONTRATADA; e) Por qualquer circunstância alheia à vontade da CONTRATADA. ", fontSize));

          // CLÁUSULA  7
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA SÉTIMA – DA RESCISÃO\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("7.1. O contrato poderá ser rescindido por qualquer uma das partes, desde que mediante aviso prévio com antecedência mínima de 30 (trinta) dias, conforme as seguintes condições:\r\n7.2. Rescisão pelo CONTRATANTE Antes da Concessão da Liminar:\r\na) Caso o CONTRATANTE deseje rescindir o contrato antes da concessão da liminar, a rescisão estará sujeita ao\r\npagamento de uma multa compensatória correspondente a 50% (cinquenta por cento) do valor total do contrato, em razão\r\ndos custos já incorridos pela CONTRATADA na intermediação do processo e preparação dos documentos.\r\nb) A CONTRATADA reterá todos os valores pagos até o momento da rescisão, e caso o montante pago seja inferior à multa\r\ncompensatória prevista, o CONTRATANTE deverá efetuar o pagamento da diferença no prazo de 10 (dez) dias úteis a\r\ncontar da notificação da rescisão.\r\n7.3. Rescisão pelo CONTRATANTE Após a Concessão da Liminar:\r\na) Uma vez que a liminar tenha sido concedida pelo juiz, o contrato não poderá mais ser rescindido pelo CONTRATANTE,\r\ne este permanecerá obrigado a efetuar todos os pagamentos previstos no contrato até o término do prazo de vigência.\r\nb) Em caso de inadimplência por parte do CONTRATANTE após a concessão da liminar, a CONTRATADA poderá tomar\r\nas medidas legais cabíveis para a cobrança dos valores devidos, conforme previsto na Cláusula Terceira deste contrato.\r\n7.4. Rescisão pela CONTRATADA:\r\na) A CONTRATADA poderá rescindir o contrato a qualquer momento em caso de inadimplência do CONTRATANTE ou\r\ndescumprimento das obrigações contratuais, sem prejuízo do direito de cobrar os valores devidos até o momento da rescisão,\r\nbem como a aplicação das penalidades previstas neste contrato.\r\nb) Em caso de rescisão por parte da CONTRATADA, serão aplicadas as mesmas condições de multa previstas na Cláusula\r\n7.2.\r\n7.5. Multa por Rescisão Indevida:\r\na) Se o CONTRATANTE rescindir o contrato após a concessão da liminar ou sem cumprir as condições previstas nesta\r\ncláusula, será aplicada uma multa equivalente a 50% (cinquenta por cento) do valor total do contrato, a título de\r\ncompensação pelos prejuízos causados à CONTRATADA.\r\nCLÁUSULA OITAV", fontSize));
         
          // CLÁUSULA  8
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA OITAVA – DOS PRAZOS\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("8.1. A CONTRATADA terá o prazo de 60 (sessenta) dias corridos, a contar da confirmação do pagamento, para intermediar\r\n\r\ne concluir os serviços contratados.\r\nParágrafo primeiro: Caso o prazo acima mencionado não seja cumprido por responsabilidade da CONTRATADA, o\r\nCONTRATANTE terá a opção de solicitar o CANCELAMENTO dos serviços contratados, bem como a devolução do valor\r\npago, acrescidos de correção por índices oficiais.\r\n", fontSize));

          // CLÁUSULA  9
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA NONA – DA VIGÊNCIA DO CONTRATO\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("9.1. Este contrato tem validade de 12 (doze) meses, a contar da data de sua assinatura.\r\n\r\nParágrafo primeiro: A vigência do contrato não será prorrogada em caso de revogação ou suspensão de liminar obtida\r\ndurante o período do contrato.\r\n", fontSize));

          // CLÁUSULA  10
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA DÉCIMA – DA REEMISSÃO DE LIMINAR E CONDIÇÕES\r\nESPECIAIS\r\n\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("10.1. Caso a liminar obtida seja revogada ou suspensa durante o período de vigência deste contrato, a CONTRATADA\r\nenvidará seus melhores esforços para intermediar a obtenção de uma nova liminar, sem custos adicionais para o\r\nCONTRATANTE.\r\n10.2. A vigência do contrato não será prorrogada ou estendida em virtude de revogação ou suspensão da liminar. O período\r\nde vigência do contrato é contado a partir da data de assinatura, independentemente de quaisquer eventos que possam\r\nocorrer durante esse período.\r\n10.3. A CONTRATADA terá o prazo de até 60 (sessenta) dias corridos, a partir da notificação da queda da liminar, para\r\nintermediar a reemissão do processo ou adotar as medidas judiciais cabíveis para tentar obter uma nova liminar.\r\n10.4. A CONTRATADA não será responsável por qualquer prorrogação do período de vigência do contrato em função de\r\neventuais períodos em que o nome do CONTRATANTE possa ficar sem a proteção da liminar, decorrentes de fatores\r\nalheios ao controle da CONTRATADA ou da associação de consumidores.", fontSize));

          // CLÁUSULA  11
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA DÉCIMA PRIMEIRA – DA NEGATIVAÇÃO DO\r\nCONTRATANTE E DOS PROCEDIMENTOS\r\n\r\nESPECIAIS\r\n\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("11.1. O CONTRATANTE reconhece que, ao firmar este contrato, está ciente de que a CONTRATADA pode, em caso de\r\ninadimplência, inscrever o nome do CONTRATANTE nos órgãos de proteção ao crédito, como Serasa, SPC e Boa Vista,\r\nem razão de débitos decorrentes deste contrato.\r\n11.2. Ao firmar este contrato, o CONTRATANTE expressamente autoriza a CONTRATADA a proceder com a inscrição de\r\nseu nome nos órgãos de proteção ao crédito mencionados, caso ocorra inadimplência, sem a necessidade de envio de\r\nnotificação prévia por meio de Aviso de Recebimento (AR).\r\n11.3. O CONTRATANTE, ao firmar este contrato, declara estar plenamente ciente das disposições do Código de Defesa do\r\nConsumidor relativas à notificação prévia, e, por meio deste contrato, reconhece que já recebeu ciência adequada sobre a\r\npossibilidade de negativação em caso de inadimplência, dispensando, assim, a necessidade de notificação formal adicional\r\npor parte da CONTRATADA.\r\n11.4. A CONTRATADA compromete-se a seguir todos os procedimentos legais aplicáveis, exceto a exigência de\r\nnotificação prévia por AR, a qual o CONTRATANTE, por este instrumento, expressamente renuncia, autorizando a\r\nCONTRATADA a proceder diretamente à inscrição nos cadastros de inadimplência em caso de descumprimento das\r\nobrigações financeiras estabelecidas neste contrato.", fontSize));

          // CLÁUSULA  12
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA DÉCIMA SEGUNDA – DAS DISPOSIÇÕES GERAIS").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);
          col.Item().Element(GetParagraphLine("12.1. As Partes obrigam-se a manter em sigilo o conteúdo do presente instrumento, não devendo divulgá-los a terceiros.\r\n12.2. As Partes declaram que são independentes entre si, não podendo este contrato ser interpretado como qualquer relação\r\nempregatícia, de representação, joint-venture, incorporação, sociedade ou consórcio entre as partes. Nenhuma das Partes tem\r\nqualquer direito, poder ou autoridade para assumir qualquer obrigação ou responsabilidade em nome da outra.\r\n12.3. A CONTRATADA não poderá transferir a terceiros, total ou parcialmente, os direitos e obrigações contraídos neste\r\ncontrato sem a expressa autorização por escrito do CONTRATANTE.", fontSize));

          // CLÁUSULA  13
          col.Item().PaddingTop(20).AlignLeft().Text("CLÁUSULA DÉCIMA TERCEIRA – DO FORO\r\n").LineHeight(lineHeightParagrafo).Bold().FontSize(fontSize);

          col.Item().Element(GetParagraphLine($"13.1. Fica eleito o Foro da Comarca de {CadEmpresas.Cidades.NomeCidade}/{CadEmpresas.Cidades.Estados.UF} como único competente para todas as ações relativas a este contrato e\r\ndecorrentes do mesmo.\r\nE, por estarem assim justos e contratados, firmam o presente instrumento, em 02 (duas) vias, do mesmo teor e forma, e para\r\num mesmo efeito.", fontSize));
          col.Item().Element(GetParagraphLine("", fontSize));
          col.Item().Element(GetParagraphLine("", fontSize));



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

        col.Item().AlignCenter().Text(CadEmpresas.Nome).FontSize(fontSize).Bold();
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
    public Action<IContainer> GetParagraphLine(string texto, float fontSize = 14)
    {
      return container => container
          .PaddingTop(10)
          .AlignLeft()
          .Text(text =>
          {
            text.Span(texto).FontSize(fontSize).LineHeight(lineHeightNormal);
          });
    }
    static IContainer CellStyle(IContainer container)
    {
      return container
          .Border(1)
          .Padding(5)
          .AlignLeft()
          .AlignMiddle();
    }
  }
}
