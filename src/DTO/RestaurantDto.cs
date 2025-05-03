namespace RemoteMenu.DTO
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MenuCount { get; set; }
        public ICollection<MenuDto> Menus { get; set; }
    }
}
