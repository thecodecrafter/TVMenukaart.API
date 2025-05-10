namespace TVMenukaart.Models
{
    public class MenuSection
    {
        public MenuSection(string name)
        {
            Name = name;
        }

        public int Id { get; set; }
        public int MenuId { get; set; }
        public string Name { get; set; }
        public List<MenuItem> MenuItems { get; set; } = [];
    }
}
