using Org.BouncyCastle.Asn1.Pkcs;
using RecomeceAPI.Services.Common;

namespace RecomeceAPI.Models.Serasa
{
  public class ArquivoRetornoConcentre
  {
    public string Registro { get; set; }
    public string Subtipo { get; set; }
    public string Texto { get; set; }
    public decimal Valor { get; set; }

    public string LimparArquivoRetorno(string a)
    {
      return a.Replace("\n", "").Replace("\r", "");
    }

    public ArquivoRetornoConcentre RetornaInicio(string arquivo)
    {
      ArquivoRetornoConcentre concentre = new ArquivoRetornoConcentre
      {
        Texto = arquivo?.Substring(0, 400),
        Registro = "B49C"
      };
      return concentre;
    }

    public ArquivoRetornoConcentre RetornaRegistrosTexto(string arquivo)
    {
      ArquivoRetornoConcentre concentre = new ArquivoRetornoConcentre();
      concentre.Registro = arquivo?.Substring(0, 4);
      if (concentre.Registro == "F900")
        concentre.Subtipo = arquivo?.Substring(8, 3);
      else
        concentre.Subtipo = arquivo?.Substring(4, 2);

      int tamanhoRegistro = 115; // Tamanho padrão para outros registros
      concentre.Texto = ExtensionService.NovoSubstring(arquivo, 0, tamanhoRegistro);
      if (concentre.Subtipo == "01")
        concentre.BuscaTolizador();
      return concentre;
    }

    public List<ArquivoRetornoConcentre> MapearArquivo(string arquivoBase, List<ArquivoRetornoConcentre> concentre = null)
    {
      arquivoBase = LimparArquivoRetorno(arquivoBase);
      if (concentre == null)
        concentre = new List<ArquivoRetornoConcentre>();

      // Adiciona o registro inicial
      concentre.Add(RetornaInicio(arquivoBase));
      string arquivoTemp = arquivoBase.Substring(400); // Remove o registro inicial

      // Tamanho fixo dos registros
      int tamanhoRegistro = 115;

      while (arquivoTemp.Length >= tamanhoRegistro)
      {
        ArquivoRetornoConcentre temp = RetornaRegistrosTexto(arquivoTemp);

        if (!string.IsNullOrEmpty(temp.Texto))
        {
          concentre.Add(temp);
          arquivoTemp = arquivoTemp.Substring(tamanhoRegistro); // Avança para o próximo bloco
        }
        else
        {
          break; // Para o loop se o registro estiver vazio
        }
      }

      return concentre;
    }

    public void BuscaTolizador()
    {
      if (Registro == "I220")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 17, 15);
      else if (Registro == "I110")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 21, 15);
      else if (Registro == "I120")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 25, 15);
      else if (Registro == "I140")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 32, 15);
      else if (Registro == "I160")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 32, 15);
      else if (Registro == "I230")
        Valor = ExtensionService.SubstringBuscaDecimal(Texto, 17, 15);

      //if (Registro == "I140" || Registro == "I160" || Registro == "I170" || Registro == "I230")
      //  TotalizaRegistroPos63();
      //else if (Registro == "I420")
      //  TotalizaScore();
      //else if (Registro == "I110" || Registro == "I120" || Registro == "I130" || Registro == "I150")
      //  TotalizaRegistroPos77();
      //else if (Registro == "I220")
      //  BuscaDecimal(64, 76);
      //else if (Registro == "F900" && Subtipo == "001")
      //  BuscaDecimal(59, 71);
    }
    public void TotalizaRegistroPos63()
    {
      try
      {
        string valor = Texto.Substring(63, 12) + "." + Texto.Substring(75, 2);
        Valor = Convert.ToDecimal(valor);
      }
      catch
      {
        Valor = 0;
      }
    }
    public void TotalizaScore()
    {
      try
      {
        string valor = Texto.Substring(12, 6).Trim().Replace(" ", "").Replace(",", ".");
        Valor = Convert.ToDecimal(valor);
      }
      catch
      {
        Valor = 0;
      }
    }
    public void TotalizaRegistroPos77()
    {
      try
      {
        string valor = Texto.Substring(77, 12) + "." + Texto.Substring(89, 2);
        Valor = Convert.ToDecimal(valor);
      }
      catch
      {
        Valor = 0;
      }
    }
    public void BuscaDecimal(int inicial, int dec)
    {
      try
      {
        string valor = Texto.Substring(inicial, 12) + "." + Texto.Substring(dec, 2);
        Valor = Convert.ToDecimal(valor);
      }
      catch
      {
        Valor = 0;
      }
    }
    public decimal TotalGeral(List<ArquivoRetornoConcentre> concentre)
    {
      return concentre.Select(c => c.Valor).Sum();
    }

    public static string CaracteresVazios(int l)
    {
      string s = "";
      for (int i = 0; i < l; i++)
      {
        s += " ";
      }
      return s;
    }
    public static string CaracteresZero(int l)
    {
      string s = "";
      for (int i = 0; i < l; i++)
      {
        s += "0";
      }
      return s;
    }
  }
}
