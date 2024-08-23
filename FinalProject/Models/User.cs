using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace finalproject.models;


[Table("user")]
public class User
{
    internal UserLevel userlevel;

    [Key]
    public int user_id { get; set; }
    [Required]
    [MaxLength(500)]
    public string name { get; set; }

    [Required]
    [MaxLength(500)]
    public string password { get; set; }

    [Required]
    [MaxLength(500)]
    public string salt { get; set; }

    
    public int login_fail_count { get; set; }
    public bool is_lock { get; set; }

    [Required]
    [ForeignKey("user_level")]
    public int userlevel_id { get; set; }
    
}