namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ZooAnimalDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Species { get; set; } = default!;
    public int Age { get; set; }
    public bool IsEndangered { get; set; }
}
