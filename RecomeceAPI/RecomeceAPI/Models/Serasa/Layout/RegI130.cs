using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI130
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI130Resumo RegI130Resumo { get; set; }
    public List<RegI130Componentes> RegI130Componentes = new List<RegI130Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI130Componentes detalhes = new RegI130Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          RegI130Resumo = new RegI130Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          if (detalhes.RegI130Detalhe != null)
          {
            RegI130Componentes.Add(detalhes); // Adiciona o objeto atual à lista
          }

          // Cria um novo objeto `detalhes` e atribui o novo registro
          detalhes = new RegI130Componentes
          {
            RegI130Detalhe = new RegI130Detalhe(r.Texto)
          };
        }
      }

      // Adiciona o último registro `detalhes` à lista, se existir
      if (detalhes.RegI130Detalhe != null)
      {
        RegI130Componentes.Add(detalhes);
      }

      System.Diagnostics.Debug.WriteLine("RegI130 acabou...");
    }

  }

  public class RegI130Resumo
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
    public int QtdeTotal { get; set; }
    public string Origem { get; set; }

    public RegI130Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Origem = ExtensionService.NovoSubstring(rawData, 31, 30).Trim();
    }
  }

  public class RegI130Componentes
  {
    public RegI130Detalhe RegI130Detalhe { get; set; }
  }
  public class RegI130Detalhe
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string VaraCivel { get; set; }
    public string Praca { get; set; }
    public string UF { get; set; }
    public string Cidade { get; set; }
    public string FilialCNPJ { get; set; }
    public string DigDoc { get; set; }
    public string DataInclusao { get; set; }
    public string HoraInclusao { get; set; }
    public string ChvCadus { get; set; }
    public string DescNatureza { get; set; }

    public RegI130Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      VaraCivel = ExtensionService.NovoSubstring(rawData, 14, 6).Trim();
      Praca = ExtensionService.NovoSubstring(rawData, 20, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 24, 2).Trim();
      Cidade = ExtensionService.NovoSubstring(rawData, 26, 20).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 46, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 50, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 52, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 60, 6).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 66, 10).Trim();
      DescNatureza = ExtensionService.NovoSubstring(rawData, 76, 30).Trim();
    }
  }

}
