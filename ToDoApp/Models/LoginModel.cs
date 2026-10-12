using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Password must contain 3-30 characters")]
        public string? Password { get; set; }
    }
}
