using System.ComponentModel.DataAnnotations;

namespace Braphia.NotificationDispatcher.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public Pharmacy() { }

        public Pharmacy(int rootId, string name, string email)
        {
            RootId = rootId;
            Name = name;
            Email = email;
        }
    }
}
