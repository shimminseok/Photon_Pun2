using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class ChatManager : Singleton<ChatManager>, IChatClientListener
{
    ChatClient chatClient;
    string currentChannelStr;

    public TMP_InputField inputMessage;
    public GameObject textPrefab;
    public Transform messageRoot;
    public Queue<string> message;

    public ChannelType currentChannel = ChannelType.Lobby;

    void Awake()
    {
    }

    void Start()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0",
            new AuthenticationValues(PhotonNetwork.NickName));
    }

    void Update()
    {
        chatClient?.Service();
    }

    public void SendMessage(string _msg)
    {
        if (!string.IsNullOrEmpty(_msg))
        {
            chatClient.PublishMessage(currentChannel.ToString(), _msg);
            inputMessage.text = string.Empty;
        }
    }

    public void OnClickSendMessageBtn()
    {
        string msg = $"{PhotonNetwork.NickName}: {inputMessage.text}";
        SendMessage(msg);
    }

    public void SwichChannel(ChannelType _type)
    {
        switch (_type)
        {
            case ChannelType.Lobby:
                SwichToLobbyChannel();
                break;
            case ChannelType.Room:
                SwitchToRoomChannel(PhotonNetwork.CurrentRoom.Name);
                break;
        }
    }

    public void SwichToLobbyChannel()
    {
        if (!string.IsNullOrEmpty(currentChannelStr))
            chatClient.Unsubscribe(new string[] { currentChannelStr });

        currentChannel = ChannelType.Lobby;

        currentChannelStr = "LobbyChannel";
        chatClient.Subscribe(new string[] { currentChannelStr });
    }

    public void SwitchToRoomChannel(string _roomName)
    {
        if (!string.IsNullOrEmpty(currentChannelStr))
            chatClient.Unsubscribe(new string[] { currentChannelStr });

        currentChannel = ChannelType.Room;

        currentChannelStr = $"Room_{_roomName}";
        chatClient.Subscribe(new string[] { currentChannelStr });
    }


    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
        Debug.Log("??? ?????? ???????????.");
        SwichChannel(currentChannel);
    }

    public void OnDisconnected()
    {
        Debug.Log("??? ?????? ?????? ????????.");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //??????? ???????
        for (int i = 0; i < messages.Length; i++)
        {
            GameObject messageObj = Instantiate(textPrefab, messageRoot);
            ChatUI message = messageObj.GetComponent<ChatUI>();
            message.ReceiveMessage(messages[i].ToString());

            //Debug.Log($"{senders[i]}: {messages[i]}");
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}