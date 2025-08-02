namespace WebApi.Models.Empleados;

public class Cargo
{
    public int id_cargo { get; set; }
    
    public string cargo { get; set; }
    
    // Propiedad de navegación inversa
    public ICollection<Empleado> Empleados { get; set; }
}