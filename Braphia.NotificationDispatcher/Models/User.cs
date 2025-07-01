using System.ComponentModel.DataAnnotations;

namespace Braphia.NotificationDispatcher.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserTypeEnum UserType { get; set; }
        public string Email { get; set; }

        public User() { }

        public User(int rootId, string firstName, string lastName, UserTypeEnum userType, string email)
        {
            RootId = rootId;
            FirstName = firstName;
            LastName = lastName;
            UserType = userType;
            Email = email;
        }
    }
}
