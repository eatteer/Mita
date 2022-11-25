using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mita.Models
{
    public partial class Manga
    {
        public Manga()
        {
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string MalUri { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Chapters { get; set; }
        public int Volumes { get; set; }
        public string Synopsis { get; set; } = null!;
        public decimal Score { get; set; }
        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
