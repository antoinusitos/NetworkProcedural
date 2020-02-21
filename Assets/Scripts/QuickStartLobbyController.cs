using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class QuickStartLobbyController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject myQuickStartButton;
    [SerializeField]
    private GameObject myQuickCancelButton;
    [SerializeField]
    private int myRoomSize;

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        myQuickStartButton.SetActive(true);
    }

    public void QuickStart()
    {
        myQuickStartButton.SetActive(false);
        myQuickCancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Quick Start");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room");
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating room now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOptions = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)myRoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOptions);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create a room... trying again !");
        CreateRoom();
    }

    public void QuickCancel()
    {
        myQuickStartButton.SetActive(true);
        myQuickCancelButton.SetActive(false);
        PhotonNetwork.LeaveRoom();
    }
}
