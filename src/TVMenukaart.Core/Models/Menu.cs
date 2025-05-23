using System.Text.Json.Serialization;

namespace TVMenukaart.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PublicUrl { get; set; }
        public List<MenuSection> MenuSections { get; set; } = [];
        public BoardPhoto BoardPhoto { get; set; }
        public int AppUserId { get; set; }
        [JsonIgnore]
        public AppUser AppUser { get; set; } = null!;
        public int RestaurantId { get; set; }
        [JsonIgnore]
        public Restaurant Restaurant { get; set; } = null!;
    }
}
