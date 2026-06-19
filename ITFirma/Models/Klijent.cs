using System.ComponentModel.DataAnnotations;

namespace ITFirma.Models
{
    public class Klijent
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [MaxLength(100)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [MaxLength(200)]
        [Display(Name = "Adresa")]
        public string? Adresa { get; set; }

        // Navigation
        public ICollection<Projekat> Projekti { get; set; } = new List<Projekat>();
    }
}
