using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class MovementHandler : MonoBehaviour
    {
        public bool inMovement = false;

        public bool firstTileSet = false;

        bool isInitialized = false;

        FloorTile startTile;

        HitScanner hitScanner;

        ArrayList pathGameObjects = new ArrayList();

        ArrayList previousPath = new ArrayList();

        Floor floor;

        GameObject[,] floorGrid;

        GameObject prevThing;

        int floorWidth;
        int floorHeight;

        int pfDelay = 50;
        int pfDelayCounter = 0;

        void Start()
        {
            hitScanner = gameObject.GetComponent<HitScanner>();
            floor = GameObject.FindWithTag("floor").GetComponent<Floor>();
            floorWidth = floor.width;
            floorHeight = floor.height;
        }

        // Update is called once per frame
        void Update()
        {
            pfDelayCounter++;
            if(floor.isInitialized && !isInitialized){
                floorGrid = floor.getFloorTileGOArr();
                isInitialized = true;
            }
            if (inMovement)
            {
                GameObject thingHit = hitScanner.ThingHit();

                if (thingHit)
                {
                        if (thingHit != prevThing && thingHit.tag == "tile-moveable"){
                            pathfind(startTile, thingHit.GetComponent<FloorTile>());
                            prevThing = thingHit;
                        }
                }
            }
            else
            {
                DestroyPath();
            }
        }

        void DestroyPath()
        {
            if (pathGameObjects.Count > 0)
            {
                foreach (GameObject go in pathGameObjects)
                {
                    go
                        .GetComponent<MeshRenderer>()
                        .sharedMaterial
                        .SetColor("_Color", new Color(0,0,0,0.35f));
                }
                pathGameObjects.Clear();
            }
        }

        //TODO Fix this
        void pathfind(FloorTile startingTile, FloorTile endingTile){
            SimplePriorityQueue<FloorTile, float> openList = new SimplePriorityQueue<FloorTile,float>();
            ArrayList closedList = new ArrayList();

            openList.Enqueue(startingTile, 0);

            while(openList.Count > 0) {

                FloorTile currentNode = openList.Dequeue();
                closedList.Add(currentNode);
                //Debug.Log(currentNode.name);

                if (currentNode.equals(endingTile)){
                    Debug.Log("Path Found!");
                    drawPath(currentNode);
                    break;
                }

                ArrayList children = currentNode.getAdjacent();
                //Debug.Log($"children: {children.Count}");

                foreach (FloorTile child in children){
                    
                    if (closedList.Contains(child) || !child.hasSpace){
                        continue;
                    }
                    child.pathParent = currentNode;

                    child.g = currentNode.g + 1;
                    child.h = ((endingTile.row - child.row) * (endingTile.row - child.row)) + ((endingTile.col - child.col)*(endingTile.col - child.col));
                    child.f = child.g + child.h;

                    //Debug.Log($"g: {child.g}, h:{child.h}, f:{child.h}");
                    if (openList.Contains(child)){
                        if (openList.GetExistingNode(child).Data.g < child.g){
                            continue;
                        }
                    }
                    openList.Enqueue(child, (float)child.f);
                }
            }
        }

        void drawPath(FloorTile endingTile){
            ArrayList path = new ArrayList();
            FloorTile current = endingTile;
            while(current.pathParent != null){
                path.Add(current);
                current = current.pathParent;
            }
            if (previousPath.Count > 0){
                foreach (FloorTile f in previousPath){
                    f.tileGO.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", new Color(0,0,0,0.35f));
                }
            }
            if (path.Count > 0){
                String pathStr = "";
                foreach (FloorTile f in path){
                    pathStr += f.name + " ";
                    f.tileGO.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", new Color(0,0,0,0.7f));
                }
                Debug.Log(pathStr);
            }
            previousPath = path;
        }

        public void setStartTile(FloorTile startTile)
        {
            this.startTile = startTile;
        }
    }
}
