public class EmployeeProfileDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public Guid EmployeeId { get; set; }

    public string? EmployeeName { get; set; }
    public string? ProfessionName { get; set; }

    public List<string> ZooAnimals { get; set; } = new();
}
