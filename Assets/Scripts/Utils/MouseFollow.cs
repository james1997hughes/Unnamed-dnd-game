using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class MouseFollow : MonoBehaviour
{
    public GameObject floorGrid;
    GameObject cam;
    Vector3 worldPosition;
    InventoryItem[] placeables;
    InventoryItem currentItem;
    int currentItemIndex;
   

    
    // Start is called before the first frame update
    void Start()
    {
        GameObject cam = GameObject.Find("Main Camera");
        gameObject.transform.position = floorGrid.transform.position;
        worldPosition = new Vector3(0,0,0);

        placeables = new InventoryItem[3]; // TODO Find a better way to load in items
        placeables[0] = new InventoryItem("Armchair", Resources.Load<Mesh>("props/armchair"), Resources.Load<Material>("props/chairMat"));
        placeables[1] = new InventoryItem("Barrel", Resources.Load<Mesh>("props/barrel"), Resources.Load<Material>("props/barrelMat"));
        placeables[2] = new InventoryItem("Candelier", Resources.Load<Mesh>("props/Candelier"), Resources.Load<Material>("props/candelMat"));

        currentItemIndex = 0;
        currentItem = placeables[currentItemIndex];
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler();
        MoveAndPlaceHandler();
    }

    void MoveAndPlaceHandler(){
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 newPos;
        currentItem = placeables[currentItemIndex];
        gameObject.GetComponent<MeshFilter>().mesh = currentItem.mesh;
        gameObject.GetComponent<MeshRenderer>().material = currentItem.mat;

        // Check what mouse is pointing at
        if (Physics.Raycast(ray, out hit)){
            worldPosition = hit.point;
            newPos = new Vector3(worldPosition.x, floorGrid.transform.position.y + 0.2f, worldPosition.z);
            GameObject thingHit = hit.collider.gameObject;

            if (thingHit.tag == "tile"){
                FloorTile tileHit = thingHit.GetComponent<FloorTile>();
                float tileSize = (float)tileHit.getTileSize(); //This could be cached... won't change

                newPos = thingHit.transform.position;
                newPos.z += (tileSize/2);
                newPos.x += (tileSize/2); // Center of tile

                if (Input.GetButtonDown("Place Item") && tileHit.hasSpace){
                    GameObject placedItem = Instantiate(gameObject, newPos, transform.rotation, thingHit.transform);
                    Destroy(placedItem.GetComponent<MouseFollow>());
                    tileHit.hasSpace = false;
                }
            }
            transform.position = newPos; // Snap item to center of tile
        }
    }

    void InputHandler(){

        // Inventory controls
        if (Input.GetButtonDown("Next Item")){
            if (currentItemIndex < placeables.Length-1){
                currentItemIndex++;
            }
        }
        if (Input.GetButtonDown("Previous Item")){
            if (currentItemIndex > 0){
                currentItemIndex--;
            }
        }
        if (Input.GetButtonDown("Rotate Item")){
            transform.Rotate(0,0,45);
        }
    }

}
