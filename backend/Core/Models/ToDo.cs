using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class ToDo : BaseModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; } = false;

        // Relations
        public string Username { get; set; }
        public User User { get; set; }
    }
}
