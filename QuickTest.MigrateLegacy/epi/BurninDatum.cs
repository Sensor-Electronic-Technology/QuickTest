using System;
using System.Collections.Generic;

namespace QuickTest.MigrateLegacy.epi;

public partial class BurninDatum
{
    public string WaferId { get; set; } = null!;

    public string? System { get; set; }

    public int? StationId { get; set; }

    public ulong? Running { get; set; }

    public ulong? Completed { get; set; }

    public double? AInitVolt { get; set; }

    public double? AInitCurrent { get; set; }

    public double? BInitVolt { get; set; }

    public double? BInitCurrent { get; set; }

    public double? CInitVolt { get; set; }

    public double? CInitCurrent { get; set; }

    public double? RInitVolt { get; set; }

    public double? RInitCurrent { get; set; }

    public double? LInitVolt { get; set; }

    public double? LInitCurrent { get; set; }

    public double? TInitVolt { get; set; }

    public double? TInitCurrent { get; set; }

    public double? BottomInitCurrent { get; set; }

    public double? BottomInitVolt { get; set; }

    public double? AFinalVolt { get; set; }

    public double? AFinalCurrent { get; set; }

    public double? BFinalVolt { get; set; }

    public double? BFinalCurrent { get; set; }

    public double? CFinalVolt { get; set; }

    public double? CFinalCurrent { get; set; }

    public double? RFinalVolt { get; set; }

    public double? RFinalCurrent { get; set; }

    public double? LFinalVolt { get; set; }

    public double? LFinalCurrent { get; set; }

    public double? TFinalVolt { get; set; }

    public double? TFinalCurrent { get; set; }

    public double? BottomFinalCurrent { get; set; }

    public double? BottomFinalVolt { get; set; }

    public int? APocket { get; set; }

    public int? ASetCurrent { get; set; }

    public int? ASetTemp { get; set; }

    public int? BPocket { get; set; }

    public int? BSetCurrent { get; set; }

    public int? BSetTemp { get; set; }

    public int? CPocket { get; set; }

    public int? CSetCurrent { get; set; }

    public int? CSetTemp { get; set; }

    public int? RPocket { get; set; }

    public int? RSetCurrent { get; set; }

    public int? RSetTemp { get; set; }

    public int? LPocket { get; set; }

    public int? LSetCurrent { get; set; }

    public int? LSetTemp { get; set; }

    public int? TPocket { get; set; }

    public int? TSetCurrent { get; set; }

    public int? TSetTemp { get; set; }

    public int? BottomPocket { get; set; }

    public int? BottomSetCurrent { get; set; }

    public int? BottomSetTemp { get; set; }

    public virtual ICollection<BurninLog> BurninLogWafer1Navigations { get; set; } = new List<BurninLog>();

    public virtual ICollection<BurninLog> BurninLogWafer2Navigations { get; set; } = new List<BurninLog>();

    public virtual ICollection<BurninLog> BurninLogWafer3Navigations { get; set; } = new List<BurninLog>();
}
