using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager instance;
    [SerializeField] private Dictionary<LifeComponent,bool> _players = new Dictionary<LifeComponent, bool>();
    private void Start()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SuscribeHero(LifeComponent h)
    {
        _players.Add(h,true);
    }
    public void UnsuscribeHero(LifeComponent h)
    {
        _players[h]= false;
        int num=0;
        foreach (var item in _players)
        {
            if (item.Value)
            {
                num++;
            }
        }
        if (num < 2)
        {
            foreach (var item in _players)
            {
                item.Key.ShowVictoryOrDefeatUI();
            }
        }
    }
}
