namespace KaznacheystvoCourse.DTO;

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public List<T> Items { get; set; }
}