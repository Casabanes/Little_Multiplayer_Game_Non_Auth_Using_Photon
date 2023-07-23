using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
public class Hero : MonoBehaviourPun
{
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private float _speed;
    [SerializeField] private Transform _targetPoint;
    private Vector3 _startingForward;
    private Vector3 _direction;
    [SerializeField] private float _timeRotating;
    [SerializeField] private float _timeToRotate;
    private FollowTarget _camera;

    private const int _constZero = 0;



    [SerializeField] private ProjectileBasicAttack _attackProjectile;
    [SerializeField] private float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private Transform _normalAttackShootPosition;

    [SerializeField] private LayerMask _floor;


    [SerializeField] private MeshRenderer _shieldmeshRenderer;
    [SerializeField] private BoxCollider _shieldbc;
    [SerializeField] private Shield _shield;
    [SerializeField] private Animator _animator;
    [SerializeField] private string shootAnimatorParameter="shoot";
    [SerializeField] private string moveAnimatorParameter="moving";

    private void Start()
    {
       
    }
    private void Awake()
    {
        if (photonView.IsMine)
        {
            _targetPoint = Instantiate(_targetPoint, transform.position, Quaternion.identity);
            _rigidBody = GetComponent<Rigidbody>();
            _camera = FindObjectOfType<FollowTarget>();
            _camera.SetTarget(transform);
            if (_animator == null)
            {
                _animator = GetComponent<Animator>();
            }
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
                {
                    photonView.RPC("SetTeam", RpcTarget.All,i);
                }
            }
        }
    }
    private void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }    
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse1))
        {
            ClickInWindow();
        }
        GoToTarget();
        if (Input.GetKeyDown(KeyCode.Q))
        {
            photonView.RPC("RPC_NormalAttack", RpcTarget.All,_damage);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            photonView.RPC("RPC_TurnShieldOn", RpcTarget.All);
        }
    }
    [PunRPC]public void RPC_NormalAttack(float damage)
    {
        ProjectileBasicAttack basicAttack = Instantiate(_attackProjectile,
            _normalAttackShootPosition.position, _normalAttackShootPosition.rotation)
            .SetOwner(this)
            .SetDamage(damage);
        _animator.SetTrigger(shootAnimatorParameter);
    }
    [PunRPC] public void RPC_TurnShieldOn()
    {
        _shieldbc.enabled = true;
        _shieldmeshRenderer.enabled = true;
        _shield.TurningOff();
    }
    [PunRPC] public void RPC_AnimatorIsMovingOrNot(bool value)
    {
        _animator.SetBool(moveAnimatorParameter, value);
    }
    public void GoToTarget()
    {
        if (transform.position == _targetPoint.position)
        {
            return;
        }
        _direction = (_targetPoint.position - transform.position);
        _direction.y = _constZero;
        if (_direction.magnitude <= (_direction.normalized * _speed * Time.deltaTime).magnitude)
        {
            transform.position = _targetPoint.position;
            photonView.RPC("RPC_AnimatorIsMovingOrNot", RpcTarget.All,false);
        }
        else
        {
            transform.position += _direction.normalized * _speed * Time.deltaTime;
            if (!_animator.GetBool(moveAnimatorParameter))
            {
                photonView.RPC("RPC_AnimatorIsMovingOrNot", RpcTarget.All,true);
            }
        }
        RotateToTarget();
    }
    public void SelectTarget(Vector3 position)
    {
        _startingForward = transform.forward;
        _timeRotating = _constZero;
        position.y = _targetPoint.position.y;
        _targetPoint.position = position;
    }
    public void ClickInWindow()
    {
        Vector2 pointClicked = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(pointClicked);
        if(Physics.Raycast(ray,out RaycastHit raycastHit, _floor))
        {
            SelectTarget(raycastHit.point);
        }
    }
    public void RotateToTarget()
    {
        if (_direction.normalized == Vector3.zero)
        {
            return;
        }
        if(_timeRotating<= _timeToRotate)
        {
            _timeRotating += Time.deltaTime;
            transform.forward = Vector3.Slerp(_startingForward, _direction.normalized
                , _timeRotating / _timeToRotate);
            Mathf.Clamp(_timeRotating, _constZero, _timeToRotate);
        }
    }
    [PunRPC]
    public void SetTeam(int i)
    {
        switch (i)
        {
            case 0:
                GetComponent<ColorChanger>().ChangeColor(Color.red);
                break;
            case 1:
                GetComponent<ColorChanger>().ChangeColor(Color.blue);
                break;
            case 2:
                GetComponent<ColorChanger>().ChangeColor(Color.yellow);
                break;
            case 3:
                GetComponent<ColorChanger>().ChangeColor(Color.green);
                break;
            case 4:
                GetComponent<ColorChanger>().ChangeColor(Color.cyan);
                break;
            case 5:
                GetComponent<ColorChanger>().ChangeColor(Color.white);
                break;
            case 6:
                GetComponent<ColorChanger>().ChangeColor(Color.grey);
                break;
            case 7:
                GetComponent<ColorChanger>().ChangeColor(Color.black);
                break;
        }
    }
}
