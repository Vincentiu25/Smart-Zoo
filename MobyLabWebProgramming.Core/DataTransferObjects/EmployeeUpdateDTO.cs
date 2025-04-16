public class EmployeeUpdateDTO
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = default!;
    public int Age { get; set; }
    public Guid ProfessionId { get; set; }
}
