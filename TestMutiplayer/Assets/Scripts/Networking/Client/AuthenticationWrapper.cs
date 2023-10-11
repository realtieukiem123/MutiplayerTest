using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public static class AuthenticationWrapper
{
    public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;
    public static async Task<AuthState> DoAuth(int maxTries = 5)
    {

        if (AuthState == AuthState.Authenticalted)
        {
            return AuthState;
        }

        if (AuthState == AuthState.Authenticating)
        {
            Debug.LogWarning("Already authenticating");
            await Authenticating();
            return AuthState;
        }


        await SignInAnonymouslyAsync(maxTries);
        return AuthState;
    }

    private static async Task<AuthState> Authenticating()
    {
        while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
        {
            await Task.Delay(200);
        }
        return AuthState;
    }
    private static async Task SignInAnonymouslyAsync(int maxRetries)
    {
        AuthState = AuthState.Authenticating;
        int reTries = 0;
        while (AuthState == AuthState.Authenticating && reTries < maxRetries)
        {

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                {
                    AuthState = AuthState.Authenticalted;
                    break;
                }

            }
            catch (AuthenticationException authException)
            {
                Debug.LogError(authException);
                AuthState = AuthState.Error;
            }
            catch (RequestFailedException requestException)
            {
                Debug.LogError(requestException);
                AuthState = AuthState.Error;
            }

            reTries++;
            await Task.Delay(1000);

        }
        if (AuthState != AuthState.Authenticalted)
        {
            Debug.LogWarning($"PLayer was not signed in successfully after {reTries} retries");
            AuthState = AuthState.Timeout;
        }
    }
}
public enum AuthState
{
    NotAuthenticated,
    Authenticating,
    Authenticalted,
    Error,
    Timeout
}
