using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework {
    public class Floor : MonoBehaviour
    {
        [Header("Size")]
        [Tooltip("Width of grid")]
        public int width;

        [Tooltip("Width of grid")]
        public int height;

        public int tileSize;

        private GameObject[,] floorTileGOArr;
        private GameObject grid;
        private int numLines = 0;
        public Floor(){}

        void Awake(){
            grid = new GameObject();
            grid.name = "Tile Master GO";
            grid.transform.parent = gameObject.transform;

            floorTileGOArr = new GameObject[height, width];
        }

        void generateFloor(){
            
            GameObject tileHolder = new GameObject();
            tileHolder.name = "Grid Tile Holder";
            tileHolder.transform.parent = grid.transform;

            for (int row = 0; row < height; row++){
                for (int col = 0; col < width; col++){
                    GameObject tile = new GameObject(); // Create holder gameobject and add a FloorTile class to it.
                    FloorTile ftComp = tile.AddComponent<FloorTile>() as FloorTile;
                    ftComp.setProps(row, col, tileSize, this); //Set properties because we can't do this during the AddComponent?
                    tile.name = "TILE - "+row+","+col;
                    tile.tag = "tile";
                    tile.transform.parent = tileHolder.transform; // Rename and organize
                    tile.transform.position = transform.position + ftComp.getRelativePos(); // Move tile gameobject to it's correct position as defined inside FloorTile class

                    ftComp.drawTile(); // Call long ass method inside the FloorTile to draw a mesh 

                    MeshCollider tileMc = tile.AddComponent<MeshCollider>() as MeshCollider;
                    
                    floorTileGOArr[row,col] = tile; // Add tile to gameobject array
                }
            }
        }
        void drawGrid(){
            // +Z is UP, +X is RIGHT
            GameObject lineGrid = new GameObject();
            lineGrid.name = "GridLines Holder";
            Color lineColor = Color.magenta;
            Vector3 upMod = new Vector3(0,0,tileSize);
            Vector3 rightMod = new Vector3(tileSize,0,0);

            //Draw up line, draw right line for each cell
            for (int row = 0; row < height; row++){
                for (int col = 0; col < width; col++){
                    Vector3 lineRoot = transform.position + floorTileGOArr[row,col].GetComponent<FloorTile>().getRelativePos();
                    //draw up
                    drawLine(lineRoot, (lineRoot + upMod), lineColor, lineGrid);
                    //draw right
                    drawLine(lineRoot, (lineRoot + rightMod), lineColor, lineGrid);
                }
            }

            //draw top and right big side
            // Are the getComponent calls the best way? seems heavy
            Vector3 topLeft = floorTileGOArr[height-1,0].GetComponent<FloorTile>().getRelativePos() + transform.position + upMod;
            Vector3 topRight = floorTileGOArr[height-1,width-1].GetComponent<FloorTile>().getRelativePos() + transform.position + upMod + rightMod;
            Vector3 bottomRight = floorTileGOArr[0,width-1].GetComponent<FloorTile>().getRelativePos() + transform.position + rightMod;
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
            float lineWidth = 0.2f;
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

        void Start(){
            generateFloor();
            drawGrid();
        }

        void Update(){

        }
    }
}