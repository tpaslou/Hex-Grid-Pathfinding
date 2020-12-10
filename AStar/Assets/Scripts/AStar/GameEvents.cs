using System;
using AStar;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    //This can be Wiedely extended , and become
    //the observer pattern
    public static GameEvents instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<StarNode> onNodeClick;
    public event Action<int> onCostUpdate;
    public void NodeClick(StarNode node)
    {
        if (onNodeClick != null)
        {
            onNodeClick(node);
        }
    }

    public void CostUpdate(int value)
    {
        if (onCostUpdate != null)
        {
            onCostUpdate(value);
        }
    }
}