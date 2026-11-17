using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using SniperOnline.Core;

// Requires Firebase Unity SDK (Auth + Core) and Google Sign-In plugin for Google login.
public sealed class FirebaseLoginController : MonoBehaviour
{
    [SerializeField] private SniperOnlineClient onlineClient;
    private FirebaseAuth auth;
    private bool ready;

    public event Action<string> LoginStatus;
    public string UserId => auth?.CurrentUser?.UserId;

    private async void Awake()
    {
        var dependency = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependency != DependencyStatus.Available)
        {
            LoginStatus?.Invoke("Firebase dependencies unavailable: " + dependency);
            return;
        }
        auth = FirebaseAuth.DefaultInstance;
        ready = true;
        LoginStatus?.Invoke("Firebase ready.");
    }

    public async void SignInAsGuest()
    {
        if (!ready) { LoginStatus?.Invoke("Firebase is not ready."); return; }
        try
        {
            var result = await auth.SignInAnonymouslyAsync();
            await ConnectAuthenticatedUser(result.User);
        }
        catch (Exception ex) { LoginStatus?.Invoke("Guest login failed: " + ex.Message); }
    }

    // Supply the ID token returned by the Google Sign-In plugin.
    // Linking preserves the existing anonymous Firebase UID and its game identity.
    public async void LinkGoogleAccount(string googleIdToken, string googleAccessToken = null)
    {
        if (!ready || auth.CurrentUser == null) { LoginStatus?.Invoke("Sign in as guest first."); return; }
        if (string.IsNullOrWhiteSpace(googleIdToken)) { LoginStatus?.Invoke("Google ID token is missing."); return; }
        try
        {
            var credential = GoogleAuthProvider.GetCredential(googleIdToken, googleAccessToken);
            AuthResult result;
            if (auth.CurrentUser.IsAnonymous)
                result = await auth.CurrentUser.LinkWithCredentialAsync(credential);
            else
                result = await auth.SignInWithCredentialAsync(credential);
            await ConnectAuthenticatedUser(result.User);
        }
        catch (Exception ex) { LoginStatus?.Invoke("Google login/link failed: " + ex.Message); }
    }

    private async Task ConnectAuthenticatedUser(FirebaseUser user)
    {
        var token = await user.TokenAsync(false);
        LoginStatus?.Invoke(user.IsAnonymous ? "Guest account ready." : "Google account ready.");
        if (onlineClient == null) { LoginStatus?.Invoke("Assign SniperOnlineClient in Inspector."); return; }
        onlineClient.ConnectWithToken(token);
    }
}
