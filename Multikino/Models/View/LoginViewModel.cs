using System.ComponentModel.DataAnnotations;

namespace Multikino.Models.View
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Nazwa użytkownika")]
        public string UserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Hasło")]
        public string Password { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
