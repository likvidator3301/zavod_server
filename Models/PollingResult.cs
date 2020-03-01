namespace Models
{
    public class PollingResult
    {
        public AccessAndRefreshTokeDto Tokens { get; set; }
        public ServerUserDto User { get; set; }
    }
}