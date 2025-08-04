using System.Diagnostics;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI150
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI150Resumo Resumo { get; set; } // Resumo de Participação em Insucesso Empresarial (Subtipo 00)
    public List<RegI150Componentes> RegI150Componentes { get; set; } = new List<RegI150Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI150Componentes detalhes = new RegI150Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          Resumo = new RegI150Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          // Adiciona o objeto anterior, se válido
          if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
          {
            RegI150Componentes.Add(detalhes);
          }

          // Cria um novo conjunto de detalhes
          detalhes = new RegI150Componentes
          {
            Detalhe = new RegI150Detalhe(r.Texto)
          };
        }
        else if (r.Subtipo == "02")
        {
          // Adiciona o detalhe adicional ao mesmo conjunto
          detalhes.Detalhe02 = new RegI150Detalhe02(r.Texto);
        }
      }

      // Adiciona o último conjunto de detalhes, se válido
      if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
      {
        RegI150Componentes.Add(detalhes);
      }

      Debug.WriteLine("Processamento de RegI150 finalizado.");
    }
  }

  public class RegI150Componentes
  {
    public RegI150Detalhe Detalhe { get; set; } // Detalhe de Participação em Insucesso Empresarial (Subtipo 01)
    public RegI150Detalhe02 Detalhe02 { get; set; } // Detalhe Adicional de Participação (Subtipo 02)
  }

  public class RegI150Resumo
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
    public decimal ValorTotal { get; set; }
    public int QtdeTotal { get; set; }
    public string Origem { get; set; }

    public RegI150Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      ValorTotal = decimal.Parse(ExtensionService.NovoSubstring(rawData, 22, 15).Trim()) / 100;
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 37, 9).Trim());
      Origem = ExtensionService.NovoSubstring(rawData, 46, 30).Trim();
    }
  }

  public class RegI150Detalhe
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string Natureza { get; set; }
    public string Qualificacao { get; set; }
    public string VaraCivel { get; set; }
    public string CNPJEmpr { get; set; }
    public string NomeEmpr { get; set; }
    public string FilialCNPJ { get; set; }
    public string DigDoc { get; set; }
    public string DataInclusao { get; set; }
    public string HoraInclusao { get; set; }
    public string ChvCadus { get; set; }

    public RegI150Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 14, 3).Trim();
      Qualificacao = ExtensionService.NovoSubstring(rawData, 17, 4).Trim();
      VaraCivel = ExtensionService.NovoSubstring(rawData, 21, 6).Trim();
      CNPJEmpr = ExtensionService.NovoSubstring(rawData, 27, 14).Trim();
      NomeEmpr = ExtensionService.NovoSubstring(rawData, 41, 20).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 61, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 65, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 67, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 75, 6).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 81, 10).Trim();
    }
  }

  public class RegI150Detalhe02
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DescNatureza { get; set; }

    public RegI150Detalhe02(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DescNatureza = ExtensionService.NovoSubstring(rawData, 6, 30).Trim();
    }
  }
}
