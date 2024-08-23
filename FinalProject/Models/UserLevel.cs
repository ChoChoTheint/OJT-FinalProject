using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finalproject.models;

[Table("user_level")]
public class UserLevel
{ 
    [Key]
    public int userlevel_id { get; set; }
    [Required]
    [MaxLength(500)]
    public string userlevel_name { get; set; }

}