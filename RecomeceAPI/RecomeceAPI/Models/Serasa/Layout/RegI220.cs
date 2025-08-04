using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI220
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public PefinResumo PefinResumo { get; set; } // 00
    public List<PefinDetalhamento> PefinDetalhamento = new List<PefinDetalhamento> { };

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      PefinDetalhamento pefinDetalhamento = new PefinDetalhamento();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          PefinResumo = new PefinResumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          // Adiciona o objeto atual à lista antes de reiniciar
          if (pefinDetalhamento.PefinDetail != null ||
              pefinDetalhamento.PefinDetail2 != null ||
              pefinDetalhamento.PefinCredorDetail != null ||
              pefinDetalhamento.PefinSubjudiceDetail != null)
          {
            PefinDetalhamento.Add(pefinDetalhamento);
          }

          // Cria um novo objeto para o próximo conjunto de detalhes
          pefinDetalhamento = new PefinDetalhamento
          {
            PefinDetail = new PefinDetail(r.Texto)
          };
          TotalConsolidado += pefinDetalhamento.PefinDetail.Valor;

        }
        else if (r.Subtipo == "02")
        {
          pefinDetalhamento.PefinDetail2 = new PefinDetail2(r.Texto);
        }
        else if (r.Subtipo == "03")
        {
          pefinDetalhamento.PefinCredorDetail = new PefinCredorDetail(r.Texto);
        }
        else if (r.Subtipo == "04")
        {
          pefinDetalhamento.PefinSubjudiceDetail = new PefinSubjudiceDetail(r.Texto);
        }
      }

      // Adiciona o último objeto à lista, se houver dados
      if (pefinDetalhamento.PefinDetail != null ||
          pefinDetalhamento.PefinDetail2 != null ||
          pefinDetalhamento.PefinCredorDetail != null ||
          pefinDetalhamento.PefinSubjudiceDetail != null)
      {
        PefinDetalhamento.Add(pefinDetalhamento);
      }

      System.Diagnostics.Debug.WriteLine("Pefin acabou...");
    }

  }

  public class PefinDetalhamento
  {
    public PefinDetail PefinDetail { get; set; } // 01
    public PefinDetail2 PefinDetail2 { get; set; } // 02
    public PefinCredorDetail PefinCredorDetail { get; set; } // 03
    public PefinSubjudiceDetail PefinSubjudiceDetail { get; set; } // 04
  }
  public class PefinResumo
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro
    public string DataInicial { get; set; } // Data inicial (AAAAMMDD)
    public string DataFinal { get; set; } // Data final (AAAAMMDD)
    public int QtdeTotal { get; set; } // Quantidade total
    public decimal Valor { get; set; } // Valor da anotação mais recente, com 2 casas decimais
    public string TipoOcorrencia { get; set; } // Tipo de ocorrência ('V', '8', ou ' ')
    public string Origem { get; set; } // Nome do banco ou credor da anotação mais recente

    public PefinResumo(string rawData)
    {
      // Mapeamento dos campos com base no layout
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Valor = ExtensionService.SubstringBuscaDecimal(rawData, 31, 15); // Divisão por 100 para ajustar as casas decimais
      TipoOcorrencia = ExtensionService.NovoSubstring(rawData, 46, 1).Trim();
      Origem = ExtensionService.NovoSubstring(rawData, 47, 16).Trim();
    }
  }


  public class PefinDetail
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string Natureza { get; set; }
    public decimal Valor { get; set; }
    public string Praca { get; set; }
    public string Filler { get; set; }
    public string Principal { get; set; }
    public string Contrato { get; set; }
    public string SubJudice { get; set; }
    public string SerieCadus { get; set; }
    public string ChvCadus { get; set; }
    public string TipOcor { get; set; }
    public string Filler2 { get; set; }

    public PefinDetail(string rawData)
    {
      // Mapeando os campos com base no layout do manual
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim(); // Tipo_Reg (1 – 4)
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim(); // Subtipo (5 – 6)
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim(); // Data_Ocorr (7 – 14)
      Natureza = ExtensionService.NovoSubstring(rawData, 14, 3).Trim(); // Natureza (15 – 17)
      Valor = ExtensionService.SubstringBuscaDecimal(rawData, 17, 15); // Valor (18 – 32)
      Praca = ExtensionService.NovoSubstring(rawData, 32, 4).Trim(); // Praça (33 – 36)
      Filler = ExtensionService.NovoSubstring(rawData, 36, 32).Trim(); // Filler (37 – 68)
      Principal = ExtensionService.NovoSubstring(rawData, 68, 1).Trim(); // Principal (69)
      Contrato = ExtensionService.NovoSubstring(rawData, 69, 16).Trim(); // Contrato (70 – 85)
      SubJudice = ExtensionService.NovoSubstring(rawData, 85, 1).Trim(); // Sub-Judice (86)
      Filler2 = ExtensionService.NovoSubstring(rawData, 86, 8).Trim(); // Filler (87 – 94)
      SerieCadus = ExtensionService.NovoSubstring(rawData, 94, 1).Trim(); // Serie Cadus (95)
      ChvCadus = ExtensionService.NovoSubstring(rawData, 95, 10).Trim(); // CHV Cadus (96 – 105)
      TipOcor = ExtensionService.NovoSubstring(rawData, 105, 1).Trim(); // Tip-Ocor (106)
      Filler2 = ExtensionService.NovoSubstring(rawData, 106, 9).Trim(); // Filler (107 – 115)
    }
  }

  public class PefinDetail2
  {
    public string FilialCnpj { get; set; }
    public int DigitDoc { get; set; }
    public string InclusionDate { get; set; }
    public string InclusionHour { get; set; }
    public string OcurrencyType2 { get; set; }
    public string Modality { get; set; }

    public PefinDetail2(string rawData)
    {
      FilialCnpj = ExtensionService.NovoSubstring(rawData, 6, 4).Trim();
      DigitDoc = int.Parse(ExtensionService.NovoSubstring(rawData, 10, 2).Trim());
      InclusionDate = ExtensionService.NovoSubstring(rawData, 12, 8).Trim();
      InclusionHour = ExtensionService.NovoSubstring(rawData, 20, 6).Trim();
      OcurrencyType2 = ExtensionService.NovoSubstring(rawData, 26, 1).Trim();
      Modality = ExtensionService.NovoSubstring(rawData, 27, 12).Trim();
    }
  }

  public class PefinCredorDetail
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro
    public string DocumentoCredor { get; set; } // Documento do credor
    public string NomeCredor { get; set; } // Nome do credor
    public string TipoOcorrencia { get; set; } // Tipo de ocorrência ('V', '8', ou ' ')
    public string CreditorParticipant { get; set; } // Indica se há documento do credor ('C' ou 'P')

    public PefinCredorDetail(string rawData)
    {
      // Mapeamento dos campos com base no layout
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DocumentoCredor = ExtensionService.NovoSubstring(rawData, 6, 14).Trim();
      NomeCredor = ExtensionService.NovoSubstring(rawData, 20, 70).Trim();
      TipoOcorrencia = ExtensionService.NovoSubstring(rawData, 106, 1).Trim();
      CreditorParticipant = ExtensionService.NovoSubstring(rawData, 107, 1).Trim();
    }
  }

  public class PefinSubjudiceDetail
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro
    public string Praca { get; set; } // Código da praça
    public string Distribuidor { get; set; } // Código do distribuidor
    public string Vara { get; set; } // Vara
    public string Data { get; set; } // Data no formato AAAAMMDD
    public string Processo { get; set; } // Código do processo
    public string Mensagem { get; set; } // Mensagem
    public string TipoOcorrencia { get; set; } // Tipo de ocorrência ('V', '8', ou ' ')

    public PefinSubjudiceDetail(string rawData)
    {
      // Mapeamento dos campos com base no layout
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      Praca = ExtensionService.NovoSubstring(rawData, 6, 4).Trim();
      Distribuidor = ExtensionService.NovoSubstring(rawData, 10, 2).Trim();
      Vara = ExtensionService.NovoSubstring(rawData, 12, 2).Trim();
      Data = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      Processo = ExtensionService.NovoSubstring(rawData, 22, 16).Trim();
      Mensagem = ExtensionService.NovoSubstring(rawData, 38, 76).Trim();
      TipoOcorrencia = ExtensionService.NovoSubstring(rawData, 114, 1).Trim();
    }
  }
}
