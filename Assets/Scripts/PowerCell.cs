using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap.PhysicalHands;

public class PowerCell : MonoBehaviour
{
    private Rigidbody rigidBody;
    MeshRenderer cellRenderer;
    PhysicalHandsManager physHandsManager;

    [SerializeField] bool broken = false;

    private void Awake()
    {
        physHandsManager = FindObjectOfType<PhysicalHandsManager>();
        cellRenderer = GetComponent<MeshRenderer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        if(physHandsManager)
        {
            physHandsManager.onGrab.AddListener(OnGrabBegin);
            physHandsManager.onGrabExit.AddListener(OnGrabEnd);
        }
    }

    void OnGrabBegin(ContactHand hand, Rigidbody rbody)
    {

    }

    void OnGrabEnd(ContactHand contactHand, Rigidbody rbody)
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
