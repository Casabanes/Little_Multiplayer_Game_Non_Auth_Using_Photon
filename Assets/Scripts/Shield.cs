using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private Hero father;
    [SerializeField] private MeshRenderer _shieldmeshRenderer;
    [SerializeField] private BoxCollider _shieldbc;
    private void Start()
    {
        if (!father)
        {
            father = transform.parent.GetComponent<Hero>();
        }
        if (!_shieldmeshRenderer)
        {
            _shieldmeshRenderer = GetComponent<MeshRenderer>();
        }
        if (!_shieldbc)
        {
            _shieldbc = GetComponent<BoxCollider>();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        ProjectileBasicAttack bullet = other.GetComponent<ProjectileBasicAttack>();
        if(bullet!=null)
        {
            bullet.SetDirection(father.transform.forward);
            bullet.SetOwner(father);
        }
    }
    public void TurningOff()
    {
        StartCoroutine(TurnOff());
    }
    private IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(0.5f);
        _shieldbc.enabled = false;
        _shieldmeshRenderer.enabled = false;
    }
}
