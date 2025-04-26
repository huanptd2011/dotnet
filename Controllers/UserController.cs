using Microsoft.AspNetCore.Mvc;
using huan.Models;
using huan.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using BCrypt.Net;

namespace huan.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Register() => View();
        [HttpGet]
        public IActionResult Login() => View();
        [HttpGet]
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View(user);
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            user.Role = "Author";  // Mặc định là user
            user.Status = "active";
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && password == user.Password)
            {
                user.Status = "Active";
                _context.SaveChanges();
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("FullName", user.FullName);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetInt32("UserId", user.Id);

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Index", "Article");
            }

            ViewBag.Error = "Invalid login credentials";
            return View();
        }
        public IActionResult Logout()
        {
            try
            {
                int? userId = HttpContext.Session.GetInt32("UserId");
                if (userId.HasValue)
                {
                    var user = _context.Users.FirstOrDefault(u => u.Id == userId.Value);
                    if (user != null)
                    {
                        user.Status = "Inactive";
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during logout: " + ex.Message);
            }
            finally
            {
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login");
        }

    }
}