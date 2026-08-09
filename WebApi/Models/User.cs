namespace WebApi.Models
{
    public class User
    {
        public int userId { get; set; }
        
        public string name { get; set; }

        public string lastName { get; set; }

        public string email { get; set; }
        
        public string passwordHash { get; set; }

        public string role { get; set; }

        public Boolean isActive { get; set; }

    }
}
/*
     CREATE TABLE [users] (
      [user_id] INT IDENTITY(1,1) PRIMARY KEY,
      [email] nvarchar(255) UNIQUE,
      [password_hash] nvarchar(255),
      [role] nvarchar(255),
      [is_active] BIT DEFAULT 1
    )
    GO 
*/