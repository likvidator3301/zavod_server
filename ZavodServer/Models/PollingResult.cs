using Models;

namespace ZavodServer.Models
{
    public class PollingResult
    {
        public AccessAndRefreshTokenDto Tokens { get; set; }
        public UserDb User { get; set; }
        public Status Status { get; set; }
    }
}