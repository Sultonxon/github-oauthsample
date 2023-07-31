using System.ComponentModel.DataAnnotations;

namespace ServerApp.Models;

public class ExternalAuthModel
{
    [Required]
    public string AuthToken { get; set; }
}