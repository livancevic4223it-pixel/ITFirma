using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITFirma.Models
{
    public class Angazovanje
    {
        // Composite PK — konfigurisan u OnModelCreating
        public int ProjekatId { get; set; }
        public int ZaposleniId { get; set; }

        [Required(ErrorMessage = "Uloga je obavezna.")]
        [MaxLength(50)]
        [Display(Name = "Uloga")]
        public string Uloga { get; set; } = string.Empty; // Dev / Lead / QA

        [Required(ErrorMessage = "Satnica je obavezna.")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 10000, ErrorMessage = "Satnica mora biti između 0 i 10000.")]
        [Display(Name = "Satnica (€/h)")]
        public decimal Satnica { get; set; }

        [Required(ErrorMessage = "Datum od je obavezan.")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum od")]
        public DateTime DatumOd { get; set; }

        // Navigation
        [ForeignKey(nameof(ProjekatId))]
        public Projekat? Projekat { get; set; }

        [ForeignKey(nameof(ZaposleniId))]
        public Zaposleni? Zaposleni { get; set; }
    }
}
