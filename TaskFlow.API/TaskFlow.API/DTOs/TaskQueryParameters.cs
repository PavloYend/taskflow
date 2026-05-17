namespace TaskFlow.API.DTOs;

public class TaskQueryParameters
{
    private const int MaxPageSize = 20;

    public int PageNumber { get; set; } = 1;

    private int _pageSize = 5;

    public int PageSize
    {
        get => _pageSize;

        set
        {
            _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

    public string? SearchTerm { get; set; }

    public bool? IsCompleted { get; set; }
}
