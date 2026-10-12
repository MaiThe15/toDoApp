using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ToDoApp.Models;

public static class SeedTaskData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ToDoAppContext>();
            
        if (context.Tasks.Any())
        {
            return;
        }
        context.Tasks.AddRange(
            new TaskModel
            {
                Name = "MVC",
                Date = new DateTime(2026, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                Status = false,
                UserId = 1
            },
            new TaskModel
            {
                Name = ".Net",
                Date = new DateTime(2026, 10, 2, 0, 0, 0, DateTimeKind.Utc),
                Status = false,
                UserId = 1
            },
            new TaskModel
            {
                Name = "PgSql",
                Date = new DateTime(2026, 10, 5, 0, 0, 0, DateTimeKind.Utc),
                Status = false,
                UserId = 2
            });
        context.SaveChanges();
    }
}