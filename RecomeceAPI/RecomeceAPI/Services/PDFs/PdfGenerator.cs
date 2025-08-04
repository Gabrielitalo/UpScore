using System;
using System.IO;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace RecomeceAPI.Services.PDFs
{
  public class PdfGenerator
  {
    private readonly IConverter _converter;

    public PdfGenerator()
    {
      _converter = new SynchronizedConverter(new PdfTools());
    }

    public byte[] GerarPdf()
    {
      string html = @"
        <html>
        <head>
            <style>
                body { font-family: Arial, sans-serif; margin: 40px; }
                h1 { color: #2c3e50; }
                table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                th, td { border: 1px solid #ccc; padding: 10px; text-align: left; }
                th { background-color: #f5f5f5; }
            </style>
        </head>
        <body>
            <h1>Relatório de Vendas</h1>
            <p>Gerado em " + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + @"</p>
            <table>
                <tr><th>Produto</th><th>Valor</th></tr>
                <tr><td>Notebook Dell</td><td>R$ 3.000,00</td></tr>
                <tr><td>Monitor LG</td><td>R$ 1.200,00</td></tr>
                <tr><td>Mouse Logitech</td><td>R$ 150,00</td></tr>
            </table>
        </body>
        </html>";

      var doc = new HtmlToPdfDocument()
      {
        GlobalSettings = new GlobalSettings
        {
          PaperSize = PaperKind.A4,
          Orientation = Orientation.Portrait,
          Margins = new MarginSettings
          {
            Top = 10,
            Bottom = 10,
            Left = 10,
            Right = 10
          }
        },
        Objects = {
        new ObjectSettings
        {
            HtmlContent = html,
            WebSettings = { DefaultEncoding = "utf-8" }
        }
    }
      };


      byte[] pdf = _converter.Convert(doc);
      return pdf;
    }
  }
}
