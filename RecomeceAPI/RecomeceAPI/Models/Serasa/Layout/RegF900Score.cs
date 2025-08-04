using System;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa.Layout
{
  public class RegF900Score
  {
    public string TipoReg { get; set; } // Tipo do Registro
    public string Tipo { get; set; } // Descrição do Scoring escolhido
    public int Score { get; set; } // Pontuação do Score
    public string Range { get; set; } // Nome das faixas de cada intervalo para classificação da pontuação
    public decimal Taxa { get; set; } // Probabilidade de inadimplência
    public string Mensagem { get; set; } // Mensagem relativa ao Scoring
    public string CodigoMensagem { get; set; } // Código da Mensagem
    public string DataConsulta { get; set; } // Código da Mensagem
    public string HoraConsulta { get; set; } // Código da Mensagem

    public RegF900Score(int people, string rawData)
    {
      if (rawData is null)
        return;

      if(rawData.Contains("SCORE NAO CALCULADO") || rawData.Contains("SCORE INIBIDO PELO PROPRIO TITULAR CONSULTADO"))
      {
        NaoCalculado();
        return;
      }

      if (people == 1)
        SetPF(rawData);
      else 
        SetPJ(rawData);
    }

    public void NaoCalculado()
    {
      TipoReg = "";
      Tipo = "";
      Score = 0;
      Range = "0";
      Taxa = 0;
      Mensagem = "SCORE NAO CALCULADO - INSUFICIENCIA INFORMACOES BASE DE DADOS SERASA EXPERIAN";
      CodigoMensagem = "404";
    }

    public void SetPF(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim(); // Posição 1-4
      Tipo = ExtensionService.NovoSubstring(rawData, 4, 4).Trim(); // Posição 5-8
      Score = int.Parse(ExtensionService.NovoSubstring(rawData, 8, 6).Trim()); // Posição 9-14
      Range = ExtensionService.NovoSubstring(rawData, 14, 6).Trim(); // Posição 15-20
      Taxa = ExtensionService.SubstringBuscaDecimal(rawData, 20, 5); // Posição 21-25
      Mensagem = ExtensionService.NovoSubstring(rawData, 25, 49).Trim(); // Posição 26-74
      CodigoMensagem = ExtensionService.NovoSubstring(rawData, 74, 6).Trim(); // Posição 75-80
    }
    public void SetPJ(string rawData)
    {
      TipoReg = ExtensionService.NovoSubstring(rawData, 0, 4).Trim(); // Posição 1-4
      Tipo = ExtensionService.NovoSubstring(rawData, 4, 4).Trim(); // Posição 5-8
      Score = int.Parse(ExtensionService.NovoSubstring(rawData, 27, 4).Trim()); // Posição 9-14
      Range = ExtensionService.NovoSubstring(rawData, 14, 6).Trim(); // Posição 15-20
      Taxa = ExtensionService.SubstringParseDecimal(rawData, 31, 5); // Posição 21-25
      Mensagem = ExtensionService.NovoSubstring(rawData, 36, 74).Trim(); // Posição 26-74
      CodigoMensagem = ExtensionService.NovoSubstring(rawData, 36, 4).Trim(); // Posição 75-80

      DataConsulta = ExtensionService.NovoSubstring(rawData, 11, 8).Trim(); // Posição 75-80
      HoraConsulta = ExtensionService.NovoSubstring(rawData, 19, 8).Trim(); // Posição 75-80
    }
  }
}
