namespace WebApi.Models.Empleados
{
    public class Empleado
    {
        public int id_empleado { get; set; }
        public int id_cargo { get; set; }
        public int id_empleado_estado { get; set; }

        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string telefono { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public DateTime fecha_contratacion { get; set; }

        // Propiedades de navegación (relaciones)
        public Cargo Cargo { get; set; }
        public EmpleadoEstado EmpleadoEstado { get; set; }
    }
}