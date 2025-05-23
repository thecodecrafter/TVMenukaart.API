using System.Text.Json.Serialization;

namespace TVMenukaart.Models
{
    public class MenuSection
    {
        public MenuSection(string name)
        {
            Name = name;
        }

        public int Id { get; set; }

        [JsonIgnore]
        public Menu Menu { get; set; } = null!;

        public int MenuId { get; set; }
        public string Name { get; set; }
        public List<MenuItem> MenuItems { get; set; } = [];
    }
}
