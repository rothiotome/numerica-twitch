using System;
using UnityEditor;
using UnityEngine;

namespace TwitchAPI
{
    [CustomEditor(typeof(TwitchOAuth))]
    public class TwitchOAuthEditor : Editor
    {
        bool visibleURLs = false;

        private SerializedProperty useUUUIDProp;
        private SerializedProperty loginSuccessMessageProp;
        private SerializedProperty loginFailMessageProp;
        private SerializedProperty portListProp;
        private SerializedProperty clientIDProp;

        private void OnEnable()
        {
            useUUUIDProp = serializedObject.FindProperty("useUUUID");
            loginSuccessMessageProp = serializedObject.FindProperty("loginSuccessMessage");
            loginFailMessageProp = serializedObject.FindProperty("loginFailMessage");
            portListProp = serializedObject.FindProperty("portList");
            clientIDProp = serializedObject.FindProperty("clientId");
        }

        public override void OnInspectorGUI()
        {
            var script = (TwitchOAuth)target;

            serializedObject.Update();

            EditorGUILayout.PropertyField(clientIDProp,
                new GUIContent("Client ID",
                    "The client ID from dev.twitch.com"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Login Messages", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(loginSuccessMessageProp,
                new GUIContent("Success",
                    "This message will be shown in the browser when the token has been successfully retrieved"));
            EditorGUILayout.PropertyField(loginFailMessageProp,
                new GUIContent("Fail",
                    "This message will be shown in the browser when the connection has somehow failed"));

            EditorGUILayout.Space();

            if (!useUUUIDProp.boolValue)
            {
                EditorGUILayout.HelpBox(
                    "Caution: The token will be visible on the browser URL when authorizing the app. This may result on the token being leaked if the user is streaming",
                    MessageType.Warning);
            }

            EditorGUILayout.PropertyField(useUUUIDProp, new GUIContent("Use UUID to hide Token",
                "Add a long numbers chain to the redirect URL to avoid the token to be shown on the browser"));

            for (int i = 0; i < portListProp.arraySize; i++)
            {
                portListProp.GetArrayElementAtIndex(i).intValue =
                    Math.Clamp(portListProp.GetArrayElementAtIndex(i).intValue, 1024, 65535);
            }
            
            EditorGUILayout.Space();

            //TODO: if there are more than 10, warning and remove the last one.
            EditorGUILayout.PropertyField(portListProp,
                new GUIContent("Port list",
                    "List of ports the game will try to use"));
            
            visibleURLs = EditorGUILayout.BeginFoldoutHeaderGroup(visibleURLs,
                new GUIContent("Redirect URL", "List of URLs that have to be added to Twitch Dashboard"));
            if (visibleURLs)
            {
                for (int i = 0; i < portListProp.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    string url = useUUUIDProp.boolValue
                        ? script.twitchRedirectHost + portListProp.GetArrayElementAtIndex(i).intValue + "/" +
                          script.uselessUUID
                        : script.twitchRedirectHost + portListProp.GetArrayElementAtIndex(i).intValue + "/";

                    if (GUILayout.Button(EditorGUIUtility.IconContent("Clipboard", "Copy to Clipboard"),
                            GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        GUIUtility.systemCopyBuffer = url;
                    }
                    
                    EditorGUILayout.SelectableLabel(url, EditorStyles.textArea);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}