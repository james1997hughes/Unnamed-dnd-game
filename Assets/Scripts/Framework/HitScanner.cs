using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class HitScanner : MonoBehaviour
    {

        GameObject lastGameObjectHit;
        RaycastHit lastHit;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out lastHit))
            {
                lastGameObjectHit = lastHit.collider.gameObject;
            }
        }

        public GameObject ThingHit()
        {
            return lastGameObjectHit;
        }

        public RaycastHit GetLastHit(){
            return lastHit;
        }
    }
}
