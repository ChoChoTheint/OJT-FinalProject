using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using finalproject.DTO;

namespace finalproject.models;

[Table("exercise")]
public class Exercise
{ 
    [Key]
    public int exercise_id { get; set; }

    [Required]
    public string exercise_no { get; set; }

    [Required]
    public string description { get; set; }

    [Required]
    public string exercise_content { get; set; }

    [NotMapped]
    public ExerciseResponseDTO ExerciseResponse { get; set; }
}

public class ExerciseResponseDTO
{
    public int exercise_id { get; set; }
    public string exercise_no { get; set; }
    public string description { get; set; }
    public string exercise_content { get; set; }
    public int pageSize { get; set; }
    public int pageCount { get; set; }
    public int pageNumber { get; set; }
}
