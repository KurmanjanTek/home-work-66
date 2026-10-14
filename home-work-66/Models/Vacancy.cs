namespace home_work_66.Models;

public class Vacancy
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Salary { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public int ExperienceFrom { get; set; }
    public int ExperienceTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; }
    public int CompanyId { get; set; }
    public User Company { get; set; } = null;
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null;

}