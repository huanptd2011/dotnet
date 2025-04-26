using Microsoft.AspNetCore.Mvc;
using huan.Models;
using huan.Data;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace huan.Controllers // Ensure no 'public' here
{
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticleController(ApplicationDbContext context) => _context = context;
        // Index action to display list of articles
        [HttpGet]
        public IActionResult Index()
        {
            

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("AccessDenied", "User");

            var articles = _context.Articles
                .Where(a => a.AuthorId == userId.Value)
                .ToList();
            return View(articles);
        }
        // View article details
        [HttpGet]
        public IActionResult Details(int id)
        {
            Article article = null;
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("AccessDenied", "User");
            if( HttpContext.Session.GetString("Role") == "Author"){
                   article = _context.Articles.FirstOrDefault(a => a.Id == id && a.AuthorId == userId.Value);
            }else 
            article = _context.Articles.Include(a => a.Author).FirstOrDefault(a => a.Id == id);
            if (article == null)
                return RedirectToAction("AccessDenied", "User");

            return View(article);
        }
[HttpGet]
public IActionResult Create()
{
    var topics = _context.Topics?.ToList() ?? new List<Topic>();
    if (!topics.Any())
    {
        TempData["ArticleError"] = "Không có chủ đề nào để chọn. Vui lòng liên hệ quản trị viên.";
    }
    ViewBag.Topics = new SelectList(topics, "Id", "Name");
    if (TempData["ArticleError"] is string errorMsg)
        ViewBag.ErrorMessage = errorMsg;
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(Article article)
{
    var userId = HttpContext.Session.GetInt32("UserId");
    if (userId == null || HttpContext.Session.GetString("Role") != "Author")
        return RedirectToAction("AccessDenied", "User");

    article.AuthorId = userId.Value ;
    article.SubmissionDate = DateTime.Now;
    article.Status = "Pending";

    if (ModelState.IsValid)
    {
        var topics = _context.Topics?.ToList() ?? new List<Topic>();
        ViewBag.Topics = new SelectList(topics, "Id", "Name");
        return View(article);
    }
     

    try
    {
                Console.WriteLine($"Title: {article.Title}");
                Console.WriteLine($"TopicId: {article.TopicId}");

                // Hoặc dùng Debug.WriteLine
                System.Diagnostics.Debug.WriteLine($"Content: {article.Content}");
                _context.Articles.Add(article);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }
    catch (DbUpdateException ex)
    {
        TempData["ArticleError"] = "Lỗi khi lưu bài viết. Vui lòng kiểm tra lại dữ liệu.";
        // Log lỗi ex tại đây
        return RedirectToAction("Create");
    }
}
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("Role") != "Author")
                return RedirectToAction("AccessDenied", "User");
            var article = _context.Articles.FirstOrDefault(a => a.Id == id && a.AuthorId == userId.Value);
            if (article == null || article.Status != "Pending")
                return RedirectToAction("AccessDenied", "User");
            ViewBag.Topics = _context.Topics.ToList();
            return View(article);
        }

        [HttpPost]
        public IActionResult Edit(Article article)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("Role") != "Author")
                return RedirectToAction("AccessDenied", "User");

            var existingArticle = _context.Articles.FirstOrDefault(a => a.Id == article.Id && a.AuthorId == userId.Value);
            if (existingArticle == null || existingArticle.Status != "Pending")
                return RedirectToAction("AccessDenied", "User");

            if (ModelState.IsValid)
            {
                // Ensure Topics is always populated in case of form validation errors
                ViewBag.Topics = _context.Topics.ToList(); // This should never be null
                return View(article); // Ensure the model is passed back to the view
            }

            existingArticle.Title = article.Title;
            existingArticle.Summary = article.Summary;
            existingArticle.Content = article.Content;
            existingArticle.TopicId = article.TopicId;

            _context.Articles.Update(existingArticle);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Delete article (Authors only, if Pending)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || HttpContext.Session.GetString("Role") != "Author")
                return RedirectToAction("AccessDenied", "User");

            var article = _context.Articles.FirstOrDefault(a => a.Id == id && a.AuthorId == userId.Value);
            if (article == null || article.Status != "Pending")
                return RedirectToAction("AccessDenied", "User");

            _context.Articles.Remove(article);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}