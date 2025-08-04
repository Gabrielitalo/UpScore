namespace RecomeceAPI.Models.Common
{
  public class EmailModel
  {
    public string Remetente { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Smtp { get; set; } = string.Empty;
    public int Porta { get; set; } = 587;
    public string Destinatario { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;

    public List<EMailAnexoModel> Anexos = new List<EMailAnexoModel>();
  }

  public class EMailAnexoModel
  {
    public int Tipo { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Caminho { get; set; } = string.Empty;
    public byte[] ArqBytes { get; set; } = new byte[0];
  }

}
