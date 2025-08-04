namespace RecomeceAPI.Models.Common
{
  public class NotificationModel
  {
    public int Type { get; set; }
    public string? Title { get; set; }
    public string Content { get; set; } = string.Empty;
    public object? Item { get; set; } = new object();
  }
}
