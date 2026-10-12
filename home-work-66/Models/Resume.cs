using System.ComponentModel.DataAnnotations;

namespace home_work_66.Models;

public class Resume
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public bool IsPublished { get; set; }

    public int  UserId { get; set; } 

    public User? User { get; set; }
}