namespace MobyLabWebProgramming.Core.Entities;

public class ZooAnimal
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public bool IsEndangered { get; set; }

    //  Relația many-to-one cu Species
    public Guid SpeciesId { get; set; }
    public Species? Species { get; set; }

    //  Relația many-to-many cu Employee
    public ICollection<EmployeeZooAnimal> EmployeeZooAnimals { get; set; } = new List<EmployeeZooAnimal>();
}
