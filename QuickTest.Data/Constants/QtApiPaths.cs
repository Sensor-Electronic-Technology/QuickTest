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
        public static string GetInitialExcelResultsPath => "/api/results/initial/excel/";
        public static string GetFinalExcelResultsPath => "/api/results/final/excel/";
        public static string GetManyInitialExcelResultsPath => "/api/results/initial/excel/many";
        public static string GetManyFinalExcelResultsPath => "/api/results/final/excel/many";
        public static string GetQuickTestExistsPath => "/api/quick-test/exists/";
    #endregion

    #region InsertMeasurementsPaths
        public static string InsertInitialMeasurementPath=> "/api/results/initial/insert/";
        
        public static string InsertFinalMeasurementPath=> "/api/results/final/insert/";
        
        public static string InsertManyInitialMeasurementsPath=> "/api/results/initial/insert/many/";
        
        public static string InsertManyFinalMeasurementsPath=> "/api/results/final/insert/many/";

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

    public static string CreateQuickTestPath => "/api/create";

    public static string MarkInitialCompleted => "/api/results/initial/mark-completed/";
    public static string MarkFinalCompleted => "/api/results/final/mark-completed/";
    
}