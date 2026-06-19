using ITFirma.Models;

namespace ITFirma.ViewModels
{
    public class ProjekatIndexVM
    {
        public List<Projekat> Projekti { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 5;
        public string? Sort { get; set; }
        public string? Filter { get; set; }
        public int TotalCount { get; set; }
    }
}
