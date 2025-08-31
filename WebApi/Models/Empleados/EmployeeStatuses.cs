namespace WebApi.Models.Empleados;

public class EmployeeStatuses
{
    public int EmployeeStatusId { get; set; }

    public string name { get; set; }

    public bool status { get; set; } = true;

    // Propiedad de navegación inversa
    public ICollection<Employee> Employees { get; set; }
}