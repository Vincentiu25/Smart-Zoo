public class EmployeeDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public int Age { get; set; }
    public string ProfessionName { get; set; } = default!; // Pentru afișare
}
