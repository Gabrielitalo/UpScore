using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI110
  {
    public RegI110Resumo RegI110Resumo { get; set; } // 00
    public List<RegI110Componentes> RegI110Componentes = new List<RegI110Componentes> { };
    public decimal TotalConsolidado { get; set; }

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI110Componentes detalhes = new RegI110Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          // Processa o resumo
          RegI110Resumo = new RegI110Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          // Adiciona o objeto atual à lista, caso já tenha dados
          if (detalhes.RegI110Detalhe != null || detalhes.RegI110Subjudice != null)
          {
            RegI110Componentes.Add(detalhes);
          }

          // Cria um novo objeto para o próximo conjunto de detalhes
          detalhes = new RegI110Componentes
          {
            RegI110Detalhe = new RegI110Detalhe(r.Texto)
          };
          TotalConsolidado += detalhes.RegI110Detalhe.Valor;
        }
        else if (r.Subtipo == "02")
        {
          // Adiciona o subjudice ao mesmo conjunto de detalhes
          detalhes.RegI110Subjudice = new RegI110Subjudice(r.Texto);
        }
      }

      // Adiciona o último registro à lista, se houver dados
      if (detalhes.RegI110Detalhe != null || detalhes.RegI110Subjudice != null)
      {
        RegI110Componentes.Add(detalhes);
      }

      System.Diagnostics.Debug.WriteLine("RegI110 acabou...");
    }

  }

  public class RegI110Resumo
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "00")
    public string DataInicial { get; set; } // Data inicial (AAAAMMDD)
    public string DataFinal { get; set; } // Data final (AAAAMMDD)
    public int QtdeTotal { get; set; } // Quantidade total
    public decimal Valor { get; set; } // Valor da anotação mais recente (2 casas decimais)
    public string Origem { get; set; } // Nome da origem da anotação

    public RegI110Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 31, 15).Trim()) / 100;
      Origem = ExtensionService.NovoSubstring(rawData, 46, 40).Trim();
    }
  }

  public class RegI110Componentes
  {
    public RegI110Detalhe RegI110Detalhe { get; set; } // 01
    public RegI110Subjudice RegI110Subjudice { get; set; } // 02
  }

  public class RegI110Detalhe
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "01")
    public string DataOcorr { get; set; } // Data da ocorrência (AAAAMMDD)
    public string NumCartor { get; set; } // Número do cartório
    public string Natureza { get; set; } // Código da Natureza
    public decimal Valor { get; set; } // Valor da anotação (2 casas decimais)
    public string Praca { get; set; } // Código da praça
    public string UF { get; set; } // Unidade federativa
    public string Cidade { get; set; } // Nome da cidade
    public string SubJudice { get; set; } // Indicador sub judice
    public string DataCarta { get; set; } // Data da carta (AAAAMMDD)
    public string FilialCNPJ { get; set; } // Número da filial do CNPJ
    public string DigDoc { get; set; } // Dígito verificador do documento
    public string DataInclusao { get; set; } // Data de inclusão da anotação
    public string HoraInclusao { get; set; } // Hora da inclusão da anotação (HHMMSS)
    public string ChvCadus { get; set; } // Chave cadus

    public RegI110Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim(); // Posição 1-4
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim(); // Posição 5-6
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim(); // Posição 7-14
      NumCartor = ExtensionService.NovoSubstring(rawData, 14, 4).Trim(); // Posição 15-18
      Natureza = ExtensionService.NovoSubstring(rawData, 18, 3).Trim(); // Posição 19-21
      Valor = ExtensionService.SubstringBuscaDecimal(rawData, 21, 15); // Posição 22-36
      Praca = ExtensionService.NovoSubstring(rawData, 36, 4).Trim(); // Posição 37-40
      UF = ExtensionService.NovoSubstring(rawData, 40, 2).Trim(); // Posição 41-42
      Cidade = ExtensionService.NovoSubstring(rawData, 42, 30).Trim(); // Posição 43-72
      SubJudice = ExtensionService.NovoSubstring(rawData, 72, 1).Trim(); // Posição 73-73
      DataCarta = ExtensionService.NovoSubstring(rawData, 73, 8).Trim(); // Posição 74-81
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 81, 4).Trim(); // Posição 82-85
      DigDoc = ExtensionService.NovoSubstring(rawData, 85, 2).Trim(); // Posição 86-87
      DataInclusao = ExtensionService.NovoSubstring(rawData, 87, 8).Trim(); // Posição 88-95
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 95, 6).Trim(); // Posição 96-101
      ChvCadus = ExtensionService.NovoSubstring(rawData, 101, 10).Trim(); // Posição 102-111
    }
  }

  public class RegI110Subjudice
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "02")
    public string Praca { get; set; } // Código da praça
    public string Distribuidor { get; set; } // Distribuidor
    public string Vara { get; set; } // Vara
    public string Data { get; set; } // Data (AAAAMMDD)
    public string Processo { get; set; } // Número do processo
    public string Mensagem { get; set; } // Mensagem

    public RegI110Subjudice(string rawData)
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
