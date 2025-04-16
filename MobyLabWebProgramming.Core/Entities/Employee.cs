using System.ComponentModel.DataAnnotations.Schema;

namespace MobyLabWebProgramming.Core.Entities;

public class Employee
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public int Age { get; set; }

    //  Relația many-to-one cu profesia
    public Guid ProfessionId { get; set; }
    public Profession Profession { get; set; } = default!;

    //  Relația one-to-one cu profilul
    public EmployeeProfile Profile { get; set; } = default!;

    //  Relația many-to-many cu animalele
    public ICollection<EmployeeZooAnimal> EmployeeZooAnimals { get; set; } = new List<EmployeeZooAnimal>();
}
