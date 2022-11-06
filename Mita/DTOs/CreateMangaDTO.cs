using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class CreateMangaDTO
    {
        [Required]
        public string Name { get; set; }
        [Required, Url]
        public string MalUri { get; set; }
    }
}
