using MySqlX.XDevAPI;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI120
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI120Resumo RegI120Resumo { get; set; }
    public List<RegI120Componentes> RegI120Componentes = new List<RegI120Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI120Componentes detalhes = new RegI120Componentes();

      try
      {
        foreach (ArquivoRetornoConcentre r in list)
        {
          if (r.Subtipo == "00")
          {
            // Processa o resumo
            RegI120Resumo = new RegI120Resumo(r.Texto);
          }
          else if (r.Subtipo == "01")
          {
            // Adiciona o objeto atual à lista se ele contém dados
            if (detalhes.RegI120Detalhe != null || detalhes.RegI120Natureza != null || detalhes.RegI120Subjudice != null)
            {
              RegI120Componentes.Add(detalhes);
            }

            // Cria um novo objeto para o próximo conjunto de detalhes
            detalhes = new RegI120Componentes
            {
              RegI120Detalhe = new RegI120Detalhe(r.Texto)
            };
            TotalConsolidado += detalhes.RegI120Detalhe.Valor;
          }
          else if (r.Subtipo == "02")
          {
            // Adiciona a natureza ao mesmo conjunto de detalhes
            detalhes.RegI120Natureza = new RegI120Natureza(r.Texto);
          }
          else if (r.Subtipo == "03")
          {
            // Adiciona o subjudice ao mesmo conjunto de detalhes
            detalhes.RegI120Subjudice = new RegI120Subjudice(r.Texto);
          }
        }

        // Adiciona o último registro à lista, se houver dados
        if (detalhes.RegI120Detalhe != null || detalhes.RegI120Natureza != null || detalhes.RegI120Subjudice != null)
        {
          RegI120Componentes.Add(detalhes);
        }
      }
      catch(Exception ex)
      {
        System.Diagnostics.Debug.WriteLine(ex.Message);
      }

      System.Diagnostics.Debug.WriteLine("RegI120 acabou...");
    }

  }

  public class RegI120Componentes
  {
    public RegI120Detalhe? RegI120Detalhe { get; set; } // 01
    public RegI120Natureza? RegI120Natureza { get; set; } // 02
    public RegI120Subjudice? RegI120Subjudice { get; set; } // 03
  }

  public class RegI120Resumo
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "00")
    public string DataInicial { get; set; } // Data inicial (AAAAMMDD)
    public string DataFinal { get; set; } // Data final (AAAAMMDD)
    public int QtdeTotal { get; set; } // Quantidade total
    public decimal Valor { get; set; } // Valor da anotação mais recente (2 casas decimais)
    public string Origem { get; set; } // Nome da cidade de origem da anotação

    public RegI120Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 31, 15).Trim()) / 100;
      Origem = ExtensionService.NovoSubstring(rawData, 46, 30).Trim();
    }
  }

  public class RegI120Detalhe
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "01")
    public string DataOcorr { get; set; } // Data da ocorrência (AAAAMMDD)
    public string VaraCivel { get; set; } // Número da vara cível
    public string NumDistribuidor { get; set; } // Número do distribuidor
    public string Natureza { get; set; } // Código da natureza
    public decimal Valor { get; set; } // Valor da ação (2 casas decimais)
    public string Praca { get; set; } // Código da praça Embratel
    public string UF { get; set; } // Unidade federativa
    public string Cidade { get; set; } // Nome da cidade
    public string Principal { get; set; } // Indicador principal (S = principal, N = coobrigado)
    public string SubJudice { get; set; } // Indicador sub judice
    public string FilialCNPJ { get; set; } // Número da filial do CNPJ
    public string DigDoc { get; set; } // Dígito verificador do documento
    public string DataInclusao { get; set; } // Data de inclusão (AAAAMMDD)
    public string HoraInclusao { get; set; } // Hora de inclusão (HHMMSS)
    public string ChvCadus { get; set; } // Chave cadus

    public RegI120Detalhe(string rawData)
    {
      //TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      //Subtipo = ExtensionService.NovoSubstring(rawData, 4, 1).Trim(); // Corrigido: tamanho 1
      //DataOcorr = ExtensionService.NovoSubstring(rawData, 5, 8).Trim(); // Corrigido: começa em 5
      //VaraCivel = ExtensionService.NovoSubstring(rawData, 13, 4).Trim(); // Corrigido: começa em 13, tamanho 4
      //NumDistribuidor = ExtensionService.NovoSubstring(rawData, 17, 4).Trim(); // Corrigido: começa em 17
      //Natureza = ExtensionService.NovoSubstring(rawData, 21, 3).Trim(); // Corrigido: começa em 21
      //Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 24, 15).Trim()) / 100; // Corrigido: começa em 24
      //Praca = ExtensionService.NovoSubstring(rawData, 39, 4).Trim(); // Corrigido: começa em 39
      //UF = ExtensionService.NovoSubstring(rawData, 43, 2).Trim(); // Corrigido: começa em 43
      //Cidade = ExtensionService.NovoSubstring(rawData, 45, 30).Trim(); // Corrigido: começa em 45
      //Principal = ExtensionService.NovoSubstring(rawData, 75, 1).Trim(); // Ok
      //SubJudice = ExtensionService.NovoSubstring(rawData, 76, 1).Trim(); // Ok
      //FilialCNPJ = ExtensionService.NovoSubstring(rawData, 77, 4).Trim(); // Ok
      //DigDoc = ExtensionService.NovoSubstring(rawData, 81, 1).Trim(); // Corrigido: tamanho 1
      //DataInclusao = ExtensionService.NovoSubstring(rawData, 82, 8).Trim(); // Ok
      //HoraInclusao = ExtensionService.NovoSubstring(rawData, 90, 4).Trim(); // Corrigido: tamanho 4
      //ChvCadus = ExtensionService.NovoSubstring(rawData, 94, 6).Trim();

      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      VaraCivel = ExtensionService.NovoSubstring(rawData, 14, 4).Trim();
      NumDistribuidor = ExtensionService.NovoSubstring(rawData, 18, 6).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 26, 3).Trim();
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 25, 15).Trim()) / 100;
      Praca = ExtensionService.NovoSubstring(rawData, 40, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 44, 2).Trim();
      Cidade = ExtensionService.NovoSubstring(rawData, 46, 30).Trim();
      Principal = ExtensionService.NovoSubstring(rawData, 70, 1).Trim();
      SubJudice = ExtensionService.NovoSubstring(rawData, 71, 1).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 79, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 76, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 84, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 93, 6).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 92, 10).Trim();

    }
    public void AConferir(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      VaraCivel = ExtensionService.NovoSubstring(rawData, 14, 6).Trim();
      NumDistribuidor = ExtensionService.NovoSubstring(rawData, 20, 6).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 26, 3).Trim();
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 29, 15).Trim()) / 100;
      Praca = ExtensionService.NovoSubstring(rawData, 44, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 48, 2).Trim();
      Cidade = ExtensionService.NovoSubstring(rawData, 50, 20).Trim();
      Principal = ExtensionService.NovoSubstring(rawData, 70, 1).Trim();
      SubJudice = ExtensionService.NovoSubstring(rawData, 71, 1).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 72, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 76, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 78, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 86, 6).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 92, 10).Trim();
    }
  }

  public class RegI120Natureza
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "02")
    public string DescNatureza { get; set; } // Descrição da natureza

    public RegI120Natureza(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DescNatureza = ExtensionService.NovoSubstring(rawData, 6, 30).Trim();
    }
  }

  public class RegI120Subjudice
  {
    public string TipoReg { get; set; } // Tipo de registro
    public string Subtipo { get; set; } // Subtipo de registro (fixo = "03")
    public string Praca { get; set; } // Código da praça
    public string Distribuidor { get; set; } // Distribuidor
    public string Vara { get; set; } // Vara
    public string Data { get; set; } // Data (AAAAMMDD)
    public string Processo { get; set; } // Número do processo
    public string Mensagem { get; set; } // Mensagem

    public RegI120Subjudice(string rawData)
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
