using System.Net.Mail;
using System.Net;
using RecomeceAPI.Models.Common;
using RecomeceAPI.Interfaces;

namespace RecomeceAPI.Services.Common
{
  public class EmailService
  {
    private readonly IFileService _fileService;
    private readonly HandleCatchService _handleCatchService;
    public EmailService(IFileService fileService, HandleCatchService handleCatchService)
    {
      _fileService = fileService;
      _handleCatchService = handleCatchService;
    }
    public SmtpClient AbrirSMTP(EmailModel email)
    {
      SmtpClient smtp = new SmtpClient();
      smtp.EnableSsl = true;
      smtp.UseDefaultCredentials = false;
      smtp.Host = email.Smtp;
      smtp.Port = email.Porta;
      smtp.Credentials = new NetworkCredential(email.Remetente, email.Senha);
      return smtp;
    }
    public void SetPath(EMailAnexoModel attach)
    {
      attach.ArqBytes = _fileService.GetFileBytes(attach.Caminho);
      attach.Nome = _fileService.GetFileInfo(attach.Caminho).Name;
    }
    public void AnexarArquivos(MailMessage mail, EmailModel email)
    {
      foreach (EMailAnexoModel e in email.Anexos)
      {
        if (e.Tipo == 0)
        {
          SetPath(e);
        }
        else if (e.Tipo == 1)
        {
          e.ArqBytes = Convert.FromBase64String(ExtensionService.RemoveBase64Prefix(e.Caminho));
        }
        if (e.ArqBytes.Length > 0)
          mail.Attachments.Add(new Attachment(new MemoryStream(e.ArqBytes), e.Nome));
      }
    }
    public bool EnviarEmail(EmailModel email)
    {
      MailMessage mail = new MailMessage();
      SmtpClient smtp = AbrirSMTP(email);
      bool isSend = false;
      try
      {
        // Anexando arquivos
        AnexarArquivos(mail, email);

        // Iniciando o SMTP
        mail.From = new MailAddress(email.Remetente);
        mail.To.Add(new MailAddress(email.Destinatario));
        mail.Subject = email.Assunto;
        mail.Body = email.Mensagem;
        mail.IsBodyHtml = true;

        smtp.Send(mail);
        isSend = true;
      }
      catch (Exception ex)
      {
        _handleCatchService.HandleCatch(ex, $"Falha ao enviar e-mail de {email.Remetente} para {email.Destinatario} \r\r {email.Mensagem}");
        isSend = false;
      }
      finally
      {
        // Limpando a memória 
        mail.Dispose();
        smtp.Dispose();
      }

      return isSend;
    }
  }
}
