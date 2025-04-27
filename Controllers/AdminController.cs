using Microsoft.AspNetCore.Mvc;
using huan.Models;
using huan.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace huan.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard() => View();


        public IActionResult Home() => View();

        [HttpGet]
        public IActionResult Index()
        {
            int? userId = GetUserId();
            if (userId == null)
                return RedirectToAction("AccessDenied", "User");
            var articles = _context.Articles.ToList();

            return View(articles);
        }


        [HttpGet]
        public IActionResult GetDashboardStats()  => View();

        // Duyệt bài viết
        [HttpPost]
        public IActionResult Approve(int id)
        {
            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return NotFound();
            article.Status = "Approved";
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Từ chối bài viết
        [HttpPost]
        public IActionResult Reject(int id)
        {


            var article = _context.Articles.FirstOrDefault(a => a.Id == id);
            if (article == null)
                return NotFound();

            article.Status = "Rejected";
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Trang thống kê chi tiết
        [HttpGet]
        public IActionResult Statistics()
        {
            var stats = new
            {
                ArticlesByAuthor = _context.Articles
                    .GroupBy(a => a.Author.FullName)
                    .Select(g => new
                    {
                        Author = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),
                ArticlesByTopic = _context.Articles
                    .GroupBy(a => a.Topic.Id)
                    .Select(g => new
                    {
                        TopicId = g.Key,
                        Count = g.Count()
                    })
                    .ToList(),
                ArticlesByStatus = new
                {
                    Approved = _context.Articles.Count(a => a.Status == "Approved"),
                    Pending = _context.Articles.Count(a => a.Status == "Pending"),
                    Rejected = _context.Articles.Count(a => a.Status == "Rejected")
                }
            };

            return View(stats);
        }


        // ======= Helpers =======
        private int? GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }
    }
}
