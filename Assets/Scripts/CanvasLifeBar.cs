using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CanvasLifeBar : MonoBehaviour
{
    [SerializeField] private Lifebar _lifeBarPrefab;
    public event Action updateBars = delegate{};
    public void SpawnBar(LifeComponent target)
    {
        Lifebar lifeBar = Instantiate(_lifeBarPrefab, target.transform.position,Quaternion.identity)
            .SetParent(transform)
            .SetTarget(target);
        updateBars += lifeBar.UpdatePosition;
        target.onDestroy += () => updateBars -= lifeBar.UpdatePosition;
    }
    void LateUpdate()
    {
        updateBars();
    }
}
