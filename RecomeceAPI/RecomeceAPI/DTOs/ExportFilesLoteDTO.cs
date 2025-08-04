using RecomeceAPI.Models;
using System.Data;
using System.IO.Compression;

namespace RecomeceAPI.DTOs
{
  public class ExportFilesLoteDTO
  {
    public byte[] ArquivoBytes { get; set; } = new byte[1];
    public string NomeArquivo { get; set; } = string.Empty;

    public static List<ExportFilesLoteDTO> GetListFromDataTable(DataTable dt)
    {
      List<ExportFilesLoteDTO> list = new List<ExportFilesLoteDTO>();
      if (dt?.Rows.Count > 0)
      {
        foreach (DataRow row in dt.Rows)
        {
          list.Add(new ExportFilesLoteDTO
          {
            ArquivoBytes = row.Field<byte[]>("ArquivoBytes") ?? new byte[1],
            NomeArquivo = row.Field<string>("NomeArquivo") ?? ""
          });
        }
      }
      return list; 
    }
    public static byte[] ZipFiles (List<ExportFilesLoteDTO> list)
    {
      using (var zipStream = new MemoryStream())
      {
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
          foreach (var item in list)
          {
            var entry = archive.CreateEntry(item.NomeArquivo, CompressionLevel.Fastest);
            using var entryStream = entry.Open();
            entryStream.Write(item.ArquivoBytes, 0, item.ArquivoBytes.Length);
          }
        }

        zipStream.Position = 0;
        return zipStream.ToArray();
      }
    }
  }
}
