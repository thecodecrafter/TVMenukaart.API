using System.Text.Json.Serialization;

namespace TVMenukaart.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int AppUserId { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; } = null!;
        public ICollection<Menu> Menus { get; set; } = [];
    }
}
