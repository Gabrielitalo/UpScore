using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI140
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI140Resumo Resumo { get; set; } // Resumo de REFIN (Subtipo 00)
    public List<RegI140Componentes> Componentes { get; set; } = new List<RegI140Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI140Componentes detalhes = new RegI140Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          Resumo = new RegI140Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          if (detalhes.Detalhe != null || detalhes.Detalhe02 != null || detalhes.Subjudice != null)
          {
            Componentes.Add(detalhes);
          }

          detalhes = new RegI140Componentes
          {
            Detalhe = new RegI140Detalhe(r.Texto)
          };
          TotalConsolidado += detalhes.Detalhe.Valor;
        }
        else if (r.Subtipo == "02")
        {
          detalhes.Detalhe02 = new RegI140Detalhe02(r.Texto);
        }
        else if (r.Subtipo == "03")
        {
          detalhes.Subjudice = new RegI140Subjudice(r.Texto);
        }
      }

      if (detalhes.Detalhe != null || detalhes.Detalhe02 != null || detalhes.Subjudice != null)
      {
        Componentes.Add(detalhes);
      }

      System.Diagnostics.Debug.WriteLine("Processamento de RegI140 finalizado.");
    }
  }

  public class RegI140Componentes
  {
    public RegI140Detalhe Detalhe { get; set; } // Detalhe de REFIN (Subtipo 01)
    public RegI140Detalhe02 Detalhe02 { get; set; } // Detalhe adicional (Subtipo 02)
    public RegI140Subjudice Subjudice { get; set; } // Detalhe de Subjudice (Subtipo 03)
  }

  public class RegI140Resumo
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
    public int QtdeTotal { get; set; }
    public decimal Valor { get; set; }
    public string Origem { get; set; }

    public RegI140Resumo(string rawData)
    {
      if (rawData is null)
        return;
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 31, 15).Trim()) / 100;
      Origem = ExtensionService.NovoSubstring(rawData, 46, 16).Trim();
    }
  }

  public class RegI140Detalhe
  {
    public string TipoReg { get; set; }      // (1 - 4)
    public string Subtipo { get; set; }      // (5 - 6)
    public string DataOcorr { get; set; }    // (7 - 14)
    public string Natureza { get; set; }     // (15 - 17)
    public string CNPJOrigem { get; set; }   // (18 - 32)
    public decimal Valor { get; set; }       // (33 - 47)
    public string Praca { get; set; }        // (48 - 51)
    public string UF { get; set; }           // (52 - 53)
    public string NomeEmpresa { get; set; }  // (54 - 73)
    public string Cidade { get; set; }       // (74 - 103)
    public string Principal { get; set; }    // (104 - 104)
    public string SerieCadus { get; set; }   // (105 - 105)
    public string ChvCadus { get; set; }     // (106 - 115)

    public RegI140Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 14, 3).Trim();
      CNPJOrigem = ExtensionService.NovoSubstring(rawData, 17, 15).Trim();
      Valor = ExtensionService.SubstringBuscaDecimal(rawData, 32, 15);
      Praca = ExtensionService.NovoSubstring(rawData, 47, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 51, 2).Trim();
      NomeEmpresa = ExtensionService.NovoSubstring(rawData, 53, 20).Trim();
      Cidade = ExtensionService.NovoSubstring(rawData, 73, 30).Trim();
      Principal = ExtensionService.NovoSubstring(rawData, 103, 1).Trim();
      SerieCadus = ExtensionService.NovoSubstring(rawData, 104, 1).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 105, 10).Trim();
    }
  }

  public class RegI140Detalhe02
  {
    public string TipoReg { get; set; }         // Tipo de registro
    public string Subtipo { get; set; }         // Subtipo de registro
    public string SubJudice { get; set; }       // Anotação SUB JUDICE
    public string FilialCNPJ { get; set; }      // Número da filial do CNPJ consultado
    public string DigDoc { get; set; }          // Dígito verificador do documento consultado
    public string DataInclusao { get; set; }    // Data da inclusão da anotação (AAAAMMDD)
    public string HoraInclusao { get; set; }    // Hora da inclusão da anotação (HHMMSS)
    public string Contrato { get; set; }        // Código do contrato
    public string Modalidade { get; set; }      // Descrição da modalidade (natureza)

    public RegI140Detalhe02(string rawData)
    {
      if (rawData is null)
        return;

      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      SubJudice = ExtensionService.NovoSubstring(rawData, 6, 1).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 7, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 11, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 13, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 21, 6).Trim();
      Contrato = ExtensionService.NovoSubstring(rawData, 27, 16).Trim();
      Modalidade = ExtensionService.NovoSubstring(rawData, 43, 30).Trim();
    }
  }

  public class RegI140Subjudice
  {
    public string TipoReg { get; set; }       // Tipo de registro
    public string Subtipo { get; set; }       // Subtipo de registro
    public string Praca { get; set; }         // Código da praça
    public string Distribuidor { get; set; }  // Código do distribuidor
    public string Vara { get; set; }          // Vara
    public string Data { get; set; }          // Data, no formato AAAAMMDD
    public string Processo { get; set; }      // Código do processo
    public string Mensagem { get; set; }      // Mensagem

    public RegI140Subjudice(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      Praca = ExtensionService.NovoSubstring(rawData, 6, 4).Trim();
      Distribuidor = ExtensionService.NovoSubstring(rawData, 10, 2).Trim();
      Vara = ExtensionService.NovoSubstring(rawData, 12, 2).Trim();
      Data = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      Processo = ExtensionService.NovoSubstring(rawData, 22, 16).Trim();
      Mensagem = ExtensionService.NovoSubstring(rawData, 38, 76).Trim();
    }

  }
}
