using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;

public class AuthController : Controller
{
    private readonly ToDoAppContext _context;
    private readonly IPasswordHasher<UserModel> _passwordHasher;

    public AuthController(ToDoAppContext context, IPasswordHasher<UserModel> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel user)
    {
        if (ModelState.IsValid)
        {
            var check = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);
            if(check == null)
            {
                ModelState.AddModelError("Password", "Wrong email or password");
                return View(user);
            }
            var verify = _passwordHasher.VerifyHashedPassword(check, check.Password, user.Password);
            if (verify == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Password", "Wrong email or password");
                return View(user);
            }
            else
            {
                HttpContext.Session.SetInt32("curUserId", check.Id);
                return RedirectToAction("Index", "Tasks");
            }
        }
        return View(user);
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([Bind("Id, Name, Email, Password, ConfirmPassword")] UserModel user)
    {
        var check = await _context.Users.FirstOrDefaultAsync(m => m.Email == user.Email);
        if(check != null)
        {
            ModelState.AddModelError("Email", "Email had been used");
        }
        if (ModelState.IsValid)
        {
            user.Password = _passwordHasher.HashPassword(user, user.Password);
            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        return View(user);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
