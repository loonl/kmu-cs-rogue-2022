using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayNANOO.ChatServer;
using PlayNANOO.ChatServer.Models;

public class PlayNANOOChatExample : MonoBehaviour, IChatListener
{
    public Text chatMessageOut;
    public InputField chatMessageInput;

    ChatClient _chatClient;

    void Start()
    {
        // Chat Channel Subscribe
        string userId = "TESTER_PlayNANOOO";
        string userName = "TESTER";

        _chatClient = new ChatClient(this);
        _chatClient.SetPlayer(userId, userName);
        _chatClient.Connect();
    }

    void Update()
    {
        if (_chatClient != null)
        {
            _chatClient.Service();
        }
    }

    public void OnConntected()
    {
        Debug.Log("Connected");
        _chatClient.Subscribe("CH01");        
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnError(ChatErrorModel error)
    {
        Debug.Log(error.code);
        Debug.Log(error.message);
    }

    public void OnChannels(ChatChannelModel[] channels)
    {
        if (channels.Length > 0)
        {
            foreach (ChatChannelModel channel in channels)
            {
                Debug.Log(channel.channel);
                Debug.Log(channel.count.ToString());
            }
        }
    }

    public void OnPlayerOnline(ChatPlayerModel[] players)
    {
        if (players.Length > 0)
        {
            foreach (ChatPlayerModel player in players)
            {
                Debug.Log(player.userUniqueId);
                Debug.Log(player.online);
            }
        }
    }

    public void OnPrivateMessage(ChatInfoModel chatInfo, string message)
    {
        Debug.Log("OnPrivateMessage");
        Debug.Log(chatInfo.userName);
        Debug.Log(message);
    }

    public void OnPublicMessage(ChatInfoModel chatInfo, string message)
    {
        try
        {
            chatMessageOut.text += string.Format("[{0}] {1} \r\n", chatInfo.userName, message);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnNotifyMessage(ChatInfoModel chatInfo, string message)
    {
        try
        {
            if (chatMessageOut != null)
                chatMessageOut.text += string.Format("[{0}] {1} \r\n", chatInfo.userName, message);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnSubscribed(ChatInfoModel chatInfo)
    {
        Debug.Log("OnSubscribed");
        if (chatMessageOut != null)
            chatMessageOut.text += string.Format("{0} Joined \r\n", chatInfo.userName);
    }

    public void OnUnSubscribed(ChatInfoModel chatInfo)
    {
        throw new System.NotImplementedException();
    }

    public void InputOnEndEdit(string message)
    {
        _chatClient.SendPublicMessage("CH01", chatMessageInput.text);
        chatMessageInput.text = "";
    }
}
