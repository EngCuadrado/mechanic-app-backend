namespace WebApi.Models.DTOs
{
    public class LoginDto
    {
        public string user { get; set; }
        public string password { get; set; }
    }

    public class LoginResponseDto
    {
        public string token { get; set; }
        public DateTime expiresAt { get; set; }

        // info útil del usuario:
        public int EmployeeId { get; set; }
        public string names { get; set; }
        public string lastnames { get; set; }
        public string username { get; set; }
        public int roleId { get; set; }
        public string roleName { get; set; }
        public int statusId { get; set; }
        public string statusName { get; set; }
    }
}
