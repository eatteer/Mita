using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required, Range(0, int.MaxValue)]
        public int MangaId { get; set; }
    }
}
