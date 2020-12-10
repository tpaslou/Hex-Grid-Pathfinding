using System;
using System.Collections.Generic;
using System.Linq;
using Pathing;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace AStar
{
    public enum NodeType 
    {
        Grass,
        Desert,
        Mountain,
        Forest,
        Water
    }
    
    public class StarNode : MonoBehaviour , IAStarNode 
    {

        #region Fields
        
        [SerializeField]
        private int cost;
        [SerializeField]
        private int x, y;
        [SerializeField]
        private NodeType nodeType;
        private StarNode node=null;//using this for neighbours
        [SerializeField]
        private  MeshRenderer _renderer;
        private StarNode[,] gridArray;
        private List<IAStarNode> neighbors;
        private bool visited;
        private int width, height;
        #endregion
        
        
        
        #region Methods
        //Inherits monobehaviour so init cant happen with contructor
        public void Initialize(int _x , int _y , NodeType _type , int _width , int _height , StarNode[,] grid)
        {
            neighbors=new List<IAStarNode>();
            width = _width;
            height = _height;
            x = _x;
            y = _y;
            nodeType = _type;
            _renderer = gameObject.GetComponent<MeshRenderer>();
            visited = false;
            switch (_type)
            {
                case NodeType.Grass :
                {
                    cost = 1;
                    break;
                }
                case NodeType.Desert :
                {
                    cost = 5;
                    break;
                }
                case NodeType.Mountain :
                {
                    cost = 10;
                    break;
                }
                case NodeType.Forest :
                {
                    cost = 3;
                    break;
                }
                case NodeType.Water :
                {
                    //Well we could let it empty too.
                    cost = -1;   
                    break;
                }
                default:
                    Debug.LogError("Error on Node creation , check constructor values.");
                    break;   
            }
            
            //Arrays in C# are reference type , so a copy of the reference of the original array will work
            //even if the whole grid isn't initialized
            gridArray = grid;
        }
        
        public  NodeType RandomNodeType()
        {
            return (NodeType) Random.Range(0, 5);
        }
        
        //Changes color in the node
        public void ChangeColor(Color color)
        {
            _renderer.material.SetColor("_Color", color);
        }

        public int GetNodeCost()
        {
            return cost;
        }
        //Is it null or water node ? 
        private bool IsValidNeighbor(StarNode nd)
        {
            if (nd != null && nd.nodeType != NodeType.Water)
            {
                return true;
            }
            return false;
        }
        
        //The neighbours property returns an enumeration of all the nodes adjacent to this node
        public IEnumerable<IAStarNode>	Neighbours
        {
            get
            {
                if (!visited)
                {
                    int xOffset = 0;
                    if (y % 2 != 0)
                        xOffset = 1;

                    //up left
                    if ((y > 0 && x > 0) || (xOffset == 1 && y < height - 1) ||
                        (y == height - 1 && xOffset == 1))
                        node = gridArray[y - 1, x + xOffset - 1];
                    if (IsValidNeighbor(node)) neighbors.Add(node);

                    //up right
                    if ((y > 0 && x < width - 1) || (xOffset == 1 && x < width - 1) ||
                        (xOffset == 0 && x == width - 1 && y > 0))
                        node = gridArray[y - 1, x + xOffset];
                    if (IsValidNeighbor(node)) neighbors.Add(node);

                    //mid left
                    if (x > 0) node = gridArray[y, x - 1];
                    if (IsValidNeighbor(node)) neighbors.Add(node);

                    //mid right
                    if (x < width - 1) node = gridArray[y, x + 1];
                    if (IsValidNeighbor(node)) neighbors.Add(node);

                    //down left    
                    if ((y < height - 1 && x > 0) || (xOffset == 1 && y < height - 1) ||
                        (xOffset == 0 && x == width - 1))
                        node = gridArray[y + 1, x + xOffset - 1];
                    if (IsValidNeighbor(node)) neighbors.Add(node);

                    //down right
                    if ((y < height - 1 && x < width - 1) || (xOffset == 0 && x == width - 1))
                        node = gridArray[y + 1, x + xOffset];
                    if (IsValidNeighbor(node)) neighbors.Add(node);
                    visited = true;
                }

                if (neighbors.Any())
                {
                    foreach (IAStarNode nd in neighbors)
                    {
                        yield return nd;
                    }
                }

            }
        }


        // this function should calculate the exact cost of travelling from this node to "neighbour".
        // when the A* algorithm calls this function, the neighbour parameter is guaranteed to be one of the nodes in 'Neighbours'. 
        // 'cost' could be distance, time, anything countable, where smaller is considered better by the algorithm
        public float CostTo(IAStarNode neighbour)
        {
            //recursive trick to check neighbour and cost 
            //with IAStarNode parameter and not StarNode
            if (Neighbours.Contains(neighbour))
            {
                return neighbour.CostTo(neighbour);
            
            }
            if (neighbour == this)
            {
                return cost;
            }
            return 0;
        }

        // this function should estimate the distance to travel from this node to "goal". goal may be
        // any node in the graph, so there is no guarantee it is a direct neighbour. The better the estimation
        // the faster the AStar algorithm will find the optimal route. Be careful however, that the cost of calculating
        // this estimate doesn't outweigh any benefits for the AStar search. 
        //
        // this estimation could be distance, time, anything countable, where smaller is considered better by the algorithm
        // the estimate needs to 'consistent' (also know as 'monotone')
        public float EstimatedCostTo(IAStarNode goal)
        {
            /* Based on Stanford's article regarding Heuristics
         on hexagonal grid, I followed Manhattan distance
         modified
         http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
         https://www.redblobgames.com/grids/hexagons/#distances
        */
            StarNode end = (StarNode) goal;
            int x1 = x;//row
            int x2 = end.x;
            int y1 = y;//col
            int y2 = end.y;
            int ecost = 0;
            //transform to cube coordinates
            int cx1 = y1 - (x1 - (x1 % 2)) / 2;
            int cx2 =  y2 - (x2 - (x2 % 2)) / 2;
            int cz1 = x1;
            int cz2 = x2;
            int cy1 = -cx1 - cz1;
            int cy2 = -cx2 - cz2;
        
            //cube distance
            int distance =  Math.Max(Math.Abs(cx1 - cx2), Math.Abs(cy1 - cy2));
            distance = Math.Max(distance , Math.Abs(cz1 - cz2));
        
            //Theory says Multiply the distance in steps by the minimum cost for a step
            //The minimum cost is 1 , but when tested with cost 3 , the returning estimated
            //cost was more accurate and mostly close to the real cost.
            ecost = distance * 3;
            return ecost;
        }
        
        #endregion

    }
   
}