using Microsoft.Extensions.Configuration;
using NexusERP.Application.Common.Interfaces;

namespace NexusERP.Infrastructure.Settings;

public class LicenseSettings : ILicenseSettings
{
    public const string SectionName = "LicenseSettings";
    public string HmacSecret { get; set; } = string.Empty;

    public static LicenseSettings FromConfiguration(IConfiguration configuration)
    {
        var settings = new LicenseSettings();
        configuration.Bind(SectionName, settings);
        return settings;
    }
}
