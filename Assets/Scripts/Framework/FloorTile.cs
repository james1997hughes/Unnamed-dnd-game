using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework{
    public class FloorTile : MonoBehaviour
    {

        public int row;
        public int col;
        public string name;
        public Vector3 relativePos;
        public bool hasSpace;
        private Floor floor;
        private int tileSize;
        public GameObject tileGO;

        public double g;
        public double h;
        public double f;
        ArrayList adjacentTiles;
        public FloorTile pathParent = null;


        Material tileMat;
        Color tileColor;

        MeshRenderer meshRenderer;


        public FloorTile(int xpos, int ypos, int tileWidth, Floor parentFloor){
            setProps(xpos, ypos, tileWidth, parentFloor); // Saucy
        }

        public void setProps(int xpos, int ypos, int tileWidth, Floor parentFloor){
            col = xpos;
            row = ypos;
            name = $"col:{col}row:{row}";
            floor = parentFloor;
            tileSize = tileWidth;

            int relativex = col * tileSize;
            int relativez = row * tileSize;
            relativePos = new Vector3(relativex, 0, relativez);
        }

        public void setColor(Color color){
            tileMat.SetColor("_Color", color);
        }

        void Awake() {
            hasSpace = true;
            tileColor = Color.red;
        }

        void Start(){
        }
        public ArrayList getAdjacent(){
            return adjacentTiles;
        }

        public void generateAdjacent(){
            adjacentTiles = new ArrayList();
            GameObject[,] floorTileGOArr = floor.getFloorTileGOArr();
            if (col > 0){ 
                adjacentTiles.Add(floorTileGOArr[col-1,row].GetComponent<FloorTile>()); //Left
                if (row > 0){
                    adjacentTiles.Add(floorTileGOArr[col-1,row-1].GetComponent<FloorTile>()); //Bottom-left
                }
                if (row < floor.height-1){
                    adjacentTiles.Add(floorTileGOArr[col-1,row+1].GetComponent<FloorTile>()); //Top-left
                }
            }
            if (row > 0){
                adjacentTiles.Add(floorTileGOArr[col,row-1].GetComponent<FloorTile>()); // Bottom
            }
            if (row < floor.height-1){

                    adjacentTiles.Add(floorTileGOArr[col,row+1].GetComponent<FloorTile>());
                    //Debug.Log(floorTileGOArr[col,row+1].GetComponent<FloorTile>().row);

            }

            if (col < floor.width-1){
                //Debug.Log($"COl: {col} floor.width: {floor.width}");
                adjacentTiles.Add(floorTileGOArr[col+1,row].GetComponent<FloorTile>()); //Right
                if (row > 0){   
                    adjacentTiles.Add(floorTileGOArr[col+1,row-1].GetComponent<FloorTile>()); //Bottom-right
                }
                if (row < floor.height-1){
                    adjacentTiles.Add(floorTileGOArr[col+1,row+1].GetComponent<FloorTile>());//Top-right
                }
            }
        }

        /*void OnMouseOver(){
            meshRenderer.sharedMaterial.SetColor("_Color", Color.blue);
        }*/

        /*void OnMouseExit(){
            StartCoroutine(waitThenColor());
        }
        IEnumerator waitThenColor() {
            yield return new WaitForSeconds(0f);
            meshRenderer.sharedMaterial.SetColor("_Color", Color.red);
        }*/
        //new Color(0.39f, 0.976f, 0.71f, 0.63f)

        public void drawTile(){
            Vector3 upMod = new Vector3(0,0,tileSize); // Define handy dandy vectors
            Vector3 rightMod = new Vector3(tileSize,0,0);
            Vector3 rootPoint = new Vector3(0,0,0); // Root here is 0,0,0 as we want the grid tiles to spawn from the botleft root of the Floor/FloorManager

            meshRenderer = gameObject.AddComponent<MeshRenderer>(); 
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>(); // Add components to render ~~A~~ mesh

            tileMat = new Material(Shader.Find("Standard"));
            tileMat.SetColor("_Color", new Color(0,0,0,0.35f));
            tileMat.SetFloat("_Mode", 3);
            tileMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            tileMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tileMat.EnableKeyword("_ALPHABLEND_ON");
            tileMat.renderQueue = 3000;
            meshRenderer.sharedMaterial = tileMat;
            //meshRenderer.sharedMaterial.SetColor("_Color", tileColor); // Potentially use a texture later on.
            Mesh mesh = new Mesh(); // Now we create a new mesh by defining it's geometry.

            Vector3[] vertices = new Vector3[4]{ // Corners. In this case, it's a 2 dimensional plane, so only 4 corners.
                rootPoint, //Bottom Left
                rootPoint + rightMod, // Bottom Right
                rootPoint + upMod, // Top Left
                rootPoint + upMod + rightMod // Top right
            };
            
            int[] tris = new int[6]{ // Define triangles in the mesh.
                0,2,1, // First triangle, leads directly onto second
                2,3,1 // Second
            };
            
            Vector3[] normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward, 
                -Vector3.forward,
                -Vector3.forward
            };
                                            //Who even knows what the FUCK these two are
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.vertices = vertices;
            mesh.triangles = tris; // Add all our shit to build the mesh
            mesh.normals = normals;
            mesh.uv = uv;
            
            meshFilter.mesh = mesh; //Lastly give our mesh to the meshFilter on the GO
        }

        public void setTileGO(GameObject newTileGO){
            tileGO = newTileGO;
        }
        public GameObject getTileGO(){
            return tileGO;
        }
        public Vector3 getRelativePos(){
            return relativePos;
        }
        public int getRow(){
            return row;
        }
        public int getCol(){
            return col;
        }
        public int getTileSize(){
            return tileSize;
        }

        public bool equals(FloorTile challenge){
            if (this.row == challenge.row && this.col == challenge.col){
                return true;
            }else{
                return false;
            }
        }
    }

}

