using System.ComponentModel.DataAnnotations;

namespace home_work_66.Models;

public class WorkExperience
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название компании")]
    public string CompanyName { get; set; } = "";

    [Required(ErrorMessage = "Введите должность")]
    public string Position { get; set; } = "";

    [Required(ErrorMessage = "Введите обязанности")]
    public string Responsibilities { get; set; } = "";

    [Range(1900, 2100, ErrorMessage = "Введите корректный год")]
    public int StartYear { get; set; }

    [Range(1900, 2100, ErrorMessage = "Введите корректный год")]
    public int EndYear { get; set; }

    public int ResumeId { get; set; }

    public Resume? Resume { get; set; }
}