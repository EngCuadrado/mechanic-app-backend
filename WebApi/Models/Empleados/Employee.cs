namespace WebApi.Models.Empleados
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int EmployeeRoleId { get; set; }
        public int EmployeeStatusId { get; set; }

        public string names { get; set; }
        public string lastnames { get; set; }
        public string phone { get; set; }
        public string user { get; set; }
        public string password { get; set; }

        public string? email { get; set; }

        public string? url_photo { get; set; }

        public DateTime hiring_date { get; set; }

        // Propiedades de navegación (relaciones)
        public EmployeeRoles EmployeeRole { get; set; }
        public EmployeeStatuses EmployeeStatus { get; set; }
    }
}