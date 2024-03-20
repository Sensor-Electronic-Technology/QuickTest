using System;
using System.Collections.Generic;

namespace QuickTest.MigrateLegacy.epi;

public partial class BurninLog
{
    public string LogFileName { get; set; } = null!;

    public int? Stationid { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? StopTime { get; set; }

    public ulong? Running { get; set; }

    public ulong? Completed { get; set; }

    public int? SetCurrent { get; set; }

    public int? SetTemp { get; set; }

    public string? Wafer1 { get; set; }

    public string? W1A1 { get; set; }

    public string? W1A2 { get; set; }

    public string? Wafer2 { get; set; }

    public string? W2A1 { get; set; }

    public string? W2A2 { get; set; }

    public string? Wafer3 { get; set; }

    public string? W3A1 { get; set; }

    public string? W3A2 { get; set; }

    public virtual BurninDatum? Wafer1Navigation { get; set; }

    public virtual BurninDatum? Wafer2Navigation { get; set; }

    public virtual BurninDatum? Wafer3Navigation { get; set; }
}
