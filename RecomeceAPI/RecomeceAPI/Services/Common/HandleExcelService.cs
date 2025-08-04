using ClosedXML.Excel;
using RecomeceAPI.Interfaces;

namespace RecomeceAPI.Services.Common
{
  public class HandleExcelService
  {
    private readonly IFileService _fileService;

    public HandleExcelService(IFileService fileService)
    {
      _fileService = fileService;
    }
    public void GenerateExcel(dynamic dataList, string path, string name)
    {
      if (!_fileService.IsDirExists(path))
        _fileService.CreateDir(path);

      var workbook = new XLWorkbook(); //creates the workbook
      var wsDetailedData = workbook.AddWorksheet("data"); //creates the worksheet with sheetname 'data'
      wsDetailedData.Cell(1, 1).InsertTable(dataList); //inserts the data to cell A1 including default column name
      //workbook.SaveAs(@"C:\Makrosystem\data.xlsx"); //saves the workbook
      workbook.SaveAs(path + name); //saves the workbook
    }
  }
}
