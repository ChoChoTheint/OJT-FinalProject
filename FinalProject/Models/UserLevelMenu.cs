namespace finalproject.Models;
[System.ComponentModel.DataAnnotations.Schema.Table("user_level_menu")]
public class UserLevelMenu
{
    public int userlevel_id { get; set; }
    public required string endpoint { get; set; }
}