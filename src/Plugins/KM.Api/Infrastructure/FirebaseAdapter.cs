using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace KM.Api.Infrastructure;
public sealed class FirebaseAdapter
{
    private readonly string[] _scopes = new[]
        {
       "https://www.googleapis.com/auth/cloud-platform",
       "https://www.googleapis.com/auth/firebase",
    };

    private readonly FirebaseApp _app;
    private readonly StorageClient _storageClient;

    public FirebaseAdapter()
    {
        var cred = GoogleCredential.GetApplicationDefault();
        _app = FirebaseApp.GetInstance("user-service") ?? FirebaseApp.Create(new AppOptions()
        {
            Credential = cred,
            ProjectId = "kedem-market",
        }, "user-service");

        _ = cred.CreateScoped(_scopes)
        .UnderlyingCredential;

        _storageClient = StorageClient.Create();
    }
    public FirebaseAuth Auth => FirebaseAuth.GetAuth(_app);
    public StorageClient Storage => _storageClient;
}
