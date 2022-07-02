using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    public GameObject floorGrid;

    GameObject cam;

    Vector3 worldPosition;

    InventoryItem[] placeables;

    InventoryItem currentItem;

    int currentItemIndex;

    bool pickedUp;

    FloorTile tileHit;

    MovementHandler movementHandler;

    HitScanner hitScanner;

    MeshCollider propMC;

    // Start is called before the first frame update
    void Start()
    {
        pickedUp = true;
        GameObject cam = GameObject.Find("Main Camera");
        gameObject.transform.position = floorGrid.transform.position;
        worldPosition = new Vector3(0, 0, 0);

        placeables = new InventoryItem[4]; // TODO Find a better way to load in items
        placeables[0] =
            new InventoryItem("Armchair",
                Resources.Load<Mesh>("props/armchair"),
                Resources.Load<Material>("props/chairMat"));
        placeables[1] =
            new InventoryItem("Barrel",
                Resources.Load<Mesh>("props/barrel"),
                Resources.Load<Material>("props/barrelMat"));
        placeables[2] =
            new InventoryItem("Luden",
                Resources.Load<Mesh>("props/luden"),
                Resources.Load<Material>("props/ludenMat"));

        placeables[3] =
            new InventoryItem("Candelier",
                Resources.Load<Mesh>("props/Candelier"),
                Resources.Load<Material>("props/candelMat"));

        currentItemIndex = 0;
        currentItem = placeables[currentItemIndex];

        movementHandler = gameObject.GetComponent<MovementHandler>();
        hitScanner = gameObject.GetComponent<HitScanner>();
    }

    // Update is called once per frame
    void Update()
    {
        ModelRenderer();
        InputHandler();
        MoveAndPlaceHandler();
    }

    void ModelRenderer()
    {
        currentItem = placeables[currentItemIndex];
        if (
            gameObject.GetComponent<MeshFilter>().sharedMesh !=
            currentItem.mesh as Mesh
        )
        {
            gameObject.GetComponent<MeshFilter>().mesh = currentItem.mesh;
        }
        if (
            gameObject.GetComponent<Renderer>().sharedMaterial !=
            currentItem.mat as Material
        )
        {
            gameObject.GetComponent<Renderer>().material =
                currentItem.mat as Material;
        }
    }

    void MoveAndPlaceHandler()
    {
        GameObject thingHit = hitScanner.ThingHit();
        if (thingHit)
        {
            if (pickedUp)
            {
                Vector3 newPos;
                RaycastHit hit = hitScanner.GetLastHit();
                newPos =
                    new Vector3(hit.point.x,
                        floorGrid.transform.position.y + 0.2f,
                        hit.point.z);

                if (thingHit.tag == "tile-moveable")
                {
                    tileHit = thingHit.GetComponent<FloorTile>();
                    float tileSize = (float) tileHit.getTileSize(); //This could be cached... won't change

                    newPos = thingHit.transform.position;
                    newPos.z += (tileSize / 2);
                    newPos.x += (tileSize / 2); // Center of tile

                    if (Input.GetButtonDown("Place Item") && tileHit.hasSpace)
                    {
                        transform.position = thingHit.transform.position;
                        propMC =
                            gameObject.AddComponent<MeshCollider>() as
                            MeshCollider;
                        tileHit.hasSpace = false;
                        pickedUp = false;

                        movementHandler.setStartTile (tileHit);
                        movementHandler.inMovement = false;
                    }
                }
                transform.position = newPos; // Snap item to center of tile
            }
            else if (
                thingHit.tag == "character" && Input.GetButtonDown("Place Item")
            )
            {
                Destroy (propMC);
                tileHit.hasSpace = true;
                pickedUp = true;
                movementHandler.inMovement = true;
            }
        }
    }

    void InputHandler()
    {
        // Inventory controls
        if (Input.GetButtonDown("Next Item"))
        {
            if (currentItemIndex < placeables.Length - 1)
            {
                currentItemIndex++;
            }
        }
        if (Input.GetButtonDown("Previous Item"))
        {
            if (currentItemIndex > 0)
            {
                currentItemIndex--;
            }
        }
        if (Input.GetButtonDown("Rotate Item"))
        {
            transform.Rotate(0, 0, 45);
        }
    }
}
