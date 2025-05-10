namespace TVMenukaart.DTO
{
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int MenuSectionId { get; set; }
    }
}
