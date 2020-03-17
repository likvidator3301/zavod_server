using Models;

namespace ZavodServer.Models
{
    public class PollingResult
    {
        public AccessAndRefreshTokeDto Tokens { get; set; }
        public UserDb User { get; set; }
        public Status Status { get; set; }
    }
}