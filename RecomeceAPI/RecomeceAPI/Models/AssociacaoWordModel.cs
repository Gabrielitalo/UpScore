using RecomeceAPI.Services.Common;
using System.Data;

namespace RecomeceAPI.Models
{
  public class AssociacaoWordModel
  {
    public string Nome { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;

    public static List<AssociacaoWordModel> GetListFromDataTable(DataTable dt)
    {
      var list = new List<AssociacaoWordModel>();

      if (dt?.Rows.Count > 0)
      {
        foreach (DataRow row in dt.Rows)
        {
          list.Add(new AssociacaoWordModel
          {
            Nome = row.Field<string>("Nome") ?? "",
            Documento = ExtensionService.AplicarMascaraCpfCnpj(row.Field<string>("Inscricao") ?? "")
          });
        }
      }

      return list;
    }

  }
}
