﻿namespace QuickTest.Data.Contracts.Requests.Get;

public class GetResultRequest {
    public string? WaferId { get; set; }
    public int MeasurementType { get; set; }
}