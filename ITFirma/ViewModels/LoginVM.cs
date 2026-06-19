using System.ComponentModel.DataAnnotations;

namespace ITFirma.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamti me")]
        public bool RememberMe { get; set; }
    }
}
