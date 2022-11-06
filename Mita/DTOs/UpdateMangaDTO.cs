using System.ComponentModel.DataAnnotations;

namespace Mita.DTOs
{
    public class UpdateMangaDTO
    {
        public string? Name { get; set; }
        [Url]
        public string? MalUri { get; set; }
    }
}
