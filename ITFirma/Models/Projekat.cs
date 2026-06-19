using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITFirma.Models
{
    public enum StatusProjekta
    {
        Planiran,
        UToku,
        Zavrsen,
        Otkazan
    }

    public class Projekat
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [MaxLength(150)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Opis")]
        public string? Opis { get; set; }

        [Required(ErrorMessage = "Datum početka je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum početka")]
        public DateTime DatumPocetka { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Datum završetka")]
        public DateTime? DatumZavrsetka { get; set; }

        [Display(Name = "Status")]
        public StatusProjekta Status { get; set; } = StatusProjekta.Planiran;

        // FK
        [Required(ErrorMessage = "Klijent je obavezan.")]
        [Display(Name = "Klijent")]
        public int KlijentId { get; set; }

        // Navigation
        [ForeignKey(nameof(KlijentId))]
        public Klijent? Klijent { get; set; }

        public ICollection<Zadatak> Zadaci { get; set; } = new List<Zadatak>();
        public ICollection<Angazovanje> Angazovanja { get; set; } = new List<Angazovanje>();
    }
}
