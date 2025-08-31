namespace WebApi.Models.Empleados;

public class EmployeeRoles
{
    public int EmployeeRoleId { get; set; }
    
    public string name { get; set; }

    public Boolean status { get; set; }
    
    // Propiedad de navegación inversa
    public ICollection<Employee> Employees { get; set; }
}