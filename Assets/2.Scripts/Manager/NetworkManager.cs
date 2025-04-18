using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] bool isTest;
    private TypedLobby sqlLobby = new TypedLobby("customSqlLobby", LobbyType.SqlLobby);
    public const string MYRATING_PROP_KEY = "C0";
    public const string MYMONSTER_DECK_PROP_KEY = "MONSTER_IDs";

    bool isReconnecting = false;


    Coroutine watingPlayer;

    bool isConnectedServer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Screen.SetResolution(760, 780, false);
        StartCoroutine(Connect());


        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SerializationRate = 50;
        PhotonNetwork.SendRate = 50;
    }

    void Update()
    {
        //if (watingPlayer != null)
        //{
        //    StopCoroutine(watingPlayer);
        //    watingPlayer = null;
        //}
    }

    [ContextMenu("����")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log($"���� �� �̸� : {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"���� �� �ο� �� : {PhotonNetwork.CurrentRoom.PlayerCount}");

            string playerName = "�濡 �ִ� �÷��̾� ��� : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                playerName += $"{PhotonNetwork.PlayerList[i].NickName},";
            }

            Debug.Log(playerName);
        }
        else
        {
            Debug.Log($"���� �ο� �� : {PhotonNetwork.CountOfPlayers}");
            Debug.Log($"�� ���� : {PhotonNetwork.CountOfRooms}");
        }
    }

    public void OnClickMathcingBtn()
    {
        JoinRandomRoomOrCreate();
    }

    public IEnumerator Connect(float _timeOut = 10f)
    {
        isConnectedServer = false;
        PhotonNetwork.ConnectUsingSettings();
        float elapsed = 0f;
        while (!isConnectedServer && elapsed < _timeOut)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (!isConnectedServer)
        {
            Debug.LogWarning("���� ���� ����");
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("���� ���� �Ϸ�");
        isConnectedServer = true;
    }

    public void DisConnected()
    {
        PhotonNetwork.Disconnect();
        ///
        /// 
        ///
    }

    /// <summary>
    /// ��Ʈ��ũ�������� 10������
    /// 
    /// </summary>
    /// <param name="cause"></param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        switch (cause)
        {
            case DisconnectCause.ClientTimeout:
                Debug.LogWarning($"���� ���� �������� ���� ����");
                if (!isReconnecting)
                    StartCoroutine(TryReconnect());
                break;
            case DisconnectCause.ServerTimeout:
                Debug.LogWarning($"���� �� Ÿ�Ӿƿ� �߻�");
                break;
            case DisconnectCause.Exception:
            case DisconnectCause.ExceptionOnConnect:
                Debug.LogWarning($"��Ʈ��ũ  ���� �߻�");
                break;

            case DisconnectCause.DisconnectByClientLogic:
                Debug.LogWarning($"���� ���� ����");
                break;
            default:
                Debug.Log($"��Ÿ ���� ���� : {cause}");
                break;
        }
    }

    IEnumerator TryReconnect()
    {
        int tryCount = 0;
        while (tryCount < 60)
        {
            /*��¥ ���� ������ �ƴ�, ���������� ������ �õ��� �� �ִ� ������ �ȴ�(�����Ѱ� �ƴ�);
            if (PhotonNetwork.Reconnect())
            {
                Debug.Log("���� �翬�� ����");
                yield break;
            }
            */

            if (PhotonNetwork.NetworkClientState == ClientState.Disconnected)
            {
                Debug.Log($"���� �翬�� �õ���...");
                //1�е����� �� ������ ��
                PhotonNetwork.ReconnectAndRejoin();
                Connect();
            }
            else if (PhotonNetwork.NetworkClientState == ClientState.ConnectedToMasterServer)
            {
                Debug.Log("���� �翬�� ����!!!!");
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
            tryCount++;
        }

        Debug.Log("���� �� ���� ����");
        //�α��� ȭ������
    }

    public void Reconnected()
    {
        PhotonNetwork.Reconnect();
    }

    public override void OnConnected()
    {
        base.OnConnected();
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("�κ� ���� �Ϸ�");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("�� ���� �Ϸ�");
        PhotonNetwork.NickName = $"Player{PhotonNetwork.CurrentRoom.PlayerCount}";


        //�ӽ�
        SetMonsterDeck(SelectedMonsterHolder.Instance.GetMonsterIds());
        watingPlayer = StartCoroutine(WaitingPlayer());
        //
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log($"�� ���� ���� : Code{returnCode} Message : {message}");
    }

    #region[JoinRandomRoomOrCreate�� ����]

    //public void JoinRandomRoom()
    //{
    //    int myRating = int.Parse(myRatingText.text);

    //    string sqlLobbyFilter = $"C0 >= {myRating - 200} AND C0 <= {myRating + 200}";

    //    PhotonNetwork.JoinRandomRoom(null, 0, MatchmakingMode.FillRoom, sqlLobby, sqlLobbyFilter);
    //}
    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    base.OnJoinRandomFailed(returnCode, message);
    //    CreateRoom();
    //}
    //public void CreateRoom()
    //{
    //    int myRating = int.Parse(myRatingText.text);

    //    RoomOptions roomOption = new RoomOptions();
    //    roomOption.MaxPlayers = 2;
    //    roomOption.CustomRoomProperties = new Hashtable { { MYRATING_PROP_KEY, myRating } };
    //    roomOption.CustomRoomPropertiesForLobby =  new string[] { MYRATING_PROP_KEY};

    //    PhotonNetwork.CreateRoom(null, roomOption,sqlLobby);
    //}

    #endregion

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        var curRoom = PhotonNetwork.CurrentRoom;


        List<(string ownerPrefix, int monsterId)> allMonsterEntries = new();
        if (curRoom.PlayerCount == curRoom.MaxPlayers)
        {
            //���ξ� ����
            Debug.Log("���� ����");
            curRoom.IsOpen = false;
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("InGameScene");
            }
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log($"�� ����� ���� Code : {returnCode}, Message : {message}");
    }

    public void JoinRandomRoomOrCreate()
    {
        int myRating = 0;

        string sqlLobbyFilter = $"C0 >= {myRating - 200} AND C0 <= {myRating + 200}";

        int[] monsterIds = new int[4];

        for (int i = 0; i < SelectedMonsterHolder.Instance.GetMonsterList().Count; i++)
        {
            monsterIds[i] = SelectedMonsterHolder.Instance.GetMonsterList()[i].ID;
        }

        Hashtable roomProps = new Hashtable
        {
            { MYRATING_PROP_KEY, myRating }
        };

        RoomOptions roomOption = new RoomOptions()
        {
            MaxPlayers = isTest ? 1 : 2,
            CustomRoomProperties = roomProps,
            CustomRoomPropertiesForLobby = new string[] { MYRATING_PROP_KEY, "MasterMonsterIDs" },
            PlayerTtl = 0,
            EmptyRoomTtl = 1000,
        };

        PhotonNetwork.JoinRandomOrCreateRoom(null, 1, MatchmakingMode.FillRoom, sqlLobby, sqlLobbyFilter, null,
            roomOption);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("�� ���� ����");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        Debug.Log("���� �� ����");
        SceneManager.LoadSceneAsync("MainScene");
    }

    //���� �������� ���� �÷��̾�� ȣ��Ǵ� �Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //�ðܵ� ȣ���� ��. �ð����� ȣ��, �÷��̾ ���� �������� ȣ��
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log("��� �÷��̾ �� ������");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (changedProps.ContainsKey(MYMONSTER_DECK_PROP_KEY))
        {
            Debug.Log($"{targetPlayer.NickName} �� ���� �Ϸ�");
        }
    }

    IEnumerator WaitingPlayer()
    {
        //TO DO
        //Stop�ؾ� �ϴ»�Ȳ : ��Ī�� ��Ī ���, ��Ī �� ���� ���� ��������
        Debug.Log("�÷��̾� ��ٸ��� ��");

        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers);
        Debug.Log("���� ����");
        var curRoom = PhotonNetwork.CurrentRoom;

        if (curRoom.PlayerCount == curRoom.MaxPlayers && isTest)
        {
            //���ξ� ����
            Debug.Log("���� ����");
            curRoom.IsOpen = false;

            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("InGameScene");
                ChatManager.Instance.SwichChannel(ChannelType.Lobby);
            }
        }
    }

    public void SetMonsterDeck(int[] _selectedMonIds)
    {
        Hashtable prop = new Hashtable()
        {
            { MYMONSTER_DECK_PROP_KEY, _selectedMonIds }
        };

        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);
    }
}