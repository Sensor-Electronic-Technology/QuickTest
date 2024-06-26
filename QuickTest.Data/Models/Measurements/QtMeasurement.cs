﻿using MongoDB.Bson;
using QuickTest.Data.Models.Wafers.Enums;

namespace QuickTest.Data.Models.Measurements;

/*public class Measurement {
    public string? Pad { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public string? Current { get; set; }
    public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }
}*/

public abstract class Measurement {
    public ObjectId _id { get; set; }
    public string WaferId { get; set; }
    public ObjectId QuickTestResultId { get; set; }
    //public string? Pad { get; set; }
    public MeasurementType MeasurementType { get; set; }
    public int Current { get; set; }
}

public class QtMeasurement:Measurement {
    public Dictionary<string,PadMeasurement> Measurements { get; set; } = new Dictionary<string, PadMeasurement>();
    /*public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }*/
}

public class PadMeasurement {
    public string ActualPad { get; set; }
    public double Wl { get; set; }
    public double Power { get; set; }
    public double Voltage { get; set; }
    public double Knee { get; set; }
    public double Ir { get; set; }
}

