using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;

namespace Framework {
    public class Floor : MonoBehaviour
    {
        [Header("Size")]
        [Tooltip("Width of grid")]
        public int width;

        [Tooltip("Height of grid")]
        public int height;

        public int tileSize;

        public GameObject[,] floorTileGOArr;
        public bool isInitialized = false;
        private GameObject grid;
        private int numLines = 0;
        private Levels levels = new Levels();
        private Material tileMat;
        public Floor(){}

        void Awake(){
            grid = new GameObject();
            grid.name = "Tile Master GO";
            grid.transform.parent = gameObject.transform;

            floorTileGOArr = new GameObject[width, height];
            isInitialized  = true;
        }

        public GameObject[,] getFloorTileGOArr(){
            return floorTileGOArr;
        }

        void generateFloor(){
            
            GameObject tileHolder = new GameObject();
            tileHolder.name = "Grid Tile Holder";
            tileHolder.transform.parent = grid.transform;

            for (int row = 0; row < height; row++){
                for (int col = 0; col < width; col++){
                    GameObject tile = new GameObject(); // Create holder gameobject and add a FloorTile class to it.
                    FloorTile ftComp = tile.AddComponent<FloorTile>() as FloorTile;
                    ftComp.setProps(col, row, tileSize, this); //Set properties
                    ftComp.setTileGO(tile); //set reference to parent GO
                    tile.name = "TILE - "+col+","+row;
                    tile.tag = "tile-moveable";
                    tile.transform.parent = tileHolder.transform; // Rename and organize
                    tile.transform.position = transform.position + ftComp.getRelativePos(); // Move tile gameobject to it's correct position as defined inside FloorTile class

                    ftComp.drawTile(); // Call long ass method inside the FloorTile to draw a mesh 

                    MeshCollider tileMc = tile.AddComponent<MeshCollider>() as MeshCollider;
                    
                    floorTileGOArr[col,row] = tile; // Add tile to gameobject array
                }
            }

        }
        void generateAdjacents(){
            for (int x = 0; x < width;x++){
                for (int y = 0; y < height; y++){
                    floorTileGOArr[x,y].GetComponent<FloorTile>().generateAdjacent();
                }
            }
        }
        void drawGrid(){
            // +Z is UP, +X is RIGHT
            GameObject lineGrid = new GameObject();
            lineGrid.transform.parent = gameObject.transform; //pointless, linerenderer is static
            lineGrid.name = "GridLines Holder";
            Color lineColor = new Color(0f, 0f, 0f, 0.8f);
            Vector3 upMod = new Vector3(0,0,tileSize);
            Vector3 rightMod = new Vector3(tileSize,0,0);

            //Draw up line, draw right line for each cell
            for (int col = 0; col < width; col++){
                for (int row = 0; row < height; row++){
                    Vector3 lineRoot = transform.position + floorTileGOArr[col,row].GetComponent<FloorTile>().getRelativePos();
                    //draw up
                    drawLine(lineRoot, (lineRoot + upMod), lineColor, lineGrid);
                    //draw right
                    drawLine(lineRoot, (lineRoot + rightMod), lineColor, lineGrid);
                }
            }

            //draw top and right big side
            // Are the getComponent calls the best way? seems heavy
            Vector3 topLeft = floorTileGOArr[0,height-1].GetComponent<FloorTile>().getRelativePos() + transform.position + upMod;
            Vector3 topRight = floorTileGOArr[width-1,height-1].GetComponent<FloorTile>().getRelativePos() + transform.position + upMod + rightMod;
            Vector3 bottomRight = floorTileGOArr[width-1,0].GetComponent<FloorTile>().getRelativePos() + transform.position + rightMod;
            //draw top
            drawLine(topLeft, topRight, lineColor, lineGrid);
            //draw right
            drawLine(topRight, bottomRight, lineColor, lineGrid);

            // Add lineGrid to Grid gameObject
            lineGrid.transform.parent = grid.transform;
        }

        void drawLine(Vector3 start, Vector3 end, Color color, GameObject parentObj)
        {
            numLines++;
            float lineWidth = 0.1f;
            GameObject myLine = new GameObject();
            myLine.name = "GridLine-"+numLines;
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Standard Surface"));
            lr.SetColors(color, color);
            lr.SetWidth(lineWidth, lineWidth);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            myLine.transform.parent = parentObj.transform;
        }

        void drawBoundaries(){
            int[][] currentLevel = levels.level1;

            foreach (int[] boundaryTile in currentLevel) {
                floorTileGOArr[boundaryTile[0],boundaryTile[1]].tag = "tile-blocked";
                floorTileGOArr[boundaryTile[0],boundaryTile[1]].GetComponent<FloorTile>().hasSpace = false;
                floorTileGOArr[boundaryTile[0],boundaryTile[1]].GetComponent<MeshRenderer>().material = tileMat;
            }
        }

        void drawMap(){
            //create a plane
             GameObject mapPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
             mapPlane.GetComponent<Renderer>().material = Resources.Load<Material>("props/mapMat");
             mapPlane.transform.position = gameObject.transform.position;
             mapPlane.transform.position = new Vector3(6.64f,-0.02f,-5.93f);
             mapPlane.transform.rotation = new Quaternion(0f,0.707106829f,0f,-0.707106829f);
             mapPlane.transform.localScale = new Vector3(2.915f,1.0f,1.65f);
        }

        void Start(){
            tileMat = new Material(Shader.Find("Standard"));
            tileMat.SetColor("_Color", new Color(0,0,0,0.8f));
            tileMat.SetFloat("_Mode", 3);
            tileMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tileMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tileMat.EnableKeyword("_ALPHABLEND_ON");
            tileMat.renderQueue = 3000;
            generateFloor();
            drawGrid();
            drawBoundaries();
            drawMap();
            generateAdjacents();
        }

        void Update(){

        }
    }
}