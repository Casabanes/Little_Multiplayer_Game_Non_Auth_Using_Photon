using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] _renderers;
    public void ChangeColor(Color c)
    {
        foreach (var item in _renderers)
        {
            item.material.color = c;
        }
    }
}
