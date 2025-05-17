namespace KaznacheystvoCourse.DTO;

public class QueryObject
{
    public int PageNumber { get; set; } = 1;
            
    public int PageSize { get; set; } = 25;
            
    public string? Search { get; set; } = string.Empty;
}