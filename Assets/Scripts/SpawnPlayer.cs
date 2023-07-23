using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] GameObject _playerPrefab;
    [SerializeField] Transform[] _playerSpawnPoint;
    void Start()
    {
        if (_playerSpawnPoint == null)
        {
            PhotonNetwork.Instantiate(_playerPrefab.name, transform.position,Quaternion.identity);
        }
        else
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    PhotonNetwork.Instantiate(_playerPrefab.name, _playerSpawnPoint[i].position, _playerSpawnPoint[i].rotation);
                    break;
                }
            }
        }
    }
}
