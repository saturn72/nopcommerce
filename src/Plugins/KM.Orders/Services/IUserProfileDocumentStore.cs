namespace KM.Orders.Services;

public interface IUserProfileDocumentStore : IDocumentStore<UserProfileDocument>
{
    Task<UserProfileDocument> GetByUserId(string uid);
}