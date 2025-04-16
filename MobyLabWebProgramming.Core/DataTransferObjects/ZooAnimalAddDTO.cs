namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// DTO pentru adăugarea unui animal nou în grădina zoologică.
/// </summary>
public class ZooAnimalAddDTO
{
    public string Name { get; set; } = default!;
    public Guid SpeciesId { get; set; }  
    public int Age { get; set; }
    public bool IsEndangered { get; set; }
}
