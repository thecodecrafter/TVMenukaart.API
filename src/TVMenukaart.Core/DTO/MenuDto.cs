namespace TVMenukaart.DTO
{
    public class MenuDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PublicUrl { get; set; }
        public ICollection<MenuSectionDto> MenuSections { get; set; }
    }
}
