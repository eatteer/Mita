using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class UpdateCommentDTO
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
