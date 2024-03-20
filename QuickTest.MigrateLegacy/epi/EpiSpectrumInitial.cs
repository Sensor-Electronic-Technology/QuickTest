using System;
using System.Collections.Generic;

namespace QuickTest.MigrateLegacy.epi;

public partial class EpiSpectrumInitial
{
    public string WaferId { get; set; } = null!;

    public DateTime? DateTime { get; set; }

    public string? System { get; set; }

    public ulong? Tested { get; set; }

    public string? CenterAWl { get; set; }

    public string? CenterASpect { get; set; }

    public string? CenterAWl50mA { get; set; }

    public string? CenterASpect50mA { get; set; }

    public string? CenterBWl { get; set; }

    public string? CenterBSpect { get; set; }

    public string? CenterBWl50mA { get; set; }

    public string? CenterBSpect50mA { get; set; }

    public string? CenterCWl { get; set; }

    public string? CenterCSpect { get; set; }

    public string? CenterCWl50mA { get; set; }

    public string? CenterCSpect50mA { get; set; }

    public string? CenterDWl { get; set; }

    public string? CenterDSpect { get; set; }

    public string? CenterDWl50mA { get; set; }

    public string? CenterDSpect50mA { get; set; }

    public string? RightWl { get; set; }

    public string? RightSpect { get; set; }

    public string? RightWl50mA { get; set; }

    public string? RightSpect50mA { get; set; }

    public string? TopWl { get; set; }

    public string? TopSpect { get; set; }

    public string? TopWl50mA { get; set; }

    public string? TopSpect50mA { get; set; }

    public string? LeftWl { get; set; }

    public string? LeftSpect { get; set; }

    public string? LeftWl50mA { get; set; }

    public string? LeftSpect50mA { get; set; }

    public string? BottomWl { get; set; }

    public string? BottomSpect { get; set; }

    public string? BottomWl50mA { get; set; }

    public string? BottomSpect50mA { get; set; }

    public int? StationId { get; set; }
}
