using QuickTest.Data.Models.Measurements;

namespace QuickTest.Data.Contracts.Events;

public class UpdateEpiSystemEvent {
    public required string WaferId { get; set; }
    public required string ReferenceId { get; set; }
    public required MeasurementType MeasurementType { get; set; }
}