using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ITFirma.Models.Validation;

namespace ITFirma.Models
{
    public enum StatusZadatka
    {
        Otvoren,
        UToku,
        Zavrsen
    }

    public class Zadatak
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [MaxLength(150)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(500)]
        [Display(Name = "Opis")]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Rok je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Rok")]
        [RokNakonPocetka("ProjekatId")]
        public DateTime Rok { get; set; }

        [Display(Name = "Status")]
        public StatusZadatka Status { get; set; } = StatusZadatka.Otvoren;

        // FK
        [Required(ErrorMessage = "Projekat je obavezan.")]
        [Display(Name = "Projekat")]
        public int ProjekatId { get; set; }

        // Navigation
        [ForeignKey(nameof(ProjekatId))]
        public Projekat? Projekat { get; set; }
    }
}
