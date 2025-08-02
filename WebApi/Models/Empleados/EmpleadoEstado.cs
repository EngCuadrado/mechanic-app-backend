namespace WebApi.Models.Empleados;

public class EmpleadoEstado
{
    public int id_empleado_estado { get; set; }

    public string estado { get; set; }
    
    // Propiedad de navegación inversa
    public ICollection<Empleado> Empleados { get; set; }
}