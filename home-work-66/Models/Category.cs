namespace home_work_66.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Vacancy> Vacancies { get; set; } = new List<Vacancy>();
}