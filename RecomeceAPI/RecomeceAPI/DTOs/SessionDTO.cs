using RecomeceAPI.Models;

namespace RecomeceAPI.DTOs
{
  public class SessionDTO
  {
    public int Active { get; set; }
    public int IdType { get; set; }
    public int IdRole { get; set; }
    public long IdCompany { get; set; }
    public DateTime ExpiresSession { get; set; }
    public string Username { get; set; } = string.Empty;
    public bool IsAssociacao { get; set; } = false;
    public string Token { get; set; } = string.Empty;
    public WhiteLabelConfigModel WhiteLabelConfig { get; set; } = new WhiteLabelConfigModel();

  }
}
