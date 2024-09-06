namespace KM.Api.Models.Store;
public record StoreInfoApiModel
{
    public string StoreName { get; init; }
    public string DisplayName { get; init; }
    public string Url { get; init; }
    public string Phone { get; init; }
    public IReadOnlyDictionary<string, string> SocialLinks { get; init; }
}
