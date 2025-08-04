using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf.IO;
using PdfSharpCore.Pdf;
using QuestPDF.Fluent;
using RecomeceAPI.Services.PDFs;

namespace RecomeceAPI.Services.Common
{
  public static class PdfSharpService
  {
    public static void AdicionarPagina(string caminhoEntrada, string caminhoSaida)
    {
      // Abre o documento existente
      using var inputStream = File.OpenRead(caminhoEntrada);
      var documento = PdfReader.Open(inputStream, PdfDocumentOpenMode.Modify);

      // Cria nova página
      PdfPage novaPagina = documento.AddPage();
      XGraphics gfx = XGraphics.FromPdfPage(novaPagina);
      gfx.DrawString("Nova página adicionada", new XFont("Arial", 20), XBrushes.Black,
                     new XRect(0, 0, novaPagina.Width, novaPagina.Height),
                     XStringFormats.Center);

      // Salva o novo arquivo
      documento.Save(caminhoSaida);
    }

     public static void CallMerge(string source, string dest)
    {
      byte[] pdfOriginal = File.ReadAllBytes(source);
      byte[] pdfFinal = MergeWithExistingPdf(pdfOriginal);
      // Salva o resultado final com a nova página adicionada
      File.WriteAllBytes(dest, pdfFinal);
    }
    public static byte[] CallMerge(string source, string dest, List<byte[]> pages)
    {
      byte[] pdfOriginal = File.ReadAllBytes(source);
      byte[] pdfFinal = AddPagesIntoPdf(pdfOriginal, pages);
      // Salva o resultado final com a nova página adicionada
      File.WriteAllBytes(dest, pdfFinal);
      return pdfFinal;
    }
    public static byte[] AddPagesIntoPdf(byte[] existingPdfBytes, List<byte[]> pages)
    {
      using var outputStream = new MemoryStream();
      using var streamOriginal = new MemoryStream(existingPdfBytes);

      var docOriginal = PdfReader.Open(streamOriginal, PdfDocumentOpenMode.Modify);

      foreach (var pageBytes in pages)
      {
        using var streamNova = new MemoryStream(pageBytes);
        var docNova = PdfReader.Open(streamNova, PdfDocumentOpenMode.Import);

        for (int i = 0; i < docNova.PageCount; i++)
        {
          docOriginal.AddPage(docNova.Pages[i]);
        }
      }

      docOriginal.Save(outputStream);
      return outputStream.ToArray();
    }

    public static byte[] MergeWithExistingPdf(byte[] existingPdfBytes)
    {
      // 1. Gerar a nova página com QuestPDF em memória
      var questPdf = new UpScoreBeneficiario();
      byte[] novaPaginaBytes = questPdf.GeneratePdf();

      // 2. Abrir os dois documentos em memória
      using var outputStream = new MemoryStream();
      using var streamOriginal = new MemoryStream(existingPdfBytes);
      using var streamNovaPagina = new MemoryStream(novaPaginaBytes);

      var docOriginal = PdfReader.Open(streamOriginal, PdfDocumentOpenMode.Modify);
      var docNova = PdfReader.Open(streamNovaPagina, PdfDocumentOpenMode.Import);

      for (int i = 0; i < docNova.PageCount; i++)
      {
        docOriginal.AddPage(docNova.Pages[i]);
      }

      docOriginal.Save(outputStream);
      return outputStream.ToArray();
    }
  }
}
