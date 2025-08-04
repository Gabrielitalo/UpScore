using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI160
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI160Resumo Resumo { get; set; } // Resumo do Registro I160 (Subtipo 00)
    public List<RegI160Componentes> Componentes { get; set; } = new List<RegI160Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI160Componentes detalhes = new RegI160Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          Resumo = new RegI160Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          // Adiciona o conjunto atual à lista se contiver dados
          if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
          {
            Componentes.Add(detalhes);
          }
          // Cria um novo conjunto para os detalhes
          detalhes = new RegI160Componentes
          {
            Detalhe = new RegI160Detalhe(r.Texto)
          };
          TotalConsolidado += detalhes.Detalhe.Valor;
        }
        else if (r.Subtipo == "02")
        {
          // Adiciona o detalhe 02 ao conjunto atual
          detalhes.Detalhe02 = new RegI160Detalhe02(r.Texto);
        }
      }

      // Adiciona o último conjunto de detalhes, se houver dados
      if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
      {
        Componentes.Add(detalhes);
      }

      System.Diagnostics.Debug.WriteLine("Processamento de RegI160 finalizado.");
    }
  }

  public class RegI160Componentes
  {
    public RegI160Detalhe Detalhe { get; set; } // Detalhe do Registro I160 (Subtipo 01)
    public RegI160Detalhe02 Detalhe02 { get; set; } // Detalhe adicional do Registro I160 (Subtipo 02)
  }


  public class RegI160Resumo
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
    public decimal Valor { get; set; }
    public int QtdeTotal { get; set; }
    public string Origem { get; set; }

    public RegI160Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 22, 15).Trim()) / 100;
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 37, 9).Trim());
      Origem = ExtensionService.NovoSubstring(rawData, 46, 30).Trim();
    }
  }

  public class RegI160Detalhe
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string Banco { get; set; }
    public string Agencia { get; set; }
    public string ContaOcorr { get; set; }
    public string Natureza { get; set; }
    public decimal Valor { get; set; }
    public string Praca { get; set; }
    public string UF { get; set; }
    public string NomeBanco { get; set; }
    public string NumCheque { get; set; }
    public string Cidade { get; set; }

    public RegI160Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      Banco = ExtensionService.NovoSubstring(rawData, 14, 3).Trim();
      Agencia = ExtensionService.NovoSubstring(rawData, 17, 4).Trim();
      ContaOcorr = ExtensionService.NovoSubstring(rawData, 21, 9).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 30, 2).Trim();
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 32, 15).Trim()) / 100;
      Praca = ExtensionService.NovoSubstring(rawData, 47, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 51, 2).Trim();
      NomeBanco = ExtensionService.NovoSubstring(rawData, 53, 20).Trim();
      NumCheque = ExtensionService.NovoSubstring(rawData, 73, 10).Trim();
      Cidade = ExtensionService.NovoSubstring(rawData, 83, 30).Trim();
    }
  }

  public class RegI160Detalhe02
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string FilialCNPJ { get; set; }
    public string DigDoc { get; set; }
    public string DataInclusao { get; set; }
    public string HoraInclusao { get; set; }
    public string ChvCadus { get; set; }

    public RegI160Detalhe02(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 6, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 10, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 12, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 20, 6).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 26, 10).Trim();
    }
  }
}
