namespace QuickTest.Data.AppSettings;

public class DatabaseSettings {
    public string DatabaseName { get; set; }
    public string QuickTestCollectionName { get; set; }
    public string InitialMeasurementCollectionName { get; set; }
    public string FinalMeasurementCollectionName { get; set; }
    public string InitialSpectrumCollectionName { get; set; }
    public string FinalSpectrumCollectionName { get; set; }
    public string WaferPadCollectionName { get; set; }
    public string ProbeStationCollectionName { get; set; }
    public string EpiDataEndpoint { get; set; }
}