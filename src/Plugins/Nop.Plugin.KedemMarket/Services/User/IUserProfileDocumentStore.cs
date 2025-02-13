using KedemMarket.Documents;
using KedemMarket.Services.Documents;

namespace KedemMarket.Services.User;

public interface IUserProfileDocumentStore : IDocumentStore<UserProfileDocument>
{
    Task<UserProfileDocument> GetByUserId(string uid);
}