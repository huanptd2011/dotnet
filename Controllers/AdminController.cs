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

        // Trang dashboard (giao diện tổng quan)
        public IActionResult Dashboard()
        {
            return View();
        }
        
        public IActionResult Home()
        {
            return View();
        }
        // Trang quản lý bài viết của tác giả (Author)
[HttpGet]
public IActionResult Index()
{
    int? userId = GetUserId();
    if (userId == null)
        return RedirectToAction("AccessDenied", "User");
        var articles = _context.Articles.ToList();

    return View(articles);
}


        // API thống kê (Admin)
        [HttpGet]
        public IActionResult GetDashboardStats()
        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Unauthorized" });

            var stats = new
            {
                TotalArticles = _context.Articles.Count(),
                TotalAuthors = _context.Users.Count(u => u.Role == "Author"),
                PendingArticles = _context.Articles.Count(a => a.Status == "Pending"),
                ArticlesByStatus = new
                {
                    Approved = _context.Articles.Count(a => a.Status == "Approved"),
                    Pending = _context.Articles.Count(a => a.Status == "Pending"),
                    Rejected = _context.Articles.Count(a => a.Status == "Rejected")
                },
                ArticlesByTopic = _context.Articles
                    .GroupBy(a => a.Topic.Name)
                    .Select(g => new { TopicName = g.Key, Count = g.Count() })
                    .ToList(),
                ArticlesByAuthor = _context.Articles
                    .GroupBy(a => a.Author.FullName)
                    .Select(g => new { AuthorName = g.Key, Count = g.Count() })
                    .ToList()
            };

            return Json(new { success = true, data = stats });
        }

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
    // Statistics data based on Article model
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
