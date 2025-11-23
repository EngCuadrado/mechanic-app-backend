namespace WebApi.GraphQL.Inputs
{
    // -- Employee --
    /// <summary>
    /// Input para CREAR un empleado.
    /// Los campos obligatorios no son anulables (string, int).
    /// </summary>
    public record AddEmployeeInput(
        string Names,
        string Lastnames,
        string User,
        string Password,
        DateTime Hiring_date,
        int EmployeeRoleId,
        int EmployeeStatusId,

        // Opcionales
        string? Phone,
        string? Email,
        string? Url_photo
    );

    /// <summary>
    /// Input para ACTUALIZAR un empleado.
    /// El ID es obligatorio. Todo lo demás es opcional (string?, int?)
    /// para permitir actualizaciones parciales.
    /// </summary>
    public record UpdateEmployeeInput(
        int EmployeeId, // Obligatorio para saber a quién actualizar

        string? Names,
        string? Lastnames,
        string? User,
        string? Password,
        DateTime? Hiring_date,
        int? EmployeeRoleId,
        int? EmployeeStatusId,
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
