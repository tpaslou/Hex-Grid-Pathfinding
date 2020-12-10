using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


namespace AStar
{
    public class Grid : MonoBehaviour
    {

        #region Fields

        [SerializeField]
        private Transform hexagonPrefab;
        private Vector3 startPosition;
        private Material material ;
        private StarNode starNode;
        [SerializeField]
        private  int width;
        [SerializeField]
        private  int height;
    
        float hexWidth = 1+0.1f;//This it the prefab scale value + offset for gaping
        float hexHeight = 1.0f+0.1f;

        //Another Way to do that would be Material Fields on this script 
        // & Drag n Drop to the inspector
        //each approach has it's pros and cons
        //Nevertheless the point is to load values once and this is
        //a different approach , commonly used in my C++ games.
        private Dictionary<NodeType, Material> nodeTextures;
        private StarNode[,] gridArray;
        
        #endregion

        #region Methods
        
        public StarNode[,] GenerateGrid(int _width,int _height,Transform prefab)
        {
            InitReferences(_width,_height, prefab);
            InitPosition();
            CreateGrid();
            return gridArray;
        }
        
        //Set local fields
        private void InitReferences(int _width,int _height,Transform prefab)
        {
            nodeTextures = new Dictionary<NodeType, Material>();
            //Storing Materials
            foreach (var value in Enum.GetValues(typeof(NodeType)).Cast<NodeType>())
            {
                nodeTextures.Add(value,Resources.Load(value.ToString(),typeof(Material)) as Material);
            }
            //Height and Width
            hexagonPrefab = prefab;
            width = _width;
            height = _height;
            gridArray = new StarNode[height,width];
            
        }
        //Start Grid Position
        private void InitPosition()
        {
            float offset = 0;
            if (height / 2 % 2 != 0)
                offset = hexWidth / 2;
 
            float x = -hexWidth * (width / 2) - offset;
            float z = hexHeight * 0.75f * (height / 2);
 
            startPosition = new Vector3(x, 0, z);
        }
        //Position on world space
        private Vector3 WorldPosition(Vector2 gridPos)
        {
            float offset = 0;
            if (gridPos.y % 2 != 0)
                offset = hexWidth / 2;
 
            float x = startPosition.x + gridPos.x * hexWidth + offset;
            float z = startPosition.z - gridPos.y * hexHeight * 0.75f;
 
            return new Vector3(x, 0, z);
        }
        //Creates grid on Screen
        public void CreateGrid()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //instansiate prefab
                    Transform hex = Instantiate(hexagonPrefab, this.transform, true);
                    Vector2 gridPos = new Vector2(x, y);
                    hex.position = WorldPosition(gridPos);
                    hex.name = "Hexagon" + y + "|" + x;
                    
                    //Adding Node Component
                    starNode = hex.gameObject.AddComponent<StarNode>();
                    NodeType t = starNode.RandomNodeType();
                    starNode.Initialize(x,y,t,width,height,gridArray);
                    hex.GetComponent<MeshRenderer>().material = nodeTextures[t];
                    hex.tag = t.ToString();
                    
                    gridArray[y, x] = starNode;
                }
            }
        }

        #endregion
    }
}