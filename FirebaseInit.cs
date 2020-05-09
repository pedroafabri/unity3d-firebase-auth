using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseInit : MonoBehaviour
{
    public bool showSceneAlert = true;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && showSceneAlert)
        {
            Debug.LogWarning(System.String.Format("FirebaseInit: It's recommended to add FirebaseInit object in the first scene of Build Settings, but it's currently being initialized in scene \"{0}\". (Deactivate this alert unchecking \"Show Scene Alert\")", SceneManager.GetActiveScene().name));
        }

        // Validate Google Play Services if Android
        if (Application.platform == RuntimePlatform.Android)
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                var dependencyStatus = task.Result;
                if (dependencyStatus != Firebase.DependencyStatus.Available)
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }
    }
}
