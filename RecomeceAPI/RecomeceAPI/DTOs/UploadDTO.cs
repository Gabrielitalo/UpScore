namespace RecomeceAPI.DTOs
{
  public class UploadDTO
  {
    public string Base64 { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int Tipo { get; set; }
  }
}
