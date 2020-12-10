using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text Cost;
    void Start()
    {
        GameEvents.instance.onCostUpdate += UpdateCost;
    }

    public void UpdateCost(int cost)
    {
        Cost.text = cost.ToString();
    }
}
