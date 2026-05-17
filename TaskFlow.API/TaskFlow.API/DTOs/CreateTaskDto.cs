using System.ComponentModel.DataAnnotations;

namespace TaskFlow.API.DTOs;

public class CreateTaskDto
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public DateTime? DueDate { get; set; }

    public int CategoryId { get; set; }
}
