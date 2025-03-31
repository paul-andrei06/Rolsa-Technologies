namespace RolsaTechnologies.Models
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; } // Id of the user (Foreign Key)
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; } // Email of the user
        public IEnumerable<string> Roles { get; set; } // Roles that can be assinged displayed as a list
    }
}
