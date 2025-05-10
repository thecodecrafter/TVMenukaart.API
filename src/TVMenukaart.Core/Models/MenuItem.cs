namespace TVMenukaart.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public int MenuSectionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
