using System.ComponentModel.DataAnnotations;

namespace ITFirma.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Lozinka")]
        [MinLength(6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrda lozinke je obavezna.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrdi lozinku")]
        [Compare("Password", ErrorMessage = "Lozinke se ne poklapaju.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
