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

        Floor floor;

        GameObject[,] floorGrid;


        void Start()
        {
            hitScanner = gameObject.GetComponent<HitScanner>();
            floor = GameObject.FindWithTag("floor").GetComponent<Floor>();
        }

        // Update is called once per frame
        void Update()
        {
            if(floor.isInitialized && !isInitialized){
                floorGrid = floor.floorTileGOArr;
                isInitialized = true;
            }
            if (inMovement)
            {
                GameObject thingHit = hitScanner.ThingHit();
                if (!firstTileSet)
                {
                    startTile.setColor(Color.blue);
                    firstTileSet = true;
                }

                if (thingHit)
                {
                    if (thingHit.tag == "tile-moveable")
                    {
                        pathGameObjects.Add (thingHit);
                        MeshRenderer mr = thingHit.GetComponent<MeshRenderer>();
                        mr.sharedMaterial.SetColor("_Color", new Color(0,0,0,0.7f));
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
                        .SetColor("_Color", Color.red);
                }
                pathGameObjects.Clear();
            }
        }

        private int calculateHeuristic(GameObject startGO, GameObject goalGO)
        {
            Debug.Log("");
            return 1;
        }

        public void setStartTile(FloorTile startTile)
        {
            this.startTile = startTile;
        }
    }
}
