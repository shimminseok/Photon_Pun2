using NUnit.Framework;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //�ӽ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            NetworkManager.Instance.LeaveRoom();
            return;
        }


        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            NetworkManager.Instance.LeaveRoom();
        }
    }
}