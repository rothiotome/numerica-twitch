using System.Collections.Generic;
using TwitchChat;
using UnityEngine;

public class TestingChat : MonoBehaviour
{
    //[SerializeField] private TwitchSettings settings;

    void Start()
    {
        TwitchController.Login("RothioTome"); // connection to channel with the default settings
        
        //witchController.Login("RothioTome", settings); // connection to channel with the custom settings

        TwitchController.onChannelJoined += ChannelJoined;
        TwitchController.onTwitchMessageReceived += OnMessageReceived;
        TwitchController.onTwitchCommandReceived += OnCommandReceived;
    }

    private void OnDestroy()
    {
        TwitchController.onChannelJoined -= ChannelJoined;
        TwitchController.onTwitchMessageReceived -= OnMessageReceived;
        TwitchController.onTwitchCommandReceived -= OnCommandReceived;
    }

    private void ChannelJoined()
    {
        Debug.Log("Channel Joined");
    }

    private void OnMessageReceived(string user, string message)
    {
        Debug.Log($"Message received from {user} : {message}");
    }

    private void OnCommandReceived(string user, string command, List<string> arguments)
    {
        Debug.Log($"Command received from {user} : {command}");
    }
}