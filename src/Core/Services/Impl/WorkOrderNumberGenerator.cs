namespace ClearMeasure.Bootcamp.Core.Services.Impl;

public class WorkOrderNumberGenerator : IWorkOrderNumberGenerator
{
    public string GenerateNumber()
    {
        return Guid.NewGuid().ToString().Substring(8, 5).ToUpper();
    }
}