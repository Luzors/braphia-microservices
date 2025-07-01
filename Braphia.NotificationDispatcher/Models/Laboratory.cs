using System.ComponentModel.DataAnnotations;

namespace Braphia.NotificationDispatcher.Models
{
    public class Laboratory
    {
        [Key]
        public int Id { get; set; }
        public int RootId { get; set; }
        public string LaboratoryName { get; set; }
        public string Email { get; set; }

        public Laboratory() { }

        public Laboratory(int rootId, string laboratoryName, string email)
        {
            RootId = rootId;
            LaboratoryName = laboratoryName;
            Email = email;
        }
    }
}
