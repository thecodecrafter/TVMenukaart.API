using System.Text.Json.Serialization;

namespace TVMenukaart.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [JsonIgnore]
        public MenuSection MenuSection { get; set; } = null!;

        public int MenuSectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
