using Microsoft.EntityFrameworkCore;

public class ToDoAppContext(DbContextOptions<ToDoAppContext> options) : DbContext(options)
{
    public DbSet<ToDoApp.Models.TaskModel> Tasks { get; set; } = default!;
    public DbSet<ToDoApp.Models.UserModel> Users { get; set; } = default!;
}
