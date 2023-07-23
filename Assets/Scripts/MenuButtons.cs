using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
public class MenuButtons : MonoBehaviourPunCallbacks
{
    public GameObject[] mainMenuUI;
    public GameObject[] earlyLobbyMenuUI;

    public TMP_InputField _gamerPersonNickName;
    private string _gpnnDefault;
    public TMP_InputField _roomName;
    private string _rnDefault;
    public string _levelName;
    public TMP_Text _playersList;
    public string _playersListDefault;
    public TMP_Text[] _playersNames;

    public TMP_Text _lobbyName;

    public TMP_Text _readyButton;
    private string _readybt = "Ready";
    private string _notReadybt = "Not ready";
    private string _lobbySceneName = "MainMenu";
    private bool _changeScene;
    [SerializeField] private Dictionary<string, bool> _playersReadyOrNot= new Dictionary<string, bool>();
    private bool _enablePlay;
    [SerializeField] private bool isGoingToMenu;
    private void Start()
    {
        if (isGoingToMenu)
        {
            return;
        }
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
            PhotonNetwork.ConnectUsingSettings();
            _gpnnDefault = _gamerPersonNickName.text;
            _rnDefault = _roomName.text;
            _playersListDefault = _playersList.text;
    }
    #region Functions
    public void PalLobby()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(_lobbySceneName);
    }
    public void RefreshNames()
    {
        for (int i = 0; i < _playersNames.Length; i++)
        {
            if (i < PhotonNetwork.PlayerList.Length)
            {
                    if (_playersReadyOrNot[PhotonNetwork.PlayerList[i].NickName])
                    {
                        _playersNames[i].text = PhotonNetwork.PlayerList[i].NickName + " Ready";
                    }
                else
                    {
                        _playersNames[i].text = PhotonNetwork.PlayerList[i].NickName + " Not ready";
                    }
            }
            else
            {
                _playersNames[i].text = "";
            }
        }
        foreach (var item in _playersReadyOrNot)
        {
            _changeScene = true;
            if (!item.Value)
            {
                _changeScene = false;
                return;
            }
        }
        if (_changeScene && PhotonNetwork.PlayerList.Length>1)
        {
            PhotonNetwork.LoadLevel(_levelName);
        }
    }
    public void ReadyButton()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) //paso por la lista de players de photon
        {
            if (PhotonNetwork.PlayerList[i].NickName == PhotonNetwork.LocalPlayer.NickName)
            {
                if (_playersReadyOrNot.ContainsKey(PhotonNetwork.PlayerList[i].NickName))
                {
                    _playersReadyOrNot[PhotonNetwork.PlayerList[i].NickName] = 
                        !_playersReadyOrNot[PhotonNetwork.PlayerList[i].NickName];
                    RefreshNames();
                }
            }
            if (_playersReadyOrNot[PhotonNetwork.PlayerList[i].NickName])
            {
                _playersNames[i].text = PhotonNetwork.PlayerList[i].NickName + " Ready";
            }
            else
            {
                _playersNames[i].text = PhotonNetwork.PlayerList[i].NickName + " Not ready";
            }
        }
        photonView.RPC("RPC_NotifyReadyOrNot", RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName,
            _playersReadyOrNot[PhotonNetwork.LocalPlayer.NickName]);
    }
    public void OnOffMainMenuUI(bool value)
    {
        if (!_enablePlay)
        {
            return;
        }
        foreach (var item in mainMenuUI)
        {
            item.SetActive(value);
        }
        foreach (var item in earlyLobbyMenuUI)
        {
            item.SetActive(!value);
        }
    }
    public void ExitButton()
    {
        Application.Quit();
    }
    #endregion
    #region Punes
    [PunRPC]
    private void RPC_NotifyNewOnesStateOfRoom(Dictionary<string,bool> dic)
    {
        if (!photonView.IsMine)
        {
            _playersReadyOrNot = dic;
            RefreshNames();
        }
    }
    [PunRPC] public void RPC_NotifyReadyOrNot(string key,bool value)
    {
            _playersReadyOrNot[key] = value;
            RefreshNames();
    }
    #endregion
    #region PhotonThings
    public void ButtonConnect()
    {
        if (_gamerPersonNickName.text == null || _gamerPersonNickName.text == _gpnnDefault)
        {
            return;
        }
        if (_roomName.text == null || _roomName.text == _rnDefault)
        {
            return;
        }
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            return;
        }
        OnOffMainMenuUI(false);
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        PhotonNetwork.LocalPlayer.NickName = _gamerPersonNickName.text;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        _enablePlay = true;        
    }
    public override void OnJoinedLobby()
    {
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _lobbyName.text = _roomName.text;
        _playersList.text = _playersListDefault + " " + PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (PhotonNetwork.PlayerList.Length == 1)
        {
            _playersReadyOrNot.Add(PhotonNetwork.LocalPlayer.NickName, false);
            RefreshNames();
        }
        Debug.Log("Room Joined");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _playersList.text = _playersListDefault + " " + PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        _playersReadyOrNot.Add(newPlayer.NickName,false);
        RefreshNames();
        photonView.RPC("RPC_NotifyNewOnesStateOfRoom", newPlayer, _playersReadyOrNot);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        _playersList.text = _playersListDefault + " " + PhotonNetwork.PlayerList.Length + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
        if (_playersReadyOrNot.ContainsKey(otherPlayer.NickName))
        {
            _playersReadyOrNot.Remove(otherPlayer.NickName);
        }
        RefreshNames();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log("Failed to join room");
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Room Created");
        _lobbyName.text = _roomName.text;
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("Failed to create room");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Conection failed, Cause: "+ cause.ToString());
    }
    #endregion
}