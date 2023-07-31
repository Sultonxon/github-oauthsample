using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models;

public class AuthModel
{
    [Required]
    public string UserName { get; set; }
    
    [Required]
    public string Password { get; set; }
}