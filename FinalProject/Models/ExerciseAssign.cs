using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finalproject.models;

[Table("exercise_assign")]
public class ExerciseAssign
{ 
    [Key]
    public int exercise_assign_id { get; set; }

    [Required]
    [ForeignKey("exercise")]
    public int exercise_id { get; set; }

    [Required]
    [ForeignKey("user")]
    public int user_id { get; set; }

    [Required]
    public int mark { get; set; }

}