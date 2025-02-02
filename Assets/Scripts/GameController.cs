using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject xrRig;
    [SerializeField] FloorDoorScript floorDoorScript;

    TitleAndCreditController titleCreditsController;

    [SerializeField] bool debugFloorActivate;

    // AI buddy hello sequence vars
    [SerializeField] GameObject buddyDeployButtonContainer;
    [SerializeField] BuddyDoor buddyDoor;
    bool isBuddyDeploying = false;
    Coroutine buddyDeployCoroutine;

    [SerializeField]
    GameObject attachmentHands;

    Cubesat cutesat;

	private void Awake()
	{
		cutesat = FindObjectOfType<Cubesat>();
        titleCreditsController = FindObjectOfType<TitleAndCreditController>();
	}


	// Start is called before the first frame update
	void Start()
    {
        
    }

    IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(150);
        titleCreditsController.showCredits();
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

    public void StartBuddyDeploy()
    {
        if(!isBuddyDeploying)
        {
            isBuddyDeploying = true;
            buddyDeployCoroutine = StartCoroutine(DeployBuddySequence());
        }
    }

    IEnumerator DeployBuddySequence()
    {
        const float buddyDoorWaitTime = 1;

        // open the door
        buddyDoor.Open();

        yield return new WaitForSeconds(buddyDoorWaitTime);

        buddyDeployButtonContainer.SetActive(false);
        

        // get our buddy to move out of the door

        const float buddyEmergeTime = 1;
        cutesat.StartDialog("emerge");

        yield return new WaitForSeconds(buddyEmergeTime);

        buddyDoor.Close();

        attachmentHands.SetActive(true);
    }
}
