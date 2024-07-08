namespace QuickTest.Data.Constants;

public static class QtApiPaths {
    
    #region WaferPadPaths
        public static string GetMapPath => "/api/map/";
        public static string GetLabviewWaferMap => "/api/labview/map/";
        public static string CreateWaferPadPath => "/api/pads/create/";
        public static string GetAvailableBurnInPadsPath => "/api/pads/available/";
    #endregion
    
    #region GetResultsPaths
        public static string GetQuickTestExcelResultsPath => "api/results/all/excel/";
        public static string GetManyQuickTestExcelResultsPath => "api/results/all/excel/many/";
        public static string GetExcelResultPath => "/api/results/excel/";
        public static string GetLabviewResultsPath=>"/api/results/labview/";
        public static string GetManyExcelResultsPath => "/api/results/excel/many";
        public static string GetQuickTestExistsPath => "/api/quick-test/exists/";
        public static string GetInitialResultsPath => "/api/results/initial/";
    #endregion
    
    
    public static string CreateQuickTestPath => "/api/quick-tests/create/";
    public static string CheckQuickTestPath => "/api/quick-tests/check/";
    public static string MarkTestedPath => "/api/quick-tests/mark-tested/";
    public static string InsertMeasurementPath => "/api/results/insert/";
    
    public static string DeleteQuickTestPath => "/api/quick-tests/delete/";
    
    public static string GetQuickTestListSincePath => "/api/quick-tests/list-since/";
    
}