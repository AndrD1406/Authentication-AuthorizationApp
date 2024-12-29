using System.ComponentModel.DataAnnotations;
using TaskAuthenticationAuthorization.Models;

namespace TaskAuthenticationAuthorization.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public Discount Discount { get; set; }
    }
}
