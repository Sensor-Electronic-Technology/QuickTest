namespace QuickTest.Data.Contracts.Responses.Get.Excel;

public class GetResultExcelResponse {
    public List<string>? Row { get; set; }
}

public class GetManyResultsExcelResponse {
    public List<List<string>>? Rows { get; set; }
}