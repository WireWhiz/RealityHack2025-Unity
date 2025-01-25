using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerBox : MonoBehaviour
{
    [SerializeField] Joint socketA;
    PowerCell cellInSocketA;

    [SerializeField] Joint socketB;
    PowerCell cellInSocketB;

    private void Awake()
    {

    }

	// Start is called before the first frame update
	void Start()
    {
        // get our starting cells
        cellInSocketA = GetCellInSocket(true);
        cellInSocketB = GetCellInSocket(false);
    }

    PowerCell GetCellInSocket(bool socketType)
    {
        Joint jointToAcquire = (socketType) ? socketA :
            socketB;

        // get the powercell associated with any rigidbody on the joint
        if(jointToAcquire && jointToAcquire.connectedBody)
        {
            PowerCell candidateCell = jointToAcquire.connectedBody.GetComponent<PowerCell>();

            if(candidateCell)
            {
                return candidateCell;
            }
        }

        return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
