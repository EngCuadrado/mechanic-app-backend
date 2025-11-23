namespace WebApi.GraphQL.Inputs
{
    // -- Employee --
    public record EmployeeInput(
        int? EmployeeId,
        string Names,
        string Lastnames,
        string User,
        string Password,
        DateTime Hiring_date,
        int EmployeeRoleId,
        int EmployeeStatusId,
        // Campos opcionales (pueden ser null)
        string? Phone,
        string? Email,
        string? Url_photo
    );

    // --- ROLE ---
    public record CreateRoleInput(string Name);

    public record UpdateRoleInput(int Id, string? Name, bool? Status);

    // --- STATUS ---
    public record CreateStatusInput(string Name);

    public record UpdateStatusInput(int Id, string? Name, bool? Status);
}
