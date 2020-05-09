using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using UnityEngine.Experimental.UIElements;

[CustomEditor(typeof(Authentication))]
public class AuthenticationEditor : Editor
{
    bool expandOnLoginSuccess = true;
    bool expandOnLoginError = true;
    bool expandErrorMessages = false;

    public override void OnInspectorGUI()
    {

        Authentication authentication = (Authentication)target;
        this.serializedObject.Update();

        // Obligatory Input Fields
        EditorGUILayout.LabelField("Input Fields");
        authentication.emailField = (InputField)EditorGUILayout.ObjectField(new GUIContent("E-mail InputField", "InputField that will keep the user's e-mail."), authentication.emailField, typeof(InputField), true);
        authentication.passwordField = (InputField)EditorGUILayout.ObjectField(new GUIContent("Password InputField", "InputField that will keep the user's password."), authentication.passwordField, typeof(InputField), true);
        EditorGUILayout.Space();

        // Show Loader options
        authentication.useLoader = EditorGUILayout.ToggleLeft(new GUIContent("Show Loader", "Defines an object to be shown when the plugin is contacting firebase server."), authentication.useLoader);
        if (authentication.useLoader)
        {
            authentication.loader = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Loader GameObject", "The loader to be shown."), authentication.loader, typeof(GameObject), true);

        }

        // On Login Success options
        expandOnLoginSuccess = EditorGUILayout.Foldout(expandOnLoginSuccess, "On Login Success");
        if (expandOnLoginSuccess)
        {
            // Configure Success Scene Change
            authentication.changeSceneOnSuccess = EditorGUILayout.ToggleLeft(new GUIContent("Change to Scene", "Defines a scene to gowhen the login is successful."), authentication.changeSceneOnSuccess);
            if (authentication.changeSceneOnSuccess)
            {
                string[] scenes = getScenes();
                if (scenes.Length == 0)
                {
                    // Displays helpbox if scene list is empty
                    EditorGUILayout.HelpBox("No scene available. To select a scene here, first add it to your Project Build in \"File > Build Settings > Scene in build\", or simply click the button below.", MessageType.Info);
                    if (GUILayout.Button("Open Build Settings"))
                    {
                        // Open Build Settings menu
                        EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                    }
                }
                else
                {
                    authentication.sceneToGo = EditorGUILayout.Popup(authentication.sceneToGo, scenes);
                }
            }
            EditorGUILayout.Space();

            // Configure Success Event
            authentication.eventOnSuccess = EditorGUILayout.ToggleLeft(new GUIContent("Call Function", "Defines a function to be called when the login is successful."), authentication.eventOnSuccess);
            if (authentication.eventOnSuccess)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("successEvent"), new GUIContent("Function to Call"));
            }

        }
        EditorGUILayout.Space();

        // On Login Error options
        expandOnLoginError = EditorGUILayout.Foldout(expandOnLoginError, "On Login Error");
        if (expandOnLoginError)
        {
            // Configure Status Label
            authentication.useStatusLabel = EditorGUILayout.ToggleLeft(new GUIContent("Display error on label", "Defines a Text object to display the error message."), authentication.useStatusLabel);
            if (authentication.useStatusLabel)
            {
                authentication.statusLabel = (Text)EditorGUILayout.ObjectField(new GUIContent("Status Label", "UI.Text object that will display the current error message."), authentication.statusLabel, typeof(Text), true);
                authentication.labelErrorColor = EditorGUILayout.ColorField(new GUIContent("Label Error Color", "The color that the label will be painted on error."), authentication.labelErrorColor);


                // Display list of messages
                EditorGUI.indentLevel++;
                expandErrorMessages = EditorGUILayout.Foldout(expandErrorMessages, "Error Messages");
                if (expandErrorMessages)
                {
                    authentication.invalidEmailMessage = EditorGUILayout.TextField(authentication.invalidEmailMessage);
                    authentication.invalidPasswordMessage = EditorGUILayout.TextField(authentication.invalidPasswordMessage);
                    authentication.wrongPasswordMessage = EditorGUILayout.TextField(authentication.wrongPasswordMessage);
                    authentication.userNotFoundMessage = EditorGUILayout.TextField(authentication.userNotFoundMessage);
                    authentication.defaultErrorMessage = EditorGUILayout.TextField(authentication.defaultErrorMessage);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            // Configure Error Event
            authentication.eventOnError = EditorGUILayout.ToggleLeft(new GUIContent("Call Function", "Defines a function to be called when the login is successful."), authentication.eventOnError);
            if (authentication.eventOnError)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("errorEvent"), new GUIContent("Function to Call"));
            }

        }

        // Apply modifications on all serializedObjects
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Get a list of all scene in Build Settings
    /// </summary>
    /// <returns>string[] containing all scene names.</returns>
    private string[] getScenes()
    {

        // Get build scenes
        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        string[] arrayOfNames = new string[sceneNumber];

        for (int i = 0; i < sceneNumber; i++)
        {
            arrayOfNames[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }

        return arrayOfNames;
    }
}
