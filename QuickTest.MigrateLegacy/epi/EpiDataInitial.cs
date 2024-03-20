using System;
using System.Collections.Generic;

namespace QuickTest.MigrateLegacy.epi;

public partial class EpiDataInitial
{
    public string WaferId { get; set; } = null!;

    public DateTime? DateTime { get; set; }

    public string? System { get; set; }

    public int? StationId { get; set; }

    public ulong? Tested { get; set; }

    public double CenterAWl { get; set; }

    public double CenterAPower { get; set; }

    public double CenterAVolt { get; set; }

    public double CenterAKnee { get; set; }

    public double? CenterAReverse { get; set; }

    public double CenterBWl { get; set; }

    public double CenterBPower { get; set; }

    public double CenterBVolt { get; set; }

    public double CenterBKnee { get; set; }

    public double? CenterBReverse { get; set; }

    public double CenterCWl { get; set; }

    public double CenterCPower { get; set; }

    public double CenterCVolt { get; set; }

    public double CenterCKnee { get; set; }

    public double? CenterCReverse { get; set; }

    public double CenterDWl { get; set; }

    public double CenterDPower { get; set; }

    public double CenterDVolt { get; set; }

    public double CenterDKnee { get; set; }

    public double? CenterDReverse { get; set; }

    public double RightWl { get; set; }

    public double RightPower { get; set; }

    public double RightVolt { get; set; }

    public double RightKnee { get; set; }

    public double? RightReverse { get; set; }

    public double TopWl { get; set; }

    public double TopPower { get; set; }

    public double TopVolt { get; set; }

    public double TopKnee { get; set; }

    public double? TopReverse { get; set; }

    public double LeftWl { get; set; }

    public double LeftPower { get; set; }

    public double LeftVolt { get; set; }

    public double LeftKnee { get; set; }

    public double? LeftReverse { get; set; }

    public double BottomWl { get; set; }

    public double BottomPower { get; set; }

    public double BottomVolt { get; set; }

    public double BottomKnee { get; set; }

    public double? BottomReverse { get; set; }
}
