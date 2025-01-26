using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuddyDoor : MonoBehaviour
{
    [SerializeField] BoxCollider doorCollider;
    Animator animator;
    int openBoolHash;

    [SerializeField]
    bool isOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        openBoolHash = Animator.StringToHash("IsOpen");
	}

    public void Open()
    {
        isOpen = true;
    }

    public void Close()
    {
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool(openBoolHash, isOpen);
        doorCollider.enabled = !isOpen;
    }
}
