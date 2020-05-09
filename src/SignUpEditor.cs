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

[CustomEditor(typeof(SignUp))]
public class SignUpEditor : Editor
{
    bool expandOnCreateSuccess = true;
    bool expandOnCreateError = true;
    bool expandErrorMessages = false;

    public override void OnInspectorGUI()
    {

        SignUp signUp = (SignUp)target;
        this.serializedObject.Update();

        // Obligatory Input Fields
        EditorGUILayout.LabelField("Input Fields");
        signUp.nameField = (InputField)EditorGUILayout.ObjectField(new GUIContent("Display Name InputField", "InputField that will keep the user's display name."), signUp.emailField, typeof(InputField), true);
        signUp.emailField = (InputField)EditorGUILayout.ObjectField(new GUIContent("E-mail InputField", "InputField that will keep the user's e-mail."), signUp.emailField, typeof(InputField), true);
        signUp.passwordField = (InputField)EditorGUILayout.ObjectField(new GUIContent("Password InputField", "InputField that will keep the user's password."), signUp.passwordField, typeof(InputField), true);
        EditorGUILayout.Space();

        // Prevent Auto Login option
        signUp.preventAutoLogin = EditorGUILayout.ToggleLeft(new GUIContent("Prevent Auto Login", "SignOut automatically after user creation."), signUp.preventAutoLogin);
        EditorGUILayout.HelpBox(new GUIContent("By default, when a new user is created it's defined as the current logged user. Check the box above to prevent this."));
        EditorGUILayout.Space();

        // On Create Success options
        expandOnCreateSuccess = EditorGUILayout.Foldout(expandOnCreateSuccess, "On Creation Success");
        if (expandOnCreateSuccess)
        {
            // Configure Success Scene Change
            signUp.changeSceneOnSuccess = EditorGUILayout.ToggleLeft(new GUIContent("Change to Scene", "Defines a scene to go when the creation is successful."), signUp.changeSceneOnSuccess);
            if (signUp.changeSceneOnSuccess)
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
                    signUp.sceneToGo = EditorGUILayout.Popup(signUp.sceneToGo, scenes);
                }
            }
            EditorGUILayout.Space();

            // Configure Success Event
            signUp.eventOnSuccess = EditorGUILayout.ToggleLeft(new GUIContent("Call Function", "Defines a function to be called when the creation is successful."), signUp.eventOnSuccess);
            if (signUp.eventOnSuccess)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("successEvent"), new GUIContent("Function to Call"));
            }

        }
        EditorGUILayout.Space();

        // On Creation Error options
        expandOnCreateError = EditorGUILayout.Foldout(expandOnCreateError, "On Creation Error");
        if (expandOnCreateError)
        {
            // Configure Status Label
            signUp.useStatusLabel = EditorGUILayout.ToggleLeft(new GUIContent("Display error on label", "Defines a Text object to display the error message."), signUp.useStatusLabel);
            if (signUp.useStatusLabel)
            {
                signUp.statusLabel = (Text)EditorGUILayout.ObjectField(new GUIContent("Status Label", "UI.Text object that will display the current error message."), signUp.statusLabel, typeof(Text), true);
                signUp.labelErrorColor = EditorGUILayout.ColorField(new GUIContent("Label Error Color", "The color that the label will be painted on error."), signUp.labelErrorColor);


                // Display list of messages
                EditorGUI.indentLevel++;
                expandErrorMessages = EditorGUILayout.Foldout(expandErrorMessages, "Error Messages");
                if (expandErrorMessages)
                {
                    signUp.invalidEmailMessage = EditorGUILayout.TextField(signUp.invalidEmailMessage);
                    signUp.invalidPasswordMessage = EditorGUILayout.TextField(signUp.invalidPasswordMessage);
                    signUp.defaultErrorMessage = EditorGUILayout.TextField(signUp.defaultErrorMessage);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();

            // Configure Error Event
            signUp.eventOnError = EditorGUILayout.ToggleLeft(new GUIContent("Call Function", "Defines a function to be called when the creation is successful."), signUp.eventOnError);
            if (signUp.eventOnError)
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
