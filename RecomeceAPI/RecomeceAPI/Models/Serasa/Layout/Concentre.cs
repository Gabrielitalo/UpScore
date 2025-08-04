using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecomeceAPI.Models.Serasa;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class Concentre
  {
    public RegI001 RegI001 { get; set; }  
    public RegI100 RegI100 { get; set; }  
    public RegI101 RegI101 { get; set; }  
    public RegI102 RegI102 { get; set; }
    public RegI110 RegI110 = new RegI110();
    public RegI120 RegI120 = new RegI120();
    public RegI130 RegI130 = new RegI130();
    public RegI140 RegI140 = new RegI140();
    public RegI150 RegI150 = new RegI150();
    public RegI160 RegI160 = new RegI160();
    public RegI170 RegI170 = new RegI170();
    public RegI220 RegI220 = new RegI220();
    public RegI230 RegI230 = new RegI230();
    public RegF900Score RegF900Score { get; set; }  
    public RegA900 RegA900 { get; set; }  
    public RegT999 RegT999 { get; set; }
    public decimal TotalNegativacao { get; set; } // Calculado 
    public DateTime DataConsulta { get; set; }

    public void Processar(int people, List<ArquivoRetornoConcentre> concentre)
    {
      RegI100 = new RegI100(concentre.Where(c => c.Registro == "I100").FirstOrDefault()?.Texto);
      RegI101 = new RegI101(concentre.Where(c => c.Registro == "I101").FirstOrDefault()?.Texto);
      RegF900Score = new RegF900Score(people, concentre.Where(c => c.Registro == "F900").FirstOrDefault()?.Texto);
      RegI102 = new RegI102(concentre.Where(c => c.Registro == "I102").FirstOrDefault()?.Texto);
      RegI110.Processar(concentre.Where(c => c.Registro == "I110").ToList());
      RegI120.Processar(concentre.Where(c => c.Registro == "I120").ToList());
      RegI130.Processar(concentre.Where(c => c.Registro == "I130").ToList());
      RegI140.Processar(concentre.Where(c => c.Registro == "I140").ToList());
      RegI150.Processar(concentre.Where(c => c.Registro == "I150").ToList());
      RegI160.Processar(concentre.Where(c => c.Registro == "I160").ToList());
      RegI170.Processar(concentre.Where(c => c.Registro == "I170").ToList());
      RegI220.Processar(concentre.Where(c => c.Registro == "I220").ToList());
      RegI230.Processar(concentre.Where(c => c.Registro == "I230").ToList());
      RegA900 = new RegA900(concentre.Where(c => c.Registro == "A900").FirstOrDefault()?.Texto);
      RegT999 = new RegT999(concentre.Where(c => c.Registro == "T999").FirstOrDefault()?.Texto);

      CalcularTotais(concentre);
      //TotalizarNegativacao();
    }

    public void CalcularTotais(List<ArquivoRetornoConcentre> concentre)
    {
      List<ArquivoRetornoConcentre> list = concentre.Where(d => d.Subtipo == "01").ToList();
      decimal total = list.Sum(x => x.Valor);
      TotalNegativacao = total;

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Registro == "I220")
          RegI220.TotalConsolidado += r.Valor;
        else if (r.Registro == "I140")
          RegI140.TotalConsolidado += r.Valor;
        else if (r.Registro == "I160")
          RegI160.TotalConsolidado += r.Valor;
        else if (r.Registro == "I170")
          RegI170.TotalConsolidado += r.Valor;
        else if (r.Registro == "I110")
          RegI100.TotalConsolidado += r.Valor;
        else if (r.Registro == "I120")
          RegI120.TotalConsolidado += r.Valor;
        else if (r.Registro == "I150")
          RegI150.TotalConsolidado += r.Valor;
        else if (r.Registro == "I130")
          RegI130.TotalConsolidado += r.Valor;
        else if (r.Registro == "I230")
          RegI230.TotalConsolidado =+ r.Valor;
      }
    }

    public void TotalizarNegativacao()
    {
      try
      {
        TotalNegativacao = RegI220.TotalConsolidado + RegI140.TotalConsolidado + RegI160.TotalConsolidado + RegI170.TotalConsolidado + RegI100.TotalConsolidado + RegI120.TotalConsolidado + RegI150.TotalConsolidado + RegI130.TotalConsolidado + RegI230.TotalConsolidado;
      }
      catch (Exception ex)
      {
      }
    }
  }
}
