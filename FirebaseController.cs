using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using UnityEngine;
using System.Threading.Tasks;
using System;
using Firebase.Auth;

public class FirebaseController
{
    private Firebase.FirebaseApp App;
    private Firebase.Auth.FirebaseAuth Auth;

    /**
     * Constructors
     */
    public FirebaseController ()
    {
        this.App = Firebase.FirebaseApp.DefaultInstance;
        this.Auth = Firebase.Auth.FirebaseAuth.GetAuth(this.App);
    }

 
    
    /// <summary>
    /// Login with provided e-mail and password.
    /// The callback is an Action<Exception> returning Null on success or the error otherwise.
    /// </summary>
    /// <param name="email">User e-mail</param>
    /// <param name="password">User password</param>
    /// <param name="callback">Callback to be called</param>
    public void loginWithEmailAndPasswordAsync(string email, string password, Action<Exception> callback)
    {
        // TaskScheduler needed for running callback in same thread.
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(authTask =>
        {
            if (authTask.IsFaulted)
            {
                callback(authTask.Exception.GetBaseException());
            }
            else if (authTask.IsCompleted)
            {
                callback(null);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
      
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="displayName">User display name.</param>
    /// <param name="email">User email</param>
    /// <param name="password">User password</param>
    /// <param name="callback">Callback to be called</param>
    public void createNewUser(String displayName, string email, string password, Action<Exception> callback)
    {
        // TaskScheduler needed for running callback in same thread.
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(authTask =>
        {
            if (authTask.IsFaulted)
            {
                callback(authTask.Exception.GetBaseException());
            }
            else if (authTask.IsCompleted)
            {
                UserProfile profile = new UserProfile();
                profile.DisplayName = displayName;
                Auth.CurrentUser.UpdateUserProfileAsync(profile).ContinueWith(task => {
                    callback(null);
                }, TaskScheduler.FromCurrentSynchronizationContext());
                
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    /// <summary>
    /// Returns the current logged user.
    /// </summary>
    /// <returns></returns>
    public FirebaseUser getCurrentUser()
    {
        return Auth.CurrentUser;
    }

    /// <summary>
    /// Sign out
    /// </summary>
    public void signOut()
    {
        Auth.SignOut();
    }
}
