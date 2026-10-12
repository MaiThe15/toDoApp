using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please enter your name")]
        [StringLength(30, MinimumLength = 3, ErrorMessage ="Name must contain 3-30 characters")]
        [RegularExpression(@"^[\p{L}\s'-]+$", ErrorMessage ="Name must not contain number or special character")]
        public string? Name { get; set; }
        [Required(ErrorMessage ="Please enter your email")]
        [EmailAddress(ErrorMessage ="Please enter a valid email")]
        public string? Email { get; set; }
        [Required(ErrorMessage ="Please enter password")]
        [StringLength(255, MinimumLength = 3, ErrorMessage ="Password must contain 3-30 characters")]
        public string? Password { get; set; }
        [NotMapped]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public ICollection<TaskModel>? Tasks { get; set; }
    }
}
