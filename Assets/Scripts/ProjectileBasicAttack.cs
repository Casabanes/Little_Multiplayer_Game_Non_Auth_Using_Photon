using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBasicAttack : MonoBehaviour
{
    private float _damage;
    private float _speed = 5;
    private Hero _owner;

    public ProjectileBasicAttack SetDamage(float damage)
    {
        _damage = damage;
        return this;
    }
    public ProjectileBasicAttack SetOwner(Hero owner)
    {
        _owner = owner;
        return this;
    }
    private void Update()
    {
        transform.position += transform.up * 5 * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.layer == 7)
            {
                return;
            }
        LifeComponent character = other.GetComponent<LifeComponent>();
        if(other.GetComponent<Hero>() != _owner)
        {
            if(character!=null)
            {
                character.TakeDamage(_damage);
            }
            Destroy(gameObject);
        }
    }
    public void SetDirection(Vector3 newDirection)
    {
        transform.up = newDirection;
    }
}
