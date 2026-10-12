using home_work_66.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace home_work_66.Controllers;

[Authorize(Roles = "user")]
public class ResumeController : Controller
{
    private readonly HeadHunterContext _db;
    private readonly UserManager<User> _userManager;

    public ResumeController(HeadHunterContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Resume resume)
    {
        User? currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        resume.UserId = currentUser.Id;
        resume.UpdatedAt = DateTime.UtcNow;
        resume.IsPublished = false;

        _db.Resumes.Add(resume);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDate(int id)
    {
        User? currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        Resume? resume = await _db.Resumes.FirstOrDefaultAsync(r => r.Id == id && r.UserId == currentUser.Id);

        if (resume == null)
        {
            return NotFound();
        }

        resume.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }
}