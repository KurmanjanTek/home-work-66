namespace home_work_66.ViewModels;

public class EditCompanyProfileViewModel
{
    public string CompanyName { get; set; }
    public string PhoneNumber { get; set; }
    public string? AvatarUrl { get; set; }
    public IFormFile? Avatar { get; set; }
}