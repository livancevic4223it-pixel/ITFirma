using System.ComponentModel.DataAnnotations;
using ITFirma.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ITFirma.Models.Validation
{
    /// <summary>
    /// Provjerava da je Rok zadatka strogo nakon DatumPocetka projekta
    /// koji je odabran putem ProjekatId property-ja na istom objektu.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RokNakonPocetkaAttribute : ValidationAttribute
    {
        private readonly string _projekatIdProperty;

        public RokNakonPocetkaAttribute(string projekatIdProperty = "ProjekatId")
        {
            _projekatIdProperty = projekatIdProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext ctx)
        {
            if (value is not DateTime rok)
                return ValidationResult.Success; // Required već pokriva null

            // Dohvati ProjekatId sa istog objekta
            var projekatIdProp = ctx.ObjectType.GetProperty(_projekatIdProperty);
            if (projekatIdProp == null)
                return ValidationResult.Success;

            var projekatIdValue = projekatIdProp.GetValue(ctx.ObjectInstance);
            if (projekatIdValue is not int projekatId || projekatId == 0)
                return ValidationResult.Success; // Projekat nije odabran — Required će prijaviti grešku

            // Dohvati DatumPocetka iz baze
            var db = ctx.GetService<AppDbContext>();
            if (db == null)
                return ValidationResult.Success;

            var projekat = db.Projekti.Find(projekatId);
            if (projekat == null)
                return ValidationResult.Success;

            if (rok <= projekat.DatumPocetka)
            {
                return new ValidationResult(
                    $"Rok mora biti nakon datuma početka projekta ({projekat.DatumPocetka:dd.MM.yyyy}).");
            }

            return ValidationResult.Success;
        }
    }
}
