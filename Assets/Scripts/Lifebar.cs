using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Lifebar : MonoBehaviour
{
    private LifeComponent _target;
    [SerializeField] private Image _myImage;
    [SerializeField] private float _yOffset;
    public Lifebar SetTarget(LifeComponent target)
    {
        _target = target;
        _target.onLifeBarUpdate += UpdateBar;
        _target.onDestroy += () => Destroy(gameObject);
        return this;
    }
    public Lifebar SetParent(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }
    public void UpdateBar(float amount)
    {
        _myImage.fillAmount = amount;
    }
    public void UpdatePosition()
    {
        transform.position = _target.transform.position + Vector3.up * _yOffset;
    }
}
