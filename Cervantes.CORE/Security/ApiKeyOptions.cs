namespace Cervantes.CORE.Security;

public class ApiKeyOptions
{
    public bool Enabled { get; set; } = true;
    public string HeaderName { get; set; } = "X-API-Key";
    public string Pepper { get; set; } = ""; // Provide via secrets/env in production
    public int DefaultExpiryDays { get; set; } = 180;
    public bool RequireHttps { get; set; } = true;
}

