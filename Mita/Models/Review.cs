using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mita.Models
{
    public partial class Review
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public int MangaId { get; set; }

        public virtual Manga Manga { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
