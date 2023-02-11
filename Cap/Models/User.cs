using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Cap.Models;

[Index(nameof(ApiKey), IsUnique = true)]
public class User
{
    [Key]
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}