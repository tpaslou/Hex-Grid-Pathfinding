using System;
using Pathing;
using UnityEngine;

namespace AStar
{
    
    public class NodeEvent : MonoBehaviour
    {
        //It is a small script , but I created for purposes of possible extensibility .
        //For these simple clicks I am using Unity's Event Trigger component 
        //For complex clicks the Command Pattern can be implemented
        //I have an example here :
        //https://github.com/tpaslou/Command-Patern-Movement-Unity
        //For a state driven movement with animations I would use FSM
        //https://github.com/tpaslou/Finite-State-Machine---Unity
        public void NodeClicked()
        { 
            if (!(gameObject.CompareTag("Water")))
                GameEvents.instance.NodeClick(this.gameObject.GetComponent<StarNode>());
        }
    }
}