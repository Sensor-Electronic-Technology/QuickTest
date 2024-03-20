// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using QuickTest.Data.Measurements;
using QuickTest.Data.Wafer;
using QuickTest.MigrateLegacy.epi;
using System.ComponentModel;
using System.Text.Json;
EpiContext context = new();
/*await context.EpiDataInitials.LoadAsync();*/
var initialData = await context.EpiDataInitials.FirstOrDefaultAsync(e=>e.WaferId == "B03-2542-05");

QuickTestResult result = new QuickTestResult();
result.InitialMeasurements = new List<Measurement>();
if(initialData !=null) {
    var properties = TypeDescriptor.GetProperties(initialData);
    foreach (PropertyDescriptor property in properties) {
        if(property.Name.Contains("CenterA")) {
            
        }else if (property.Name.Contains("CenterAPower")) {
            if (initialData.CenterAPower != 0) {
                result.InitialMeasurements.Add(new Measurement() {
                    MeasurementType = MeasurementType.Initial,
                    Current="20mA",
                    Power = initialData.CenterAPower,
                    Voltage = initialData.CenterAVolt,
                    Wl = initialData.CenterAWl,
                    Ir=initialData.CenterAReverse ?? 0,
                    Knee=initialData.CenterAKnee
                });
            }
            
        }else if (property.Name.Contains("CenterBPower")) {
            if (initialData.CenterBPower != 0) {
                result.InitialMeasurements.Add(new Measurement() {
                    MeasurementType = MeasurementType.Initial,
                    Pad = PadLocation.PadLocationB.Name,
                    Current="20mA",
                    Power = initialData.CenterBPower,
                    Voltage = initialData.CenterBVolt,
                    Wl = initialData.CenterBWl,
                    Ir=initialData.CenterBReverse ?? 0,
                    Knee=initialData.CenterBKnee
                });
            }
            
        }else if (property.Name.Contains("CenterCPower")) {
            if (initialData.CenterCPower != 0) {
                result.InitialMeasurements.Add(new Measurement() {
                    MeasurementType = MeasurementType.Initial,
                    Pad = PadLocation.PadLocationC.Name,
                    Current="20mA",
                    Power = initialData.CenterCPower,
                    Voltage = initialData.CenterCVolt,
                    Wl = initialData.CenterCWl,
                    Ir=initialData.CenterCReverse ?? 0,
                    Knee=initialData.CenterCKnee
                });
            }
            
        } else if (property.Name.Contains("CenterDPower")) {
            
        }else if (property.Name.Contains("TopPower")) {
            
        }else if (property.Name.Contains("LeftPower")) {
            
        }else if (property.Name.Contains("BottomPower")) {
            
        }else if (property.Name.Contains("RightPower"))  {
            
        }
        
        string propertyName = property.Name;
        object propertyValue = property.GetValue(initialData);
        Console.WriteLine($"{propertyName}: {propertyValue}");
    }
}


Console.WriteLine($"Wafer Count: {context.EpiDataInitials.Count()}");

var json=JsonSerializer.Serialize(initialData, new JsonSerializerOptions() {
    WriteIndented = true
});
Console.WriteLine($"B03-2542-05 initial data:\n {json}");



