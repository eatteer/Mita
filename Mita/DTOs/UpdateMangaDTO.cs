using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class UpdateMangaDTO
    {
        public string? Name { get; set; }
        [Url]
        public string? MalUri { get; set; }
        public string? Status { get; set; }
        public int? Chapters { get; set; }
        public int? Volumes { get; set; }
        public string? Synopsis { get; set; }
        public decimal? Score { get; set; }
    }
}
