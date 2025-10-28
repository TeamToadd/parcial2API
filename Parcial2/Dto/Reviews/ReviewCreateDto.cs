using System.ComponentModel.DataAnnotations;

namespace Parcial2.Dto.Reviews;

public class ReviewCreateDto
{
    [Required] public int ProductId { get; set; }
    [Range(1,5)] public int Rating { get; set; }
    public string? Comment { get; set; }
}