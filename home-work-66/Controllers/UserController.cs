using home_work_66.Models;
using home_work_66.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace home_work_66.Controllers;

[Authorize(Roles = "user")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _environment;
    private readonly HeadHunterContext _db;

    public UserController(UserManager<User> userManager, IWebHostEnvironment environment, HeadHunterContext db)
    {
        _userManager = userManager;
        _environment = environment;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        User? currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        List<Resume> resumes = await _db.Resumes
            .Where(resume => resume.UserId == currentUser.Id)
            .OrderByDescending(resume => resume.UpdatedAt)
            .ToListAsync();

        ViewBag.Resumes = resumes;

        return View(currentUser);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        User? currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Account");
        }

        currentUser.UserName = model.UserName;
        currentUser.Email = model.Email;
        currentUser.PhoneNumber = model.PhoneNumber;

        if (model.Avatar != null && model.Avatar.Length > 0)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
            
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.Avatar.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            await using FileStream fileStream = new FileStream(filePath, FileMode.Create);
            await model.Avatar.CopyToAsync(fileStream);

            currentUser.AvatarUrl = $"/uploads/avatars/{uniqueFileName}";
        }

        IdentityResult result = await _userManager.UpdateAsync(currentUser);

        if (!result.Succeeded)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.OpenEditProfileModal = true;
            return View("Index", currentUser);
        }

        return RedirectToAction("Index");
    }
}