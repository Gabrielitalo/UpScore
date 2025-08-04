namespace RecomeceAPI.Models.ASAAS
{
  public class PaginationModel<T>
  {
    public string Object { get; set; } = string.Empty;
    public bool HasMore { get; set; }
    public int TotalCount { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
    public List<T> Data { get; set; } = new();
  }
}
