using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Framework{
    public class FloorTile : MonoBehaviour
    {

        public int row;
        public int col;
        public Vector3 relativePos;
        public bool hasSpace;
        private Floor parent;
        private int tileSize;
        private GameObject tileGO;

        Material tileMat;
        Color tileColor;

        MeshRenderer meshRenderer;


        public FloorTile(int xpos, int ypos, int tileWidth, Floor parentFloor){
            setProps(xpos, ypos, tileWidth, parentFloor); // Saucy
        }

        public void setProps(int xpos, int ypos, int tileWidth, Floor parentFloor){
            col = xpos;
            row = ypos;
            parent = parentFloor;
            tileSize = tileWidth;

            int relativex = col * tileSize;
            int relativez = row * tileSize;
            relativePos = new Vector3(relativex, 0, relativez);
        }

        public void setColor(Color color){
            meshRenderer.sharedMaterial.SetColor("_Color", color);
        }

        void Awake() {
            hasSpace = true;
            tileColor = Color.red;
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
    }

}

