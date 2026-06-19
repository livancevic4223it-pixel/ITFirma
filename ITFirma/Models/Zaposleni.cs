using System.ComponentModel.DataAnnotations;

namespace ITFirma.Models
{
    public class Zaposleni
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime je obavezno.")]
        [MaxLength(50)]
        [Display(Name = "Ime")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [MaxLength(50)]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Neispravan format email adrese.")]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pozicija je obavezna.")]
        [MaxLength(100)]
        [Display(Name = "Pozicija")]
        public string Pozicija { get; set; } = string.Empty;

        // Computed
        [Display(Name = "Ime i prezime")]
        public string PunoIme => $"{Ime} {Prezime}";

        // Navigation
        public ICollection<Angazovanje> Angazovanja { get; set; } = new List<Angazovanje>();
    }
}
