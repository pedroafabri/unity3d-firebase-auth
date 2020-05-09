using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using UnityEngine.SceneManagement;

public class SignUp : MonoBehaviour
{

    // Public Variables
    public InputField emailField, passwordField, nameField;
    public Text statusLabel;
    public int sceneToGo = -1;
    public bool eventOnSuccess,
                changeSceneOnSuccess,
                useStatusLabel,
                eventOnError,
                preventAutoLogin;

    // Messages
    public string invalidEmailMessage = "E-mail is invalid.";
    public string invalidPasswordMessage = "Password cannot be blank.";
    public string defaultErrorMessage = "Oops... Something went wrong.";

    public Color labelErrorColor = Color.red;
    public LoginSuccessEvent successEvent;
    public LoginErrorEvent errorEvent;

    // Private Variables
    private FirebaseController firebase;

    // Singleton control
    private static readonly object padlock = new object();
    private static SignUp instance = null;
    public static SignUp Instance
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
        if (SignUp.instance != null) Destroy(this);
        else SignUp.instance = this;
    }

    private void Start()
    {
        if (nameField == null) Debug.LogWarning("SignUp: Display Name Field is a recommended reference.");
        if (emailField == null) Debug.LogError("SignUp: Email Field is a required reference.");
        if (passwordField == null) Debug.LogError("SignUp: Password Field is a required reference.");

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
    }

    /// <summary>
    /// Creates a new user using the input fields.
    /// </summary>
    public void createUser()
    {
        string displayName = nameField.text;
        string email = emailField.text;
        string password = passwordField.text;

        if (email == "" || !EmailHelper.isEmailValid(email))
        {
            // FirebaseException to be treated and passed to errorEvent if suitable.
            treatFirebaseException(new FirebaseException((int)Firebase.Auth.AuthError.InvalidEmail));
            return;
        }

        if (password.Equals(""))
        {
            // FirebaseException to be treated and passed to errorEvent if suitable.
            treatFirebaseException(new FirebaseException((int)Firebase.Auth.AuthError.MissingPassword));
            return;
        }

        firebase.createNewUser(displayName, email, password, (error) =>
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
    /// Treats successful login
    /// </summary>
    private void success()
    {
        if (preventAutoLogin) firebase.signOut();
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
        if (changeSceneOnSuccess) SceneManager.LoadScene(sceneToGo);
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

            default:
                Debug.LogError("Firebase Error:" + errorCode + " - " + e.Message);
                message = defaultErrorMessage;
                break;
        }

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
        if (useStatusLabel) setStatusLabel(message, labelErrorColor);
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
}
