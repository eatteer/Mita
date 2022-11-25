using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class CreateCommentDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int ReviewId { get; set; }
    }
}
