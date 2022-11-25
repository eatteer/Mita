using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Mita.Models
{
    public partial class User
    {
        public User()
        {
            Comments = new HashSet<Comment>();
            Reviews = new HashSet<Review>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        [JsonIgnore]
        public string PasswordHash { get; set; } = null!;
        [JsonIgnore]
        public string PasswordSalt { get; set; } = null!;
        public string Role { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<Comment> Comments { get; set; }
        [JsonIgnore]
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
