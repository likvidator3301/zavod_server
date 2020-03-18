namespace ZavodServer.Models
{
    public enum Status
    {
        Ok,
        AccessDenied,
        AuthorizationPending,
        PollingTooFrequently,
        InvalidClient,
        InvalidGrant,
    }
}