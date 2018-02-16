namespace Delve.Tests.Models
{
    internal class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public UserRole(int id, int userId, int roleId)
        {
            Id = id;
            UserId = userId;
            RoleId = roleId;
        }

        public Role Role { get; set; }
    }
}
