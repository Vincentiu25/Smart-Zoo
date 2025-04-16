namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class EmployeeZooAnimalDTO
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;

    public string ProfessionName { get; set; } = string.Empty;

    public Guid ZooAnimalId { get; set; }
    public string AnimalName { get; set; } = string.Empty;

    public string SpeciesName { get; set; } = string.Empty;
}
