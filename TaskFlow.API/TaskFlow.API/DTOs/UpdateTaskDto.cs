using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.DTOs;

public class UpdateTaskDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? DueDate { get; set; }

    public int CategoryId { get; set; }
}
