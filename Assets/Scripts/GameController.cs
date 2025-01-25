using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject xrRig;
    [SerializeField] FloorDoorScript floorDoorScript;

    [SerializeField] bool debugFloorActivate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(debugFloorActivate)
        {
            debugFloorActivate = false;
            floorDoorScript.Open();
        }

        if(floorDoorScript.IsMoving)
        {
            xrRig.transform.position = floorDoorScript.rigConstraint.transform.position;
        }
    }
}
