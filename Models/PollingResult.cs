using ZavodServer.Models;

namespace Models
{
    public class PollingResult
    {
        public AccessAndRefreshTokenDto Tokens { get; set; }
        public ServerUserDto User { get; set; }
        public Status Status { get; set; }
    }
}