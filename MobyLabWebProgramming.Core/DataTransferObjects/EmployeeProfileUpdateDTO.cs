public class EmployeeProfileUpdateDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public Guid EmployeeId { get; set; }
}
