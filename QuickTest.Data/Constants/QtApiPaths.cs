namespace QuickTest.Data.Constants;

public static class QtApiPaths {
    
    #region WaferPadPaths
        public static string GetMapPath => "/api/map/";
        public static string CreateWaferPadPath => "/api/pads/create/";
        public static string GetAvailableBurnInPadsPath => "/api/pads/available/";
    #endregion
    
    #region GetExcelResultsPaths
        public static string GetQuickTestExcelResultsPath => "api/results/all/excel/";
        public static string GetManyQuickTestExcelResultsPath => "api/results/all/excel/many/";
        public static string GetExcelResultPath => "/api/results/final/excel/";
        public static string GetManyExcelResultsPath => "/api/results/initial/excel/many";
        public static string GetQuickTestExistsPath => "/api/quick-test/exists/";
    #endregion
    
    #region GetResultsPath
        public static string GetInitialResultsPath => "/api/results/initial/";
        public static string GetFinalResultsPath => "/api/results/final/";
        public static string GetManyInitialResultsPath => "/api/results/initial/many";
        public static string GetManyFinalResultsPath => "/api/results/final/many/";
        public static string GetQuickTestResultsPath => "/api/results/all/";
        public static string GetManyQuickTestResultsPath => "/api/results/all/many/";
    #endregion
    
    public static string GetQuickTestsPath => "/api/quick-tests/";
    public static string CreateQuickTestPath => "/api/quick-tests/create/";
    public static string CheckQuickTestPath => "/api/quick-tests/check/";
    public static string InsertMeasurementPath => "/api/results/insert/";
    public static string MarkInitialCompleted => "/api/results/initial/mark-completed/";
    public static string MarkFinalCompleted => "/api/results/final/mark-completed/";
    
}