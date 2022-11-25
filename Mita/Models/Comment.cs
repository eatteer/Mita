using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mita.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        [JsonIgnore]
        public int? ReviewId { get; set; }
        [JsonIgnore]
        public int? UserId { get; set; }

        public virtual Review? Review { get; set; }
        public virtual User? User { get; set; }
    }
}
