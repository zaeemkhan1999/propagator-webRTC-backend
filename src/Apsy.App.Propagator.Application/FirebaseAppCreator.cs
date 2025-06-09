using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;

namespace Apsy.App.Propagator.Application;

public class FirebaseAppCreator
{
    private FirebaseApp _app;
    private readonly string firebaseJsonUri;

    public FirebaseAppCreator(IConfiguration configuration)
    {
        firebaseJsonUri = configuration["ProjectJsonUri"];
    }

    public FirebaseApp GetFirebaseApp()
    {
        if (_app == null)
        {
            string[] path = new string[] { "Data", firebaseJsonUri };
            if (!File.Exists(System.IO.Path.Combine(path)))
            {
                throw new FileNotFoundException("config file not found");
            }
            var app = FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(System.IO.Path.Combine(path))
            });

            _app = app;
        }
        return _app;
    }

    public async Task DeleteUser(string externalId)
    {
        if (_app == null)
        {
            GetFirebaseApp();
        }

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        await auth.DeleteUserAsync(externalId);
    }
}