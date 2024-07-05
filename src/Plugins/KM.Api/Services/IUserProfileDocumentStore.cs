namespace KM.Api.Services;

public interface IUserProfileDocumentStore : IDocumentStore<UserProfileDocument>
{
    Task<UserProfileDocument> GetByUserId(string uid);
}