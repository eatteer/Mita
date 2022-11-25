using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class CreateMangaDTO
    {
        [Required]
        public string Name { get; set; }
        [Required, Url]
        public string MalUri { get; set; }
        [Required]
        public string Status { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Chapters { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int Volumes { get; set; }
        [Required]
        public string Synopsis { get; set; }
        [Required, Range(0, double.MaxValue)]
        public decimal Score { get; set; }
    }
}
