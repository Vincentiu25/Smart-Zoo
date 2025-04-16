using MobyLabWebProgramming.Core.Entities;

public class Profession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    // Relația one-to-many: o profesie are mulți angajați
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
