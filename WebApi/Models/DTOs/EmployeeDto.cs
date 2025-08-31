namespace WebApi.Models.DTOs
{
    public class EmployeeStatusDto
    {
        public string name { get; set; }
    }

    public class UpdateEmployeeStatusDto
    {
        public string? name { get; set; }
        public bool? status { get; set; }
    }

    public class CreateEmployeeDto
    {
        public string names { get; set; }
        public string lastnames { get; set; }
        public string phone { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public string? email { get; set; }
        public string? url_photo { get; set; }
        public DateTime hiring_date { get; set; }

        // Opcionales: si no los envías o vienen 0/negativos, se usarán 1
        public int? EmployeeRoleId { get; set; }
        public int? EmployeeStatusId { get; set; }
    }

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string names { get; set; }
        public string lastnames { get; set; }
        public string phone { get; set; }
        public string user { get; set; }
        public string? email { get; set; }
        public string? url_photo { get; set; }
        public DateTime hiring_date { get; set; }
        public int EmployeeRoleId { get; set; }
        public int EmployeeStatusId { get; set; }

        // Opcional: mostrar info del rol y estado
        public string RoleName { get; set; }
        public string StatusName { get; set; }
    }

    public class UpdateEmployeeDto
    {
        public string? names { get; set; }
        public string? lastnames { get; set; }
        public string? phone { get; set; }
        public string? user { get; set; }
        public string? password { get; set; }        // opcional (recomendado hashear)
        public string? email { get; set; }
        public string? url_photo { get; set; }
        public DateTime? hiring_date { get; set; }
        public int? EmployeeRoleId { get; set; }     // si lo envías, se valida que exista
        public int? EmployeeStatusId { get; set; }   // si lo envías, se valida que exista
    }
}
