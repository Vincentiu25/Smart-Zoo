namespace MobyLabWebProgramming.Core.Entities;

public class EmployeeZooAnimal
{
    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;

    public Guid ZooAnimalId { get; set; }
    public ZooAnimal ZooAnimal { get; set; } = default!;
}
