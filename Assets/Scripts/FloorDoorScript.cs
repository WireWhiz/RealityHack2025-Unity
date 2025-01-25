using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FloorDoorScript : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject floor;
    public float doorsMoveDistance;
    public float floorMoveDistance;    
    public float doorMoveTime;
    public float floorMoveTime;
    public float startDelay;
    public AudioSource leftDoorAudioSource;
    public AudioSource rightDoorAudioSource;
    public AudioSource floorAudioSource;
  
    // Start is called before the first frame update
    void Start()
    {
        floor.transform.Translate(0, -floorMoveDistance, 0);
        StartCoroutine(OpenDoorsAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OpenDoorsAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(OpenDoors());
    }

    IEnumerator OpenDoors()
    {
        if (!floorAudioSource.isPlaying)
        {
            floorAudioSource.Play();
        }

        if (!leftDoorAudioSource.isPlaying)
        {
            leftDoorAudioSource.Play();
        }

        if (!rightDoorAudioSource.isPlaying)
        {
            rightDoorAudioSource.Play();
        }

        Vector3 leftDoorTargetPosition = leftDoor.transform.position + new Vector3(doorsMoveDistance, 0, 0);
        Vector3 rightDoorTargetPosition = rightDoor.transform.position + new Vector3(-doorsMoveDistance, 0, 0);

        Vector3 leftDoorStartPosition = leftDoor.transform.position;
        Vector3 rightDoorStartPosition = rightDoor.transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < doorMoveTime)
        {
            leftDoor.transform.position = Vector3.Lerp(leftDoorStartPosition, leftDoorTargetPosition, elapsedTime / doorMoveTime);
            rightDoor.transform.position = Vector3.Lerp(rightDoorStartPosition, rightDoorTargetPosition, elapsedTime / doorMoveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leftDoor.transform.position = leftDoorTargetPosition;
        rightDoor.transform.position = rightDoorTargetPosition;

        if (!leftDoorAudioSource.isPlaying)
        {
            leftDoorAudioSource.Play();
        }

        if (!rightDoorAudioSource.isPlaying)
        {
            rightDoorAudioSource.Play();
        }

        StartCoroutine(LiftFloor());
    }

    IEnumerator LiftFloor()
    {
        Vector3 floorTargetPosition = floor.transform.position + new Vector3(0, floorMoveDistance, 0);
        Vector3 floorStartPosition = floor.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < floorMoveTime)
        {
            floor.transform.position = Vector3.Lerp(floorStartPosition, floorTargetPosition, elapsedTime / floorMoveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        floor.transform.position = floorTargetPosition; 

        if (floorAudioSource.isPlaying)
        {
            floorAudioSource.Stop();
        }
        
        if (!leftDoorAudioSource.isPlaying)
        {
            leftDoorAudioSource.Play();
        }
    }
}
