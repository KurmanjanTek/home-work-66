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
        
        if (!ModelState.IsValid)
        {
            return View(resume);
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
    
    [HttpGet]
    public IActionResult AddWorkExperience(int resumeId)
    {
        ViewBag.ResumeId = resumeId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddWorkExperience(WorkExperience workExperience)
    {
        int currentYear = DateTime.Now.Year;

        if (workExperience.StartYear < 1900 || workExperience.StartYear > currentYear)
        {
            ModelState.AddModelError("StartYear", $"Год должен быть от 1900 до {currentYear}");
        }

        if (workExperience.EndYear < 1900 || workExperience.EndYear > currentYear)
        {
            ModelState.AddModelError("EndYear", $"Год должен быть от 1900 до {currentYear}");
        }

        if (workExperience.EndYear < workExperience.StartYear)
        {
            ModelState.AddModelError("EndYear", "Год окончания не может быть раньше года начала");
        }
        
        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = workExperience.ResumeId;
            return View(workExperience);
        }

        _db.WorkExperiences.Add(workExperience);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }
    
    [HttpGet]
    public IActionResult AddEducation(int resumeId)
    {
        ViewBag.ResumeId = resumeId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddEducation(Education education)
    {
        int currentYear = DateTime.Now.Year;

        if (education.GraduationYear < 1900 || education.GraduationYear > currentYear)
        {
            ModelState.AddModelError("GraduationYear", $"Год должен быть от 1900 до {currentYear}");
        }
        
        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = education.ResumeId;
            return View(education);
        }

        _db.Educations.Add(education);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }
    
    [HttpGet]
    public IActionResult AddCourse(int resumeId)
    {
        ViewBag.ResumeId = resumeId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCourse(Course course)
    {
        int currentYear = DateTime.Now.Year;

        if (course.Year < 1900 || course.Year > currentYear)
        {
            ModelState.AddModelError("Year", $"Год должен быть от 1900 до {currentYear}");
        }
        
        if (!ModelState.IsValid)
        {
            ViewBag.ResumeId = course.ResumeId;
            return View(course);
        }

        _db.Courses.Add(course);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = course.ResumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditWorkExperience(int id)
    {
        WorkExperience? workExperience = await _db.WorkExperiences.FindAsync(id);

        if (workExperience == null)
        {
            return NotFound();
        }

        return View(workExperience);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditWorkExperience(WorkExperience workExperience)
    {
        if (!ModelState.IsValid)
        {
            return View(workExperience);
        }

        _db.WorkExperiences.Update(workExperience);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWorkExperience(int id)
    {
        WorkExperience? workExperience = await _db.WorkExperiences.FindAsync(id);

        if (workExperience == null)
        {
            return NotFound();
        }

        _db.WorkExperiences.Remove(workExperience);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }
    
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        User? currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        Resume? resume = await _db.Resumes
            .Include(r => r.WorkExperiences)
            .Include(r => r.Educations)
            .Include(r => r.Courses)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == currentUser.Id);

        if (resume == null)
        {
            return NotFound();
        }

        return View(resume);
    }
    
    [HttpGet]
    public async Task<IActionResult> EditEducation(int id)
    {
        Education? education = await _db.Educations.FindAsync(id);

        if (education == null)
        {
            return NotFound();
        }

        return View(education);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditEducation(Education education)
    {
        int currentYear = DateTime.Now.Year;

        if (education.GraduationYear < 1900 || education.GraduationYear > currentYear)
        {
            ModelState.AddModelError(
                "GraduationYear",
                $"Год должен быть от 1900 до {currentYear}"
            );
        }

        if (!ModelState.IsValid)
        {
            return View(education);
        }

        _db.Educations.Update(education);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = education.ResumeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteEducation(int id)
    {
        Education? education = await _db.Educations.FindAsync(id);

        if (education == null)
        {
            return NotFound();
        }

        int resumeId = education.ResumeId;

        _db.Educations.Remove(education);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = resumeId });
    }
    
    [HttpGet]
    public async Task<IActionResult> EditCourse(int id)
    {
        Course? course = await _db.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(Course course)
    {
        int currentYear = DateTime.Now.Year;

        if (course.Year < 1900 || course.Year > currentYear)
        {
            ModelState.AddModelError("Year", $"Год должен быть от 1900 до {currentYear}");
        }

        if (!ModelState.IsValid)
        {
            return View(course);
        }

        _db.Courses.Update(course);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = course.ResumeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        Course? course = await _db.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound();
        }

        int resumeId = course.ResumeId;

        _db.Courses.Remove(course);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", new { id = resumeId });
    }
}