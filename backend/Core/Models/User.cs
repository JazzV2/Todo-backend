using System.ComponentModel.DataAnnotations;

namespace backend.Core.Models
{
    public class User : BaseModel
    {
        [Key]
        public string Username { get; set; }
        [Required]
        public string HashPassword { get; set; }
        [Required]
        public string Email { get; set; }

        // Relations
        public ICollection<ToDo> ToDos { get; set; }
    }
}
