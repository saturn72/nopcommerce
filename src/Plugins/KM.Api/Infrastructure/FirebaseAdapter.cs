using Google.Apis.Auth.OAuth2;

namespace KM.Api.Infrastructure;
public sealed class FirebaseAdapter
{
    public FirebaseAuth GetAuth()
    {
        var app = FirebaseApp.GetInstance("user-service") ?? FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.GetApplicationDefault(),
            ProjectId = "kedem-market",
        }, "user-service");

        return FirebaseAuth.GetAuth(app);
    }
}
