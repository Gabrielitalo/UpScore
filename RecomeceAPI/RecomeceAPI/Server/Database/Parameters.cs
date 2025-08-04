namespace RecomeceAPI.Server.Database
{
  public class Parameters
  {
    public int LikeParam { get; set; }
    public string Field { get; set; }
    public object Value { get; set; }

    public List<Parameters> ParametersList = new List<Parameters>();

    public Parameters()
    {
      Field = "";
      Value = "0";
    }
    public void AddItem(string field, object value)
    {
      var existingParameter = ParametersList.FirstOrDefault(p => p.Field == field);
      if (existingParameter != null)
        existingParameter.Value = value;
      else
        ParametersList.Add(new Parameters { Field = field, Value = value });
    }
    public void AddItem(string field, object value, int likeParam)
    {
      var existingParameter = ParametersList.FirstOrDefault(p => p.Field == field);
      if (existingParameter != null)
        existingParameter.Value = value;
      else
        ParametersList.Add(new Parameters { Field = field, Value = value, LikeParam = likeParam });
    }
  }
}
