using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathing;
using UnityEngine;
using static Pathing.AStar;

namespace AStar
{
    //Game Manager is the controller of
    //scripts created for this assignment.
    //U can change grid size and coloring of 
    //nodes on the inspector
    //Another aproach could be singleton 
    //for the game manager to gain global access
    //an simplicity since this project isnt complex, 
    //but this pattern is controversial and bad practise
    //for extensibility. I avoided drag and drop referencing
    //because it creates tough coupling.
    public class GameManager : MonoBehaviour
    {
        #region Fields
        
        [Header("Grid Size")]
        [SerializeField]
        private int width=10;
        [SerializeField]
        private int height=10;
        private StarNode[,] GridArray;
        [Space]
        private StarNode StartPoint, EndPoint;
        [Tooltip("This is a timer used for animating path")]
        [SerializeField]
        [Range(0f,1f)]
        private float coloringTimer=0.2f;
        private IList<IAStarNode> path;
        [SerializeField]
        private Transform NodePrefab;
        private bool once;
        [SerializeField] private Transform textPrefab;
        private int cost = 0;
        #endregion
        
        void Start()
        {
            once = true;
            GridArray = new StarNode[height,width];
            path=new List<IAStarNode>();
            GameEvents.instance.onNodeClick += OnNodeClick;
            CreateGrid();
        }
        
        void Update()
        {
            CheckForPath();
        }

        #region Methods

        private void CreateGrid()
        {
            GridArray = gameObject.AddComponent<Grid>().GenerateGrid(width,height,NodePrefab);
        } 
        private void CheckForPath()
        {
            //if 2 nodes are clicked , calculate path
            if (StartPoint != null && EndPoint != null && once)
            {
                once = false;
                path = GetPath(StartPoint, EndPoint);
                if (path != null)
                {
                    cost = 0;
                    CalculatePathCost();
                    StartCoroutine(PathColorChangeRoutine(Color.red));
                }
                else
                {
                    ClearPath();
                    StartCoroutine(ShowMessage());
                }
            }
        }
        
        //Clears Path and Makes Grid Reusable
        public void ClearPath()
        {
            GameEvents.instance.CostUpdate(0);
            //In case we tried to travel through water 
            if (StartPoint) StartPoint.ChangeColor(Color.white);
            if(EndPoint) EndPoint.ChangeColor(Color.white);
            StartPoint = EndPoint = null; 
    
            if (path != null)
            {
                StartCoroutine(PathColorChangeRoutine(Color.white));
                path.Clear();
            }
            once = true;

        }

        private void CalculatePathCost()
        {
            foreach (StarNode var in path)
            {
                cost += var.GetNodeCost();
            }
            GameEvents.instance.CostUpdate(cost);
        }

        private IEnumerator PathColorChangeRoutine(Color color)

        {
            //Cloning for keeping values when path is cleared
            IList<IAStarNode> tempPath = path.ToList();
            foreach (var aStarNode in tempPath)
            {
                var nd = (StarNode) aStarNode;
                cost += nd.GetNodeCost();
                if (nd != StartPoint && nd != EndPoint)
                    nd.ChangeColor(color);
                yield return new WaitForSeconds(coloringTimer);
            }
        }

        private void OnNodeClick(StarNode node)
        {
            if (StartPoint == null)
            {
                StartPoint = node;
                StartPoint.ChangeColor(Color.green);
                Debug.Log("Start point : "+node);
            }
            else if (EndPoint == null)
            {
                EndPoint = node;
                EndPoint.ChangeColor(Color.green);
                Debug.Log("End point : "+node);

            }
            else
            { 
                ClearPath();
            }
        }

        IEnumerator ShowMessage()
        {
            Transform message = Instantiate(textPrefab,
                new Vector3(-11, 2, -1.4f) + gameObject.transform.position, Quaternion.Euler(90,0,0));
            yield return new WaitForSeconds(1f);
            Destroy(message.gameObject);
            
        }
        
        #endregion
    }
}