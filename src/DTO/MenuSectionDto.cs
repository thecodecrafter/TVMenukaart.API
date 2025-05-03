namespace RemoteMenu.DTO
{
    public class MenuSectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MenuItemsCount => MenuItems.Count;
        public ICollection<MenuItemDto> MenuItems { get; set; } = new List<MenuItemDto>();
    }
}
