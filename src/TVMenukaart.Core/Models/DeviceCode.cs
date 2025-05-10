namespace TVMenukaart.Models
{
    public class DeviceCode
    {
        public string Code { get; set; }
        public string PollingToken { get; set; }
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
        public int ExpirationInSeconds { get; set; }
    }
}
