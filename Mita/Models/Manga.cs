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

        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
