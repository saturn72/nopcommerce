using Google.Apis.Auth.OAuth2;

namespace KM.Api.Infrastructure;
public sealed class FirebaseAdapter
{
    private readonly FirebaseApp _app;

    public FirebaseAdapter()
    {
        var cred = GoogleCredential.GetApplicationDefault();
        _app = FirebaseApp.GetInstance("user-service") ?? FirebaseApp.Create(new AppOptions()
        {
            Credential = cred,
            ProjectId = "kedem-market",
        }, "user-service");


    }
    public FirebaseAuth Auth => FirebaseAuth.GetAuth(_app);
}
