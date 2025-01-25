using Leap;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerBox : MonoBehaviour
{
    [SerializeField] Anchor socketA;
    [SerializeField] Anchor socketB;
    PowerCell cellInSocketA;
    PowerCell cellInSocketB;

    private bool isPowered = false;
    private bool isOpen = true;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        socketA.OnAnchorablesAttached += RefreshSocketA;
        socketB.OnAnchorablesAttached += RefreshSocketB;

    }

    void RefreshSocketA()
    {
        if(socketA && socketA.hasAnchoredObjects)
        {
            PowerCell candidateCell = socketA.anchoredObjects.First().GetComponent<PowerCell>();
            if(candidateCell) cellInSocketA = candidateCell;
        }
        else
        {
            cellInSocketA = null;
        }
    }

    void RefreshSocketB()
    {
        if(socketB && socketB.hasAnchoredObjects)
        {
            PowerCell candidateCell = socketB.anchoredObjects.First().GetComponent<PowerCell>();
            if(candidateCell) cellInSocketB = candidateCell;
        }
        else
        {
            cellInSocketB = null;
        }
    }

    bool SocketState(bool checkA)
    {
        PowerCell cellToCheck = (checkA) ? cellInSocketA : cellInSocketB;

        return cellToCheck != null && !cellToCheck.Broken;
    }

    bool GetPoweredState()
    {
        return SocketState(true) && SocketState(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool previousPowered = isPowered;
        bool nowPowered = GetPoweredState();

        if(nowPowered != previousPowered)
        {
            // dispatch any change events
        }

        isPowered = nowPowered;
    }
}
