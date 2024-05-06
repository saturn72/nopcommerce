using Nop.Core.Configuration;

namespace Nop.Plugin.ExternalAuth.Firebase;

/// <summary>
/// Represents settings of the Firebase authentication method
/// </summary>
public class FirebaseExternalAuthSettings : ISettings
{
    public string ApiKey { get; set; }
    public string AuthDomain { get; set; }
    public string ProjectId { get; set; }
    public string AppId { get; set; }
}