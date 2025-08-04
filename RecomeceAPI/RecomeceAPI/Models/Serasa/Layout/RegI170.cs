using System.Diagnostics;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI170
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public List<RegI170Componentes> Componentes { get; set; } = new List<RegI170Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI170Componentes detalhes = new RegI170Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "01")
        {
          // Adiciona o conjunto atual à lista se contiver dados
          if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
          {
            Componentes.Add(detalhes);
          }

          // Cria um novo conjunto para os detalhes
          detalhes = new RegI170Componentes
          {
            Detalhe = new RegI170Detalhe(r.Texto)
          };
        }
        else if (r.Subtipo == "02")
        {
          // Adiciona o detalhe 02 ao conjunto atual
          detalhes.Detalhe02 = new RegI170Detalhe02(r.Texto);
        }
      }

      // Adiciona o último conjunto de detalhes, se houver dados
      if (detalhes.Detalhe != null || detalhes.Detalhe02 != null)
      {
        Componentes.Add(detalhes);
      }

      Debug.WriteLine("Processamento de RegI170 finalizado.");
    }
  }

  public class RegI170Componentes
  {
    public RegI170Detalhe Detalhe { get; set; } // Detalhe de CCF (Subtipo 01)
    public RegI170Detalhe02 Detalhe02 { get; set; } // Detalhe adicional de CCF (Subtipo 02)
  }

  public class RegI170Detalhe
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string Banco { get; set; }
    public string Agencia { get; set; }
    public int QtdeCheq { get; set; }
    public string Praca { get; set; }
    public string UF { get; set; }
    public string NomeBanco { get; set; }
    public string NomeCidade { get; set; }
    public string Natureza { get; set; }
    public string FilialCNPJ { get; set; }
    public string DigDoc { get; set; }
    public string DataInclusao { get; set; }
    public string HoraInclusao { get; set; }

    public RegI170Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      Banco = ExtensionService.NovoSubstring(rawData, 14, 3).Trim();
      Agencia = ExtensionService.NovoSubstring(rawData, 17, 4).Trim();
      QtdeCheq = int.Parse(ExtensionService.NovoSubstring(rawData, 21, 9).Trim());
      Praca = ExtensionService.NovoSubstring(rawData, 30, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 34, 2).Trim();
      NomeBanco = ExtensionService.NovoSubstring(rawData, 36, 20).Trim();
      NomeCidade = ExtensionService.NovoSubstring(rawData, 56, 30).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 86, 2).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 88, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 92, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 94, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 102, 6).Trim();
    }
  }

  public class RegI170Detalhe02
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string ChvCadus { get; set; }

    public RegI170Detalhe02(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 6, 10).Trim();
    }
  }
}
