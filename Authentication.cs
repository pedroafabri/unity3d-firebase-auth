using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Firebase;
using UnityEngine.Events;

public class Authentication : MonoBehaviour
{
    // Public variables
    public InputField emailField, passwordField;
    public Text statusLabel;
    public int sceneToGo = -1;
    public bool eventOnSuccess,
                changeSceneOnSuccess,
                useStatusLabel,
                eventOnError,
                useLoader;
    public Color labelErrorColor = Color.red;
    public LoginSuccessEvent successEvent;
    public LoginErrorEvent errorEvent;
    public GameObject loader;

    // Messages
    public string invalidEmailMessage = "E-mail is invalid.";
    public string invalidPasswordMessage = "Password cannot be blank.";
    public string wrongPasswordMessage = "Wrong password.";
    public string userNotFoundMessage = "User not found.";
    public string defaultErrorMessage = "Oops... Something went wrong.";

    // Private Variables
    private FirebaseController firebase;

    // Singleton control
    private static readonly object padlock = new object();
    private static Authentication instance = null;
    public static Authentication Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    Debug.LogError("Authentication has not been instacitated yet.");
                }
                return instance;
            }
        }
    }

    private void Awake()
    {
        if (instance != null) Destroy(this);
        else instance = this;
    }

    private void Start()
    {
        if (emailField == null) Debug.LogError("Authentication: Email Field is a required reference.");
        if (passwordField == null) Debug.LogError("Authentication: Password Field is a required reference.");

        // Check empty label
        if (useStatusLabel && statusLabel == null)
        {
            Debug.LogWarning("Authentication: The script is configured to show the error message in a label, but the label was not defined. Nothing will be shown.");
            useStatusLabel = false;
        }

        // Check not selected scene
        if (changeSceneOnSuccess && sceneToGo == -1)
        {
            Debug.LogWarning("Authentication: The script is configured to go to a scene on success, but there's no scene selected. The change will not happen.");
            changeSceneOnSuccess = false;
        }

        firebase = new FirebaseController();

        if (useLoader && loader == null)
        {
            Debug.LogWarning("Authentication: The script is configured to show a GameObject on load, but there's no object selected. The object will not appear.");
            useLoader = false;
        }
        else
        {
            hideLoader();
        }
    }

    /// <summary>
    /// Try to login on Firebase using the email and password fields.
    /// </summary>
    public void loginWithEmailAndPassword()
    {
        string email = emailField.text;
        string password = passwordField.text;

        if (email == "" || !EmailHelper.isEmailValid(email))
        {
            // FirebaseException to be treated and passed to errorEvent if suitable.
            treatFirebaseException(new FirebaseException((int) Firebase.Auth.AuthError.InvalidEmail));
            return;
        }

        if (password.Equals(""))
        {
            // FirebaseException to be treated and passed to errorEvent if suitable.
            treatFirebaseException(new FirebaseException((int)Firebase.Auth.AuthError.MissingPassword));
            return;
        }

        showLoader();

        firebase.loginWithEmailAndPasswordAsync(email, password, (error) =>
        {
            if (error == null)
            {
                success();
                return;
            }

            try
            {
                throw error;
            }
            catch (FirebaseException e)
            {
                treatFirebaseException(e);
            }
            
        });

    }

    /// <summary>
    /// Login with Google Provider
    /// </summary>
    public void loginWithGoogle()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Treats successful login
    /// </summary>
    private void success()
    {
        hideLoader();
        callSuccessEvent();
        goToSuccessScreen();
    }

    /// <summary>
    /// Call the SuccessEvent if eventOnSuccess = true
    /// </summary>
    private void callSuccessEvent()
    {
        if (eventOnSuccess) successEvent.Invoke();
    }

    /// <summary>
    /// Go to the selected scene if changeSceneOnSuccess = true.
    /// </summary>
    private void goToSuccessScreen()
    {
        if(changeSceneOnSuccess) SceneManager.LoadScene(sceneToGo);
    }

    /// <summary>
    /// Treats the given FirebaseException
    /// </summary>
    /// <param name="e">The FirebaseException to treat</param>
    private void treatFirebaseException(FirebaseException e)
    {
        int errorCode = e.ErrorCode;

        string message;
        switch (errorCode)
        {
            case (int)Firebase.Auth.AuthError.InvalidEmail: // Invalid Email
                message = invalidEmailMessage;
                break;

            case (int)Firebase.Auth.AuthError.MissingPassword: // Missing Password
                message = invalidPasswordMessage;
                break;
            case (int)Firebase.Auth.AuthError.WrongPassword: // Wrong Password
                message = wrongPasswordMessage;
                break;

            case (int)Firebase.Auth.AuthError.UserNotFound: // User Not Found
                message = userNotFoundMessage;
                break;

            default:
                Debug.LogError("Firebase Error:" + errorCode + " - " + e.Message);
                message = defaultErrorMessage;
                break;
        }

        hideLoader();
        callErrorEvent(e);
        showError(message);
    }

    /// <summary>
    /// Call the ErrorEvent if eventOnError = true
    /// </summary>
    /// <param name="e">FirebaseException to pass to the event.</param>
    private void callErrorEvent(FirebaseException e)
    {
        if (eventOnError) errorEvent.Invoke(e);
    }

    
    /// <summary>
    /// Set the given error message as the label text if useStatusLabel = true.
    /// </summary>
    /// <param name="message">The error error message to display.</param>
    private void showError(string message)
    {
        if(useStatusLabel) setStatusLabel(message, labelErrorColor);
    }

    /// <summary>
    /// Change the status label with given message and color.
    /// </summary>
    /// <param name="message">Label text message</param>
    /// <param name="color">Label text color</param>
    private void setStatusLabel(string message, Color color)
    {
        statusLabel.text = message;
        statusLabel.color = color;
        statusLabel.gameObject.SetActive(true);
    }

    /// <summary>
    /// Shows the loader object if useLoader == true
    /// </summary>
    private void showLoader()
    {
        if (useLoader) loader.SetActive(true);

    }

    /// <summary>
    /// Hides the loader object if useLoader == true
    /// </summary>
    private void hideLoader()
    {
        if (useLoader) loader.SetActive(false);
    }
}

    