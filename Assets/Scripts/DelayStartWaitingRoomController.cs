﻿using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DelayStartWaitingRoomController : MonoBehaviourPunCallbacks
{
    private PhotonView  myPhotonView = null;

    [SerializeField]
    private InputField  mySeedInput = null;

    [SerializeField]
    private int         myMultiplayerSceneIndex;
    [SerializeField]
    private int         myMenuSceneIndex;

    private int         myPlayerCount = 0;
    private int         myRoomSize = 4;

    [SerializeField]
    private int         myMinPlayersToStart = 2;

    [SerializeField]
    private Text        myPlayerCountDisplay = null;
    [SerializeField]
    private Text        myTimerToStartDisplay = null;

    private bool        myReadyToCountDown = false;
    private bool        myReadyToStart = false;
    private bool        myStartingGame = false;

    private float       myTimerToStartGame = 0;
    private float       myNotFullGameTimer = 0;
    private float       myFullGameTimer = 0;

    [SerializeField]
    private float       myMaxWaitTime;
    [SerializeField]
    private float       myMaxFullGameTime;

    private void Start()
    {
        myPhotonView = GetComponent<PhotonView>();
        myFullGameTimer = myMaxFullGameTime;
        myNotFullGameTimer = myMaxWaitTime;
        myTimerToStartGame = myMaxWaitTime;

        PlayerCountUpdate();
    }

    private void PlayerCountUpdate()
    {
        myPlayerCount = PhotonNetwork.PlayerList.Length;
        myRoomSize = PhotonNetwork.CurrentRoom.MaxPlayers;
        myPlayerCountDisplay.text = myPlayerCount + ":" + myRoomSize;

        if(myPlayerCount == myRoomSize)
        {
            myReadyToStart = true;
        }
        else if(myPlayerCount >= myMinPlayersToStart)
        {
            myReadyToCountDown = true;
        }
        else
        {
            myReadyToStart = false;
            myReadyToCountDown = false;
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PlayerCountUpdate();

        if(PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_SendTimer", RpcTarget.Others, myTimerToStartGame);
            myPhotonView.RPC("RPC_SendSeed", RpcTarget.Others, int.Parse(mySeedInput.text));
        }
    }

    public void UpdateSeedOnClients()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            myPhotonView.RPC("RPC_SendSeed", RpcTarget.Others, int.Parse(mySeedInput.text));
            FindObjectOfType<GameInstance>().mySeed = int.Parse(mySeedInput.text);
        }
    }

    [PunRPC]
    private void RPC_SendTimer(float aTimeIn)
    {
        myTimerToStartGame = aTimeIn;
        myNotFullGameTimer = aTimeIn;
        if(aTimeIn < myFullGameTimer)
        {
            myFullGameTimer = aTimeIn;
        }
    }

    [PunRPC]
    private void RPC_SendSeed(int aSeed)
    {
        FindObjectOfType<GameInstance>().mySeed = aSeed;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PlayerCountUpdate();
    }

    private void Update()
    {
        WaitingForMorePlayers();
    }

    private void WaitingForMorePlayers()
    {
        if(myPlayerCount <= 1)
        {
            ResetTimer();
        }
        if(myReadyToStart)
        {
            myFullGameTimer -= Time.deltaTime;
            myTimerToStartGame = myFullGameTimer;
        }
        else if(myReadyToCountDown)
        {
            myNotFullGameTimer -= Time.deltaTime;
            myTimerToStartGame = myNotFullGameTimer;
        }

        string tempTimer = string.Format("{0:00}", myTimerToStartGame);
        myTimerToStartDisplay.text = tempTimer;

        if(myTimerToStartGame <= 0)
        {
            if (myStartingGame)
                return;

            StartGame();
        }
    }

    private void ResetTimer()
    {
        myTimerToStartGame = myMaxWaitTime;
        myNotFullGameTimer = myMaxWaitTime;
        myFullGameTimer = myMaxFullGameTime;
    }

    private void StartGame()
    {
        myStartingGame = true;
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel(myMultiplayerSceneIndex);
    }

    public void DelayCancel()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(myMenuSceneIndex);
    }
}
