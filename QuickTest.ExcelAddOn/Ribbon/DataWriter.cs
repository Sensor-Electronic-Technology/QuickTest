using System.Net.Http.Json;
using ExcelDna.Integration;
using Microsoft.AspNetCore.Http;
using Microsoft.Office.Interop.Excel;
using QuickTest.Data.Constants;
using QuickTest.Data.Contracts.Requests.Get;
using Application = Microsoft.Office.Interop.Excel.Application;
using Range=Microsoft.Office.Interop.Excel.Range;
namespace QuickTest.ExcelAddOn.Ribbon;

public class DataWriter {
    public static void WriteData() {
        HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://172.20.4.206/");
        
        Application xlApp = (Application)ExcelDnaUtil.Application;
        Workbook wb=xlApp.ActiveWorkbook;
        if (wb == null) {
            return;
        }

        Worksheet ws = wb.ActiveSheet;
        if (ws == null) {
            return;
        }
        
        Range? range=xlApp.Selection as Range;
        if(range==null) {
            return;
        }

        
        var result=client.GetFromJsonAsync<GetResultRequest>($"{QtApiPaths.GetInitialResultsPath}B01-0000-00?0").Result;
        
        for (int i = 0; i < range.Rows.Count; i++) {
            for(int j = 0; j < range.Columns.Count; j++) {
                range.Cells[i + 1, j + 1].Value2 = i + j;
            }
        }

    }
}