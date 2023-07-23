using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
public class LifeComponent : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private float _maxLife;
    private float _life;
    public event Action<float> onLifeBarUpdate = delegate { };
    public event Action onDestroy = delegate { };
    private const int _constZero = 0;
    [SerializeField] private GameObject _victoryUI;
    [SerializeField] private GameObject _defeatUI;
    [SerializeField] private GameObject[] hideComponents;

    private void Start()
    {
        CanvasLifeBar lifeBarManager = FindObjectOfType<CanvasLifeBar>();
        lifeBarManager?.SpawnBar(this);
    }
    private void Awake()
    {
        if (photonView.IsMine)
        {
            _life = _maxLife;
        }
        PlayersManager.instance.SuscribeHero(this);
    }
    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        _life -= damage;
        onLifeBarUpdate(_life / _maxLife);
        if (_life <= _constZero)
        {
            _life = _constZero;
            photonView.RPC("RPC_Die", RpcTarget.All);
        }
    }
    [PunRPC]
    public void RPC_Die()
    {
        PlayersManager.instance.UnsuscribeHero(this);
        OnHide();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_life / _maxLife);
        }
        else
        {
            onLifeBarUpdate((float)stream.ReceiveNext());
        }
    }

    public void ShowVictoryOrDefeatUI()
    {
        photonView.RPC("RPC_ShowVictoryOrDefeatUI", RpcTarget.All);

        
    }
    [PunRPC] public void RPC_ShowVictoryOrDefeatUI()
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (_life == 0)
        {
            _defeatUI.SetActive(true);
        }
        else
        {
            _victoryUI.SetActive(true);
        }
    }
    private void OnHide()
    {
        GetComponent<Animator>().enabled = false;
        foreach (var item in hideComponents)
        {
            Destroy(item);
        }
        GetComponent<Hero>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        onDestroy();
    }
}
