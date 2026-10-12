using System.ComponentModel.DataAnnotations;

namespace ToDoApp.Models
{
    public class TaskModel
    {
        public int Id { get; set; }

        //[Display(Name = "Test display")]
        [Required(ErrorMessage ="Please enter a name")]
        public string? Name { get; set; }
        [Required(ErrorMessage ="Please choose a date")]
        public DateTime Date { get; set; }
        public Boolean Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int UserId { get; set; }
        public UserModel? User { get; set; }
    }
}
