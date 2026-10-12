using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ToDoApp.Models;

public static class SeedUserData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ToDoAppContext>();
        var passwordHasher = serviceProvider.GetRequiredService<IPasswordHasher<UserModel>>();
        
        if (context.Users.Any())
        {
            return;
        }
        var users = new List<UserModel>
        {
            new UserModel
            {
                Name = "Nguyen Van A",
                Email = "nva@gmail.com",
                Password = "12345"
            },
            new UserModel
            {
                Name = "Nguyen Van B",
                Email = "nvb@gmail.com",
                Password = "12345"
            }
        };
        foreach(var user in users)
        {
            user.Password = passwordHasher.HashPassword(user, user.Password);
        }
        context.Users.AddRange(users);
        context.SaveChanges();
    }
}