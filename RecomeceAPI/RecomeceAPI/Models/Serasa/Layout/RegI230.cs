using System;
using System.Collections.Generic;
using System.Diagnostics;
using RecomeceAPI.Models.Serasa;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegI230
  {
    public decimal TotalConsolidado { get; set; } // Calculado 
    public RegI230Resumo Resumo { get; set; } // Resumo do Registro I230 (Subtipo 00)
    public List<RegI230Componentes> Componentes { get; set; } = new List<RegI230Componentes>();

    public void Processar(List<ArquivoRetornoConcentre> list)
    {
      RegI230Componentes detalhes = new RegI230Componentes();

      foreach (ArquivoRetornoConcentre r in list)
      {
        if (r.Subtipo == "00")
        {
          Resumo = new RegI230Resumo(r.Texto);
        }
        else if (r.Subtipo == "01")
        {
          // Adiciona o conjunto atual à lista se contiver dados
          if (detalhes.Detalhe != null || detalhes.Detalhe02 != null || detalhes.Detalhe03 != null)
          {
            Componentes.Add(detalhes);
          }

          // Cria um novo conjunto para os detalhes
          detalhes = new RegI230Componentes
          {
            Detalhe = new RegI230Detalhe(r.Texto)
          };
          TotalConsolidado += detalhes.Detalhe.Valor;
        }
        else if (r.Subtipo == "02")
        {
          detalhes.Detalhe02 = new RegI230Detalhe02(r.Texto);
        }
        else if (r.Subtipo == "03")
        {
          detalhes.Detalhe03 = new RegI230Detalhe03(r.Texto);
        }
      }

      // Adiciona o último conjunto de detalhes, se houver dados
      if (detalhes.Detalhe != null || detalhes.Detalhe02 != null || detalhes.Detalhe03 != null)
      {
        Componentes.Add(detalhes);
      }

      Debug.WriteLine("Processamento de RegI230 finalizado.");
    }
  }

  public class RegI230Componentes
  {
    public RegI230Detalhe Detalhe { get; set; } // Detalhe do Registro I230 (Subtipo 01)
    public RegI230Detalhe02 Detalhe02 { get; set; } // Detalhe Adicional de Registro I230 (Subtipo 02)
    public RegI230Detalhe03 Detalhe03 { get; set; } // Detalhe Complementar de Registro I230 (Subtipo 03)
  }

  public class RegI230Resumo
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataInicial { get; set; }
    public string DataFinal { get; set; }
    public int QtdeTotal { get; set; }
    public decimal Valor { get; set; }
    public string Origem { get; set; }

    public RegI230Resumo(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataInicial = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      DataFinal = ExtensionService.NovoSubstring(rawData, 14, 8).Trim();
      QtdeTotal = int.Parse(ExtensionService.NovoSubstring(rawData, 22, 9).Trim());
      Valor = decimal.Parse(ExtensionService.NovoSubstring(rawData, 31, 15).Trim()) / 100;
      Origem = ExtensionService.NovoSubstring(rawData, 46, 16).Trim();
    }
  }

  public class RegI230Detalhe
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DataOcorr { get; set; }
    public string Natureza { get; set; }
    public decimal Valor { get; set; }
    public string Praca { get; set; }
    public string UF { get; set; }
    public string NomeInst { get; set; }
    public string Contrato { get; set; }
    public string CNPJCredor { get; set; }
    public string ChvCadus { get; set; }
    public string SerieCadus { get; set; }

    public RegI230Detalhe(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DataOcorr = ExtensionService.NovoSubstring(rawData, 6, 8).Trim();
      Natureza = ExtensionService.NovoSubstring(rawData, 14, 3).Trim();
      Valor = ExtensionService.SubstringBuscaDecimal(rawData, 17, 15);
      Praca = ExtensionService.NovoSubstring(rawData, 32, 4).Trim();
      UF = ExtensionService.NovoSubstring(rawData, 36, 2).Trim();
      NomeInst = ExtensionService.NovoSubstring(rawData, 38, 29).Trim();
      Contrato = ExtensionService.NovoSubstring(rawData, 68, 16).Trim();
      CNPJCredor = ExtensionService.NovoSubstring(rawData, 85, 9).Trim();
      ChvCadus = ExtensionService.NovoSubstring(rawData, 93, 10).Trim();
      SerieCadus = ExtensionService.NovoSubstring(rawData, 103, 1).Trim();
    }
  }

  public class RegI230Detalhe02
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string DescNatureza { get; set; }

    public RegI230Detalhe02(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      DescNatureza = ExtensionService.NovoSubstring(rawData, 6, 30).Trim();
    }
  }

  public class RegI230Detalhe03
  {
    public string TipoReg { get; set; }
    public string Subtipo { get; set; }
    public string FilialCNPJ { get; set; }
    public string DigDoc { get; set; }
    public string DataInclusao { get; set; }
    public string HoraInclusao { get; set; }
    public string MsgSubJudice { get; set; }

    public RegI230Detalhe03(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim();
      Subtipo = ExtensionService.NovoSubstring(rawData, 4, 2).Trim();
      FilialCNPJ = ExtensionService.NovoSubstring(rawData, 6, 4).Trim();
      DigDoc = ExtensionService.NovoSubstring(rawData, 10, 2).Trim();
      DataInclusao = ExtensionService.NovoSubstring(rawData, 12, 8).Trim();
      HoraInclusao = ExtensionService.NovoSubstring(rawData, 20, 6).Trim();
      MsgSubJudice = ExtensionService.NovoSubstring(rawData, 26, 78).Trim();
    }
  }
}
