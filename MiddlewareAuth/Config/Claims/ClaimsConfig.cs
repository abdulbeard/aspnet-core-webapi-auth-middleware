namespace MiddlewareAuth.Config.Claims
{
    public enum ClaimLocation
    {
        None = 0,
        Headers = 1,
        Body = 2,
        Uri = 3,
        QueryParameters = 4
    }

    public enum ExtractionType
    {
        None = 0,
        Type = 1,
        JsonPath = 2,
        RegEx = 3,
        KeyValue = 4
    }
}
