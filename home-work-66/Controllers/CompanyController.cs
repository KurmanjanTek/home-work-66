using home_work_66.Models;
using home_work_66.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace home_work_66.Controllers;

public class CompanyController : Controller
{
    private readonly HeadHunterContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    
    public CompanyController(HeadHunterContext context, UserManager<User> userManager, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<IActionResult> Index()
    {
        var user=await _userManager.GetUserAsync(User);
        if(user==null)
            return NotFound();
        var vacancies = await _context.Vacancies.Where(v => v.CompanyId == user.Id).OrderByDescending(v => v.UpdatedAt)
            .ToListAsync();
       return View(vacancies);
    }

    [HttpGet]
    public async Task<IActionResult> CreateVacancy()
    {
        var categories =await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;
        return PartialView();
    }

    [HttpPost]
    public async Task<IActionResult> CreateVacancy(CreateVacancyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return PartialView(model);
        }
        var user=await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();
        var vacancy = new Vacancy
        {
            Title = model.Title,
            Salary = model.Salary,
            Description = model.Description,
            Requirements = model.Requirements,
            ExperienceFrom = model.ExperienceFrom,
            ExperienceTo = model.ExperienceTo,
            CategoryId = model.CategoryId,
            CompanyId = user.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsPublished = false
        };
        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync();
        
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditVacancy(int id)
    {
        var user=await _userManager.GetUserAsync(User);
        if(user == null)
            return NotFound();
        
        var vacancy = _context.Vacancies.FirstOrDefault(v=> v.Id == id && v.CompanyId == user.Id);
        
        if(vacancy==null)
            return NotFound();

        var model = new CreateVacancyViewModel
        {
            Title = vacancy.Title,
            Salary = vacancy.Salary,
            Description = vacancy.Description,
            Requirements = vacancy.Requirements,
            ExperienceFrom = vacancy.ExperienceFrom,
            ExperienceTo = vacancy.ExperienceTo,
            CategoryId = vacancy.CategoryId
        };
        
        ViewBag.Categories = await _context.Categories.ToListAsync();
        
        return PartialView(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditVacancy(int id, CreateVacancyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return PartialView(model);
        }
        var user =await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();
        var vacancy=await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == user.Id);

        if (vacancy == null)
            return NotFound();
        
        vacancy.Title = model.Title;
        vacancy.Salary = model.Salary;
        vacancy.Description = model.Description;
        vacancy.Requirements = model.Requirements;
        vacancy.ExperienceFrom = model.ExperienceFrom;
        vacancy.ExperienceTo = model.ExperienceTo;
        vacancy.CategoryId = model.CategoryId;
        vacancy.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UpdateVacancy(int id)
    {
        var user=await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();
        var vacancy=await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == user.Id);
        
        if(vacancy==null)
            return NotFound();
        vacancy.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var user=await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();
        var vacancy=await _context.Vacancies.FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == user.Id);
        if (vacancy == null)
            return NotFound();
        vacancy.IsPublished = !vacancy.IsPublished;
        vacancy.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var user=await _userManager.GetUserAsync(User);
        if (user==null)
            return NotFound();
        var model = new EditCompanyProfileViewModel()
        {
            CompanyName = user.CompanyName,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl = user.AvatarUrl
        };
        return PartialView(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile(EditCompanyProfileViewModel model)
    {
        var user =await _userManager.GetUserAsync(User);
        if (user == null)
            return NotFound();
        if (!ModelState.IsValid)
        {
            return PartialView(model);
        }
        user.CompanyName = model.CompanyName;
        user.PhoneNumber = model.PhoneNumber;
        
        if (model.Avatar != null)
        {
            var extension=Path.GetExtension(model.Avatar.FileName);
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".jfif" };
              
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} are allowed");
            }
            var wwwPath=_webHostEnvironment.WebRootPath;
            var path=Path.Combine(wwwPath, "uploads", "avatars");
            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
           var newFileName = $"{Guid.NewGuid()}{extension}";
            user.AvatarUrl = $"/uploads/avatars/{newFileName}";
            var fileWithPath=Path.Combine(path, newFileName);
            using var fileStream=new FileStream(fileWithPath, FileMode.Create);
            await model.Avatar.CopyToAsync(fileStream);
        }
        var result=await _userManager.UpdateAsync(user);
       return PartialView(model);
    }
}